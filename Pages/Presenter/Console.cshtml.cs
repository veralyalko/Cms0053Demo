using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Presenter;

public class ConsoleModel(AppDbContext db) : PageModel
{
    public List<DemoScenario> Scenarios { get; set; } = [];
    public Dictionary<string, AttachmentTransaction> LastByScenario { get; set; } = [];

    public async Task OnGetAsync()
    {
        Scenarios = await db.DemoScenarios.OrderBy(s => s.Code).ToListAsync();

        var recent = await db.AttachmentTransactions
            .Include(t => t.Scenario)
            .OrderByDescending(t => t.Id)
            .Take(30)
            .ToListAsync();

        foreach (var tx in recent)
        {
            if (!LastByScenario.ContainsKey(tx.Scenario.Code))
                LastByScenario[tx.Scenario.Code] = tx;
        }
    }

    public string TxUrl(int screen, string? scenarioCode = null)
    {
        AttachmentTransaction? tx = null;
        if (scenarioCode is not null)
            LastByScenario.TryGetValue(scenarioCode, out tx);
        else if (LastByScenario.Count > 0)
            tx = LastByScenario.Values.OrderByDescending(t => t.Id).FirstOrDefault();

        return screen switch
        {
            1 => "/Demo/TodaysReality",
            2 => "/Demo/Compliance",
            3 => tx != null ? $"/Demo/TradingPartner?transactionId={tx.Id}" : "/Demo/TradingPartner",
            4 => tx != null ? $"/Demo/Pipeline?transactionId={tx.Id}" : "/Demo/Pipeline",
            5 => tx != null ? $"/Demo/Repository?transactionId={tx.Id}" : "/Demo/Repository",
            6 => tx != null ? $"/Demo/ClaimsHandoff?transactionId={tx.Id}" : "/Demo/ClaimsHandoff",
            7 => tx != null ? $"/Demo/AuditTrail?transactionId={tx.Id}" : "/Demo/AuditTrail",
            _ => "/"
        };
    }

