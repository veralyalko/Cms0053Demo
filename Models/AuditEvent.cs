namespace Cms0053Demo.Models;

public class AuditEvent
{
    public int Id { get; set; }
    public int AttachmentTransactionId { get; set; }
    public AttachmentTransaction Transaction { get; set; } = null!;

    public string EventType { get; set; } = "";
    public string Description { get; set; } = "";
    public string EventHash { get; set; } = "";
    public string PreviousHash { get; set; } = "";
    public DateTime OccurredAt { get; set; }
}
