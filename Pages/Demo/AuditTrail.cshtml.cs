using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Demo;

public class AuditTrailModel(AppDbContext db, AuditHashService auditHash) : PageModel
{
    public AttachmentTransaction? Transaction { get; set; }
    public DemoScenario? Scenario { get; set; }
    public List<AuditEvent> Events { get; set; } = [];
    public bool ChainValid { get; set; }

    public async Task OnGetAsync(int? transactionId)
    {
        if (!transactionId.HasValue) return;

        Transaction = await db.AttachmentTransactions
            .Include(t => t.Scenario)
            .Include(t => t.AuditEvents.OrderBy(e => e.OccurredAt))
            .FirstOrDefaultAsync(t => t.Id == transactionId.Value);

        if (Transaction is null) return;

        Scenario = Transaction.Scenario;
        Events = Transaction.AuditEvents.ToList();

        ChainValid = auditHash.VerifyChain(Events.Select(e =>
            (e.EventType, e.Description, e.OccurredAt, e.EventHash, e.PreviousHash)));
    }

    public static string EventBadgeClass(string eventType) => eventType switch
    {
        "X12_PARSED"         => "bg-info text-dark",
        "CDA_SCHEMA_VALID"   => "bg-info text-dark",
        "CCDA_TEMPLATE_VALID"=> "bg-info text-dark",
        "SCHEMATRON_PASSED"  => "bg-info text-dark",
        "LOINC_CLASSIFIED"   => "bg-primary",
        "SIGNATURE_VERIFIED" => "bg-success",
        "CLAIM_MATCHED"      => "bg-success",
        "PIPELINE_COMPLETE"  => "bg-success",
        _                    => "bg-secondary"
    };

    public static string HashAbbrev(string hash) =>
        hash.Length >= 16 ? hash[..8] + "…" + hash[^8..] : hash;
}