    public static readonly ScreenInfo[] AllScreens =
    [
        new(1, "Today's Reality",
            @"Before I show you the platform, let me put the problem in context. Your team is spending $3.60 per transaction processing attachments manually. 35% are still arriving by fax or mail — no tracking, no acknowledgement. The average cycle time from submission to adjudication is 21 days. That's 21 days of working capital tied up waiting on documentation. Meanwhile, CMS is moving up the compliance mandate. This screen sets the 'before' picture. Everything after this is the 'after.'",
            [
                ("Where do these numbers come from?",
                 "CAQH Index 2023 — the industry benchmark for electronic administrative transactions. These are national medians; large plans often see higher per-transaction costs because of legacy fax infrastructure."),
                ("Our denial rate is lower than 26% — is this relevant to us?",
                 "Great — that means you're already doing something right. The 26% is the industry median for initial denials tied to missing or incomplete attachments. Our target for clients is below 3% through pre-submission validation."),
                ("This seems like an ops problem, not a technology problem.",
                 "The ops burden is a direct consequence of unstructured ingestion. 3.2 FTE per 1,000 claims is what it costs to manually handle fax intake, data entry, and re-requests. Automate the intake, the FTE cost follows immediately.")
            ]),

        new(2, "Compliance Dashboard",
            @"CMS-0053-F requires all covered health plans to support electronic claim attachments using X12 275 — the structured, HIPAA-compliant attachment transaction. Large plans must comply by January 2027; all plans by January 2028. This is not optional. The penalty exposure is real, but more importantly, the payers who streamline provider experience will retain network relationships. This screen shows Meridian's current readiness score and the specific gaps we need to close. Let me walk through the gap assessment.",
            [
                ("Is the 2027 deadline final?",
                 "The proposed rule has been published; the final rule is pending. We're building to the proposed requirements, and most plans are treating 2026 as their internal preparation deadline to avoid regulatory risk. Better to be early."),
                ("What does '96% readiness' actually mean?",
                 "We mapped CMS-0053-F's eight specific technical requirements against this platform. Two areas — clearinghouse connectivity for all trading partners and full trading partner enrollment automation — are in scope for Phase 2. The 4% reflects those."),
                ("What's the real penalty exposure for non-compliance?",
                 "HIPAA civil money penalty up to $100 per day per violation per covered entity. The bigger risk is operational: if your providers can't submit electronically, they'll prefer payers who make it easy. Network leakage is the real cost.")
            ]),

        new(3, "Trading Partner Gateway",
            @"This is where an X12 275 attachment transaction arrives from the trading partner. Watch the left panel — you're looking at the actual raw X12 envelope received over SFTP. The ISA segment is the interchange control header: sender ID, receiver ID, control number, and timestamp. ST*275 confirms this is a claim attachment transaction. NM1*41 carries the submitting provider's NPI. TRN02 is the claim reference — that's how we later match this back to the claim. DMG has patient demographics. This is real X12 parsing. Not a simulation.",
            [
                ("Does this work with any clearinghouse?",
                 "The gateway is clearinghouse-agnostic — it accepts standard X12 275 over SFTP, AS2, or REST. We have pre-built connectors for Availity, Change Healthcare, and Veridian Health Exchange. Any clearinghouse that transmits standard X12 will work."),
                ("What about trading partner enrollment?",
                 "Trading partners register via SFTP key exchange or AS2 certificate. Enrollment takes 2–3 business days. The ISA13 control number uniqueness check you see here happens automatically on every submission — duplicate detection is built in."),
                ("Can providers submit directly without a clearinghouse?",
                 "Yes — the provider portal supports direct X12 275 submission. EMR systems that generate C-CDA documents can attach them directly to the X12 envelope. Athenahealth, Epic, and Cerner all have X12 275 export capability.")
            ]),

        new(4, "Validation Pipeline",
            @"This is the technical heart of the platform. Eight validation stages run in sequence, completing in under 1,600 milliseconds. Stage 1 parses the X12 envelope — ISA, GS, ST, BHT, NM1 loops, TRN, DMG. Stage 2 validates the CDA document against the HL7 CDA R2 XSD schema. Stage 3 checks C-CDA template conformance — required sections, template OIDs, LOINC codes. Stage 4 runs Schematron rules. Stage 5 classifies the document by LOINC. Stage 6 verifies the XMLDSig digital signature using the provider's X.509 certificate. Stage 7 matches the attachment to the claim. Stage 8 appends to the cryptographic audit chain. Click any row to expand the JSON detail.",
            [
                ("Is the pipeline blocking or asynchronous?",
                 "Synchronous by design — we return an acknowledgement to the trading partner within 3 seconds. The provider gets a structured response immediately, not a rejection letter days later. If stage 3 fails, the provider knows exactly which section is missing before it ever touches your claims team."),
                ("What happens when a stage fails?",
                 "The pipeline halts at the failed stage, records the structured error, and returns an X12 277 rejection with the specific reason code. The full audit trail is still written — every attempt is logged with its hash. Scenario B shows this exact failure and recovery."),
                ("Can you explain Stage 6 — the XMLDSig check?",
                 "We verify an enveloped XML digital signature using the provider's RSA-2048/SHA-256 X.509 certificate. Same mechanism as healthcare information exchange. If anyone modifies the CDA document after it's signed — even a single character — the signature check fails at Stage 6.")
            ]),

        new(5, "Document Repository",
            @"Every received attachment is stored here — indexed by LOINC code, patient name, provider NPI, and claim number. The metadata panel on the right shows the full document context: LOINC code and display name, clinician, submission timestamp. More important is the XMLDSig certificate card: you can see the cert subject, thumbprint, and whether the signature verified. This is your proof of document integrity — chain of custody from provider to payer. Click 'Show full' to view the entire signed CDA XML, including the Signature element at the bottom.",
            [
                ("How long are documents retained?",
                 "Configurable — default is 7 years to meet HIPAA minimum retention. The stored file is AES-256 encrypted at rest. The document hash in the audit chain allows you to prove the stored copy is identical to what was received, at any point in those 7 years."),
                ("Can reviewers access documents directly from their work queue?",
                 "Yes — role-based access control. Claims adjusters see matched documents surfaced in their adjudication queue. UM reviewers see routed attachments. Audit staff have read-only access to the full repository. All access is logged in the audit chain."),
                ("What if the provider sends the wrong document type?",
                 "Stage 5 (LOINC classification) catches document type mismatches. If the X12 envelope specifies 'Operative Note' but the CDA says 'Discharge Summary,' that's flagged as a warning. Stage 3 will catch missing required sections for the wrong template. The provider gets specific feedback before it reaches your team.")
            ]),

        new(6, "Claims + UM Handoff",
            @"Once the pipeline completes, the attachment routes to the appropriate downstream system. For Scenario A — the cardiac discharge summary — it routes directly to CoreClaim auto-adjudication with a predicted Approve decision. You can see the estimated payment at 82% of billed, and the ERA 835 remittance timeline. For Scenario C — the orthopedic H&P — the prior authorization trigger routes the attachment to PriorPath UM Solutions. The attachment arrives pre-validated, with full metadata, no rekeying. The UM reviewer gets the clinical document, not a fax.",
            [
                ("Does this replace our adjudication system?",
                 "No — iXDataBridge sits upstream. We integrate with CoreClaim and any FHIR-enabled claims platform via API. The attachment arrives pre-validated, pre-matched, with structured metadata. Your adjudication system gets clean input; it doesn't change how adjudication works."),
                ("What about the ERA 835 on the return path?",
                 "The adjudication decision flows back via X12 835 ERA. The attachment tracking number is preserved through the entire lifecycle — you can audit the full chain from X12 275 submission to ERA 835 remittance. Both directions are covered."),
                ("How does the UM authorization decision get back to the provider?",
                 "PriorPath returns an X12 278 authorization response, which we receive and map to the claim. The provider gets an X12 277 acknowledgement with the auth number. That's Phase 2 scope — the routing you see here is Phase 1.")
            ]),

        new(7, "Audit Trail",
            @"This is the cryptographic audit chain. Every event — X12 parsing, CDA validation, signature verification, claim match, routing — is hashed with SHA-256. Each block contains the hash of the previous block. You cannot insert, delete, or modify an event without breaking every hash that follows. This is not a database log. It's cryptographically tamper-evident at the same level as a financial audit trail. In a compliance examination, you can prove — mathematically — that a specific attachment was received, validated unmodified, and routed at a specific millisecond. That's your CMS-0053 documentation of compliance.",
            [
                ("Is this admissible as evidence in a dispute?",
                 "It's cryptographically verifiable evidence — the same hash chain mechanism used in financial audit trails and FHIR AuditEvent logs. It satisfies HIPAA audit logging requirements and is exactly what a CMS compliance examiner wants to see. Whether it's legally admissible depends on jurisdiction, but it's the strongest non-repudiation artifact available."),
                ("Can we export the audit trail?",
                 "Full export to CSV or FHIR AuditEvent resource — both available via API. The SHA-256 hash values export with it, so the chain can be independently verified by a third party. That's important for payer-provider disputes about whether an attachment was received."),
                ("What about right-to-deletion requests?",
                 "Healthcare data under HIPAA supersedes state privacy law in most cases — you generally can't delete PHI subject to a HIPAA retention obligation. For the narrow cases that do require deletion, we implement a cryptographic tombstone that preserves chain integrity while marking the record as expunged.")
            ])
    ];
}

public record ScreenInfo(
    int Number,
    string Title,
    string TalkTrack,
    (string Q, string A)[] QandA);
