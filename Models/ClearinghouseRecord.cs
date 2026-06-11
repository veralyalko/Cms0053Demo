namespace Cms0053Demo.Models;

// Mock clearinghouse staging table (unsolicited clearinghouse workflow)
public class ClearinghouseRecord
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = "";
    public string ProviderNPI { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateOnly PatientDOB { get; set; }
    public string ClaimNumber { get; set; } = "";
    public DateOnly ServiceDate { get; set; }
    public string DocumentType { get; set; } = "";
    public string LoincCode { get; set; } = "";
    public string Notes { get; set; } = "";

    // Stored file (C-CDA or PDF uploaded by provider)
    public string OriginalFileName { get; set; } = "";
    public string StoredFileName { get; set; } = "";
    public long FileSizeBytes { get; set; }
    public string ContentType { get; set; } = "";

    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = ClearinghouseStatus.New;
    public DateTime? PulledAt { get; set; }

    public ICollection<AttachmentTransaction> Transactions { get; set; } = [];
}

public static class ClearinghouseStatus
{
    public const string New    = "New";
    public const string Pulled = "Pulled";
}
