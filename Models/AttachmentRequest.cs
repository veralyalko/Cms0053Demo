namespace Cms0053Demo.Models;

// Payer-generated request for documentation (X12 277 proxy — solicited workflow)
public class AttachmentRequest
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public Claim Claim { get; set; } = null!;

    public string TrackingNumber { get; set; } = "";
    public string DocumentTypeRequested { get; set; } = "";
    public string RequestReason { get; set; } = "";
    public DateTime RequestedAt { get; set; }
    public DateTime DueDate { get; set; }
    public string ProviderEmail { get; set; } = "";
    public string SecureUploadToken { get; set; } = "";    // used in /Provider/Upload/{token}
    public string Status { get; set; } = RequestStatus.Pending;

    public ICollection<AttachmentTransaction> Transactions { get; set; } = [];
}

public static class RequestStatus
{
    public const string Pending   = "Pending";
    public const string Fulfilled = "Fulfilled";
    public const string Expired   = "Expired";
    public const string Cancelled = "Cancelled";
}
