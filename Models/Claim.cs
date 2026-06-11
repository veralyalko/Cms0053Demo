namespace Cms0053Demo.Models;

public class Claim
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateOnly PatientDOB { get; set; }
    public string ProviderNPI { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public DateOnly ServiceDate { get; set; }
    public string DiagnosisCode { get; set; } = "";
    public string DiagnosisDescription { get; set; } = "";
    public decimal AmountBilled { get; set; }
    public string Status { get; set; } = ClaimStatus.Open;

    public ICollection<AttachmentTransaction> Attachments { get; set; } = [];
    public ICollection<AttachmentRequest> AttachmentRequests { get; set; } = [];
}

public static class ClaimStatus
{
    public const string Open    = "Open";
    public const string Pending = "Pending";
    public const string Closed  = "Closed";
}
