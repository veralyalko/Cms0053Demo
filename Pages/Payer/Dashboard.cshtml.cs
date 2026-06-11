using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Payer;

public class DashboardModel(AppDbContext db) : PageModel
{
    public List<AttachmentTransaction> Transactions { get; set; } = [];
    public int RoutedCount { get; set; }
    public int FailedCount { get; set; }
    public int PendingRequestsCount { get; set; }
    public int ClearinghouseQueueCount { get; set; }

    public async Task OnGetAsync()
    {
        Transactions = await db.AttachmentTransactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(50)
            .ToListAsync();

        RoutedCount             = Transactions.Count(t => t.Status == TransactionStatus.Routed);
        FailedCount             = Transactions.Count(t => t.Status == TransactionStatus.Failed);
        PendingRequestsCount    = await db.AttachmentRequests.CountAsync(r => r.Status == RequestStatus.Pending);
        ClearinghouseQueueCount = await db.ClearinghouseRecords.CountAsync(c => c.Status == ClearinghouseStatus.New);
    }
}
