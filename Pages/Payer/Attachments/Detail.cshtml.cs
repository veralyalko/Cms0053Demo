using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Payer.Attachments;

public class DetailModel(AppDbContext db, AuditHashService auditHash) : PageModel
{
    public AttachmentTransaction? Transaction { get; set; }
    public List<ValidationStageResult> Stages { get; set; } = [];
    public List<AuditEvent> AuditEvents { get; set; } = [];
    public bool ChainValid { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Transaction = await db.AttachmentTransactions
            .Include(t => t.Claim)
            .Include(t => t.AttachmentRequest)
            .Include(t => t.ClearinghouseRecord)
            .Include(t => t.StageResults.OrderBy(s => s.StageOrder))
            .Include(t => t.AuditEvents.OrderBy(e => e.OccurredAt))
            .FirstOrDefaultAsync(t => t.Id == id);

        if (Transaction is null) return NotFound();

        Stages     = Transaction.StageResults.ToList();
        AuditEvents = Transaction.AuditEvents.ToList();

        ChainValid = auditHash.VerifyChain(AuditEvents.Select(e =>
            (e.EventType, e.Description, e.OccurredAt, e.EventHash, e.PreviousHash)));

        return Page();
    }

    public static string StageIcon(bool passed) => passed ? "✓" : "✗";
    public static string StageClass(bool passed) => passed ? "text-success" : "text-danger";
    public static string HashAbbrev(string hash) =>
        hash.Length >= 16 ? hash[..8] + "…" + hash[^8..] : hash;
}
