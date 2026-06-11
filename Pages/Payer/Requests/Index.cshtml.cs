using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Payer.Requests;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<AttachmentRequest> Requests { get; set; } = [];

    public async Task OnGetAsync()
    {
        Requests = await db.AttachmentRequests
            .Include(r => r.Claim)
            .Include(r => r.Transactions)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }
}
