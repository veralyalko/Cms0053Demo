using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages;

public class IndexModel(AppDbContext db) : PageModel
{
    public int OpenClaimsCount { get; set; }
    public int PendingRequestsCount { get; set; }
    public int ClearinghouseQueueCount { get; set; }
    public int TotalTransactionsCount { get; set; }
    public List<AttachmentTransaction> RecentTransactions { get; set; } = [];

    public async Task OnGetAsync()
    {
        OpenClaimsCount         = await db.Claims.CountAsync(c => c.Status == ClaimStatus.Open);
        PendingRequestsCount    = await db.AttachmentRequests.CountAsync(r => r.Status == RequestStatus.Pending);
        ClearinghouseQueueCount = await db.ClearinghouseRecords.CountAsync(c => c.Status == ClearinghouseStatus.New);
        TotalTransactionsCount  = await db.AttachmentTransactions.CountAsync();

        RecentTransactions = await db.AttachmentTransactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostResetAsync()
    {
        await db.AuditEvents.ExecuteDeleteAsync();
        await db.ValidationStageResults.ExecuteDeleteAsync();
        await db.AttachmentTransactions.ExecuteDeleteAsync();
        await db.ClearinghouseRecords.ExecuteDeleteAsync();
        await db.AttachmentRequests.ExecuteDeleteAsync();
        // Delete uploaded files
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (Directory.Exists(uploadsDir))
            foreach (var f in Directory.GetFiles(uploadsDir)) System.IO.File.Delete(f);
        return RedirectToPage();
    }
}
