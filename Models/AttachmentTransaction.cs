namespace Cms0053Demo.Models;

public class AttachmentTransaction
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = "";
    public string SourceType { get; set; } = Models.SourceType.Direct;

    // Submission metadata
    public string ProviderNPI { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateOnly PatientDOB { get; set; }
    public string ClaimNumber { get; set; } = "";
    public DateOnly ServiceDate { get; set; }
    public string DocumentType { get; set; } = "";
    public string LoincCode { get; set; } = "";
    public string Notes { get; set; } = "";

    // File
    public string OriginalFileName { get; set; } = "";
    public string StoredFileName { get; set; } = "";
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = "";
    public string FileHash { get; set; } = "";

    // Pipeline artifacts (stored for display on detail screens)
    public string X12Envelope { get; set; } = "";
    public string CdaDocument { get; set; } = "";

    // Linkage
    public int? ClaimId { get; set; }
    public Claim? Claim { get; set; }
    public int? AttachmentRequestId { get; set; }
    public AttachmentRequest? AttachmentRequest { get; set; }
    public int? ClearinghouseRecordId { get; set; }
    public ClearinghouseRecord? ClearinghouseRecord { get; set; }

    public string Status { get; set; } = TransactionStatus.Received;

    public DateTime SubmittedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ValidationStageResult> StageResults { get; set; } = [];
    public ICollection<AuditEvent> AuditEvents { get; set; } = [];
}

public static class SourceType
{
    public const string Solicited              = "Solicited";
    public const string UnsolicitedClearinghouse = "UnsolicitedClearinghouse";
    public const string Direct                 = "Direct";
}

public static class TransactionStatus
{
    public const string Received   = "Received";
    public const string Processing = "Processing";
    public const string Passed     = "Passed";
    public const string Failed     = "Failed";
    public const string Matched    = "Matched";
    public const string Routed     = "Routed";
}
