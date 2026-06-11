using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Clearinghouse;

public class SubmitModel(AppDbContext db, FileStorageService fileStorage) : PageModel
{
    public List<Claim> Claims { get; set; } = [];
    public List<EmrDocument> EmrDocuments { get; set; } = [];

    [BindProperty] public string ProviderNPI { get; set; } = "";
    [BindProperty] public string ProviderName { get; set; } = "";
    [BindProperty] public string PatientName { get; set; } = "";
    [BindProperty] public string PatientDOBString { get; set; } = "";
    [BindProperty] public string ClaimNumber { get; set; } = "";
    [BindProperty] public string ServiceDateString { get; set; } = "";
    [BindProperty] public string DocumentType { get; set; } = "";
    [BindProperty] public string LoincCode { get; set; } = "";
    [BindProperty] public string Notes { get; set; } = "";
    [BindProperty] public int? EmrDocumentId { get; set; }
    [BindProperty] public IFormFile? UploadedFile { get; set; }

    public async Task OnGetAsync()
    {
        Claims       = await db.Claims.Where(c => c.Status == ClaimStatus.Open).ToListAsync();
        EmrDocuments = await db.EmrDocuments.OrderBy(d => d.PatientName).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Claims       = await db.Claims.Where(c => c.Status == ClaimStatus.Open).ToListAsync();
        EmrDocuments = await db.EmrDocuments.OrderBy(d => d.PatientName).ToListAsync();

        if (!DateOnly.TryParse(PatientDOBString, out var dob))
            ModelState.AddModelError(nameof(PatientDOBString), "Invalid date of birth.");
        if (!DateOnly.TryParse(ServiceDateString, out var svcDate))
            ModelState.AddModelError(nameof(ServiceDateString), "Invalid service date.");
        if (EmrDocumentId is null && UploadedFile is null)
            ModelState.AddModelError("", "Select an EMR document or upload a file.");
        if (!ModelState.IsValid) return Page();

        string originalFileName, contentType, storedFileName, fileHash;
        long fileSize;

        if (EmrDocumentId.HasValue)
        {
            var emr = await db.EmrDocuments.FindAsync(EmrDocumentId.Value);
            var content = fileStorage.ReadSyntheaSample(emr!.FileName);
            originalFileName = emr.FileName;
            contentType = "text/xml";
            (storedFileName, fileSize, fileHash) = await fileStorage.SaveTextAsync(content, ".xml");
        }
        else
        {
            (storedFileName, fileSize, fileHash) = await fileStorage.SaveAsync(UploadedFile!);
            originalFileName = UploadedFile!.FileName;
            contentType = UploadedFile.ContentType;
        }

        // [X12-275-CLEARINGHOUSE-PLACEHOLDER] — in production, provider submits an X12 275
        // to the clearinghouse via SFTP/AS2. This mock stores it in the staging table.
        var record = new ClearinghouseRecord
        {
            TrackingNumber   = $"VHX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            ProviderNPI      = ProviderNPI,
            ProviderName     = ProviderName,
            PatientName      = PatientName,
            PatientDOB       = dob,
            ClaimNumber      = ClaimNumber,
            ServiceDate      = svcDate,
            DocumentType     = DocumentType,
            LoincCode        = LoincCode,
            Notes            = Notes,
            OriginalFileName = originalFileName,
            StoredFileName   = storedFileName,
            FileSizeBytes    = fileSize,
            ContentType      = contentType,
            SubmittedAt      = DateTime.UtcNow,
            Status           = ClearinghouseStatus.New,
        };

        db.ClearinghouseRecords.Add(record);
        await db.SaveChangesAsync();

        TempData["SuccessTrackingNumber"] = record.TrackingNumber;
        return RedirectToPage("Queue");
    }
}
