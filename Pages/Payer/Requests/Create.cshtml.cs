using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Payer.Requests;

public class CreateModel(AppDbContext db) : PageModel
{
    public List<Claim> OpenClaims { get; set; } = [];

    [BindProperty] public int ClaimId { get; set; }
    [BindProperty] public string DocumentTypeRequested { get; set; } = "";
    [BindProperty] public string RequestReason { get; set; } = "";
    [BindProperty] public string ProviderEmail { get; set; } = "";
    [BindProperty] public int DueDays { get; set; } = 14;

    public async Task OnGetAsync()
    {
        OpenClaims = await db.Claims.Where(c => c.Status == ClaimStatus.Open).ToListAsync();
    }

    // [X12-277-PLACEHOLDER] — in production, generate and transmit an X12 277 Additional
    // Information Request to the provider/clearinghouse instead of saving a DB record.
    public async Task<IActionResult> OnPostAsync()
    {
        OpenClaims = await db.Claims.Where(c => c.Status == ClaimStatus.Open).ToListAsync();

        if (!ModelState.IsValid || ClaimId == 0)
        {
            ModelState.AddModelError("", "Please select a claim.");
            return Page();
        }

        var request = new AttachmentRequest
        {
            ClaimId               = ClaimId,
            TrackingNumber        = $"REQ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            DocumentTypeRequested = DocumentTypeRequested,
            RequestReason         = RequestReason,
            ProviderEmail         = ProviderEmail,
            SecureUploadToken     = Guid.NewGuid().ToString("N"),
            RequestedAt           = DateTime.UtcNow,
            DueDate               = DateTime.UtcNow.AddDays(DueDays),
            Status                = RequestStatus.Pending,
        };

        db.AttachmentRequests.Add(request);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
