using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Demo;

public class ComplianceModel(AppDbContext db) : PageModel
{
    public int TotalTransactions { get; set; }
    public int PassedTransactions { get; set; }
    public int FailedTransactions { get; set; }

    public async Task OnGetAsync()
    {
        TotalTransactions   = await db.AttachmentTransactions.CountAsync();
        PassedTransactions  = await db.AttachmentTransactions
            .CountAsync(t => t.Status == TransactionStatus.Routed || t.Status == "UM Review");
        FailedTransactions  = await db.AttachmentTransactions
            .CountAsync(t => t.Status == TransactionStatus.Failed);
    }
}
