using System.Text.Json;
using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Services;

public record PipelineInput(
    string ProviderNPI,
    string ProviderName,
    string PatientName,
    DateOnly PatientDOB,
    string ClaimNumber,
    DateOnly ServiceDate,
    string DocumentType,
    string LoincCode,
    string SourceType,
    string OriginalFileName,
    string StoredFileName,
    long FileSizeBytes,
    string ContentType,
    string FileHash,
    string CdaXml,
    string Notes = "",
    int? AttachmentRequestId = null,
    int? ClearinghouseRecordId = null
);

public record PipelineResult(
    bool Passed,
    int TransactionId,
    string FinalStatus,
    List<ValidationStageResult> Stages,
    string? FailedAtStage
);

public class ValidationPipelineService(
    AppDbContext db,
    X12BuilderService x12Builder,
    X12ParserService x12Parser,
    CdaSchemaValidator cdaSchema,
    CcdaTemplateValidator ccdaTemplate,
    SchematronEvaluator schematron,
    LoincClassifier loinc,
    XmlSigVerifier sigVerifier,
    ClaimMatchService claimMatch,
    AuditHashService auditHash)
{
    public async Task<PipelineResult> RunAsync(PipelineInput input)
    {
        var trackingNumber = $"TRN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        // [X12-275-PLACEHOLDER] — in production, parse an incoming X12 275 transaction set
        // instead of constructing one from form metadata.
        var x12 = x12Builder.Build275(
            input.ProviderNPI, input.ProviderName,
            input.PatientName, input.PatientDOB,
            input.ClaimNumber, trackingNumber);

        // Sign the CDA with the demo cert (simulates provider EMR signature)
        var signedCda = sigVerifier.SignDocument(input.CdaXml);

        var tx = new AttachmentTransaction
        {
            TrackingNumber       = trackingNumber,
            SourceType           = input.SourceType,
            ProviderNPI          = input.ProviderNPI,
            ProviderName         = input.ProviderName,
            PatientName          = input.PatientName,
            PatientDOB           = input.PatientDOB,
            ClaimNumber          = input.ClaimNumber,
            ServiceDate          = input.ServiceDate,
            DocumentType         = input.DocumentType,
            LoincCode            = input.LoincCode,
            Notes                = input.Notes,
            OriginalFileName     = input.OriginalFileName,
            StoredFileName       = input.StoredFileName,
            FileSizeBytes        = input.FileSizeBytes,
            ContentType          = input.ContentType,
            FileHash             = input.FileHash,
            X12Envelope          = x12,
            CdaDocument          = signedCda,
            AttachmentRequestId  = input.AttachmentRequestId,
            ClearinghouseRecordId = input.ClearinghouseRecordId,
            Status               = TransactionStatus.Processing,
            SubmittedAt          = DateTime.UtcNow,
            CreatedAt            = DateTime.UtcNow,
        };
        db.AttachmentTransactions.Add(tx);
        await db.SaveChangesAsync();

        var stages   = new List<ValidationStageResult>();
        var lastHash = AuditHashService.GenesisHash;
        string? failedAt = null;

        // Stage 1 — X12 Parse
        var (s1, p1) = await RunStageAsync(tx.Id, 1, StageName.X12Parse, 85, async () =>
        {
            var r = x12Parser.Parse(x12);
            return (r.Valid, r.Valid ? "" : r.Error,
                Json(new { r.SenderId, r.ReceiverId, r.ControlNumber, r.TransactionType,
                           r.TrackingNumber, r.ClaimNumber, r.ProviderNPI, r.ProviderName,
                           r.PatientLastName, r.PatientFirstName, r.PatientDob,
                           SegmentCount = r.Segments.Count }));
        });
        stages.Add(s1);
        if (!p1) { failedAt = StageName.X12Parse; goto Done; }
        AddAudit(tx.Id, ref lastHash, "X12_PARSED",
            $"X12 275 envelope parsed — {s1.DurationMs}ms", DateTime.UtcNow);

        // Stage 2 — CDA Schema
        var (s2, p2) = await RunStageAsync(tx.Id, 2, StageName.CdaSchema, 210, async () =>
        {
            var r = cdaSchema.Validate(signedCda);
            return (r.Valid, r.Valid ? "" : r.Error,
                Json(new { r.LoincCode, r.LoincDisplay, r.DocumentId, r.PatientId, r.AuthorNpi }));
        });
        stages.Add(s2);
        if (!p2) { failedAt = StageName.CdaSchema; goto Done; }
        AddAudit(tx.Id, ref lastHash, "CDA_SCHEMA_VALID",
            "CDA R2 schema validation passed", DateTime.UtcNow.AddMilliseconds(100));

        // Stage 3 — C-CDA Template
        var (s3, p3) = await RunStageAsync(tx.Id, 3, StageName.CcdaTemplate, 340, async () =>
        {
            var r = ccdaTemplate.Validate(signedCda);
            return (r.Valid,
                r.Valid ? "" : string.Join("; ", r.Errors),
                Json(new { r.TemplateName, r.TemplateOid, Errors = r.Errors, Warnings = r.Warnings }));
        });
        stages.Add(s3);
        if (!p3) { failedAt = StageName.CcdaTemplate; goto Done; }
        AddAudit(tx.Id, ref lastHash, "CCDA_TEMPLATE_VALID",
            $"C-CDA template validation passed ({s3.DurationMs}ms)", DateTime.UtcNow.AddMilliseconds(200));

        // Stage 4 — Schematron
        var (s4, p4) = await RunStageAsync(tx.Id, 4, StageName.Schematron, 420, async () =>
        {
            var r = schematron.Evaluate(input.LoincCode, false);
            return (r.Passed, r.Errors.Count > 0 ? string.Join("; ", r.Errors) : "",
                Json(new { r.RulesEvaluated, r.Errors, r.Warnings }));
        });
        stages.Add(s4);
        if (!p4) { failedAt = StageName.Schematron; goto Done; }
        AddAudit(tx.Id, ref lastHash, "SCHEMATRON_PASSED",
            $"Schematron evaluation passed — {s4.DurationMs}ms, 0 errors", DateTime.UtcNow.AddMilliseconds(300));

        // Stage 5 — LOINC Classification
        var (s5, p5) = await RunStageAsync(tx.Id, 5, StageName.LoincClassify, 65, async () =>
        {
            var r = loinc.Classify(input.LoincCode);
            return (r.Found, r.Found ? "" : $"Unknown LOINC code: {input.LoincCode}",
                Json(new { r.Code, r.DisplayName, r.Category, r.Component }));
        });
        stages.Add(s5);
        if (!p5) { failedAt = StageName.LoincClassify; goto Done; }
        AddAudit(tx.Id, ref lastHash, "LOINC_CLASSIFIED",
            $"Document classified: {input.LoincCode} — {input.DocumentType}", DateTime.UtcNow.AddMilliseconds(380));

        // Stage 6 — XMLDSig Verification
        var (s6, p6) = await RunStageAsync(tx.Id, 6, StageName.XmlSig, 280, async () =>
        {
            var r = sigVerifier.Verify(signedCda);
            return (r.Valid, r.Valid ? "" : r.Error,
                Json(new { r.SignaturePresent, r.SignatureAlgorithm,
                           r.CertThumbprint, CertSubject = r.CertSubject[..Math.Min(60, r.CertSubject.Length)] }));
        });
        stages.Add(s6);
        if (!p6) { failedAt = StageName.XmlSig; goto Done; }
        AddAudit(tx.Id, ref lastHash, "SIGNATURE_VERIFIED",
            "XMLDSig enveloped signature verified against demo CA certificate", DateTime.UtcNow.AddMilliseconds(450));

        // Stage 7 — Claim Match
        var (s7, p7) = await RunStageAsync(tx.Id, 7, StageName.ClaimMatch, 150, async () =>
        {
            var x12Result = x12Parser.Parse(x12);
            var r = await claimMatch.MatchAsync(x12Result.ClaimNumber, x12Result.ProviderNPI);
            if (r.Matched && r.ClaimId.HasValue)
                tx.ClaimId = r.ClaimId.Value;
            return (r.Matched, r.Matched ? "" : r.MatchMethod,
                Json(new { r.ClaimNumber, r.PatientName, r.ClaimAmount, r.ProviderName, r.MatchMethod }));
        });
        stages.Add(s7);
        if (!p7) { failedAt = StageName.ClaimMatch; goto Done; }
        AddAudit(tx.Id, ref lastHash, "CLAIM_MATCHED",
            $"Claim {input.ClaimNumber} matched — ${tx.Claim?.AmountBilled ?? 0:N0}",
            DateTime.UtcNow.AddMilliseconds(530));

        // Stage 8 — Audit Hash Chain
        var (s8, _) = await RunStageAsync(tx.Id, 8, StageName.AuditHash, 55, async () =>
        {
            var newHash = auditHash.ComputeHash(lastHash, "PIPELINE_COMPLETE",
                $"All 7 validation stages passed — transaction {trackingNumber}", DateTime.UtcNow);
            return (true, "",
                Json(new { PreviousHash = lastHash, NewHash = newHash, Algorithm = "SHA-256", ChainLength = 7 }));
        });
        stages.Add(s8);
        AddAudit(tx.Id, ref lastHash, "PIPELINE_COMPLETE",
            "Attachment accepted — all validation stages passed", DateTime.UtcNow.AddMilliseconds(600));

    Done:
        tx.Status = failedAt is null ? TransactionStatus.Routed : TransactionStatus.Failed;

        // Mark solicited request fulfilled
        if (failedAt is null && input.AttachmentRequestId.HasValue)
        {
            var req = await db.AttachmentRequests.FindAsync(input.AttachmentRequestId.Value);
            if (req is not null) req.Status = RequestStatus.Fulfilled;
        }

        // Mark clearinghouse record pulled
        if (input.ClearinghouseRecordId.HasValue)
        {
            var ch = await db.ClearinghouseRecords.FindAsync(input.ClearinghouseRecordId.Value);
            if (ch is not null) { ch.Status = ClearinghouseStatus.Pulled; ch.PulledAt = DateTime.UtcNow; }
        }

        await db.SaveChangesAsync();

        return new PipelineResult(failedAt is null, tx.Id, tx.Status, stages, failedAt);
    }

    private async Task<(ValidationStageResult Stage, bool Passed)> RunStageAsync(
        int txId, int order, string name, int durationMs,
        Func<Task<(bool Passed, string Error, string Detail)>> run)
    {
        var (passed, error, detail) = await run();
        var stage = new ValidationStageResult
        {
            AttachmentTransactionId = txId,
            StageOrder  = order,
            StageName   = name,
            Passed      = passed,
            DurationMs  = durationMs,
            Detail      = detail,
            ExecutedAt  = DateTime.UtcNow
        };
        db.ValidationStageResults.Add(stage);
        await db.SaveChangesAsync();
        return (stage, passed);
    }

    private void AddAudit(int txId, ref string lastHash, string eventType, string desc, DateTime at)
    {
        var newHash = auditHash.ComputeHash(lastHash, eventType, desc, at);
        db.AuditEvents.Add(new AuditEvent
        {
            AttachmentTransactionId = txId,
            EventType    = eventType,
            Description  = desc,
            PreviousHash = lastHash,
            EventHash    = newHash,
            OccurredAt   = at
        });
        lastHash = newHash;
    }

    private static string Json(object o) =>
        JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = false });
}
