namespace Cms0053Demo.Models;

public class ValidationStageResult
{
    public int Id { get; set; }
    public int AttachmentTransactionId { get; set; }
    public AttachmentTransaction Transaction { get; set; } = null!;

    public int StageOrder { get; set; }
    public string StageName { get; set; } = "";
    public bool Passed { get; set; }
    public int DurationMs { get; set; }
    public string Detail { get; set; } = "{}";
    public DateTime ExecutedAt { get; set; }
}

public static class StageName
{
    public const string X12Parse       = "X12 Envelope Parse";
    public const string CdaSchema      = "CDA Schema Validation";
    public const string CcdaTemplate   = "C-CDA Template Validation";
    public const string Schematron     = "Schematron Evaluation";
    public const string LoincClassify  = "LOINC Classification";
    public const string XmlSig         = "XMLDSig Verification";
    public const string ClaimMatch     = "Claim Match";
    public const string AuditHash      = "Audit Hash Chain";
}
