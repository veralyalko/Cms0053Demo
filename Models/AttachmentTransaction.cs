namespace Cms0053Demo.Models;

public class AttachmentTransaction
{
    public int Id { get; set; }
    public int ScenarioId { get; set; }
    public DemoScenario Scenario { get; set; } = null!;

    public string TrackingNumber { get; set; } = "";
    public string ProviderNPI { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateOnly PatientDOB { get; set; }
    public string ClaimNumber { get; set; } = "";
    public string DocumentType { get; set; } = "";
    public string LoincCode { get; set; } = "";
    public DateTime SubmittedAt { get; set; }
    public string X12Envelope { get; set; } = "";
    public string CdaDocument { get; set; } = "";
    public string Status { get; set; } = TransactionStatus.Received;
    public bool IsTampered { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ValidationStageResult> StageResults { get; set; } = [];
    public ICollection<AuditEvent> AuditEvents { get; set; } = [];
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
