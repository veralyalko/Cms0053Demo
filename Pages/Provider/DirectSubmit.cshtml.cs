using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Provider;

public class DirectSubmitModel(AppDbContext db, ValidationPipelineService pipeline,
    FileStorageService fileStorage) : PageModel
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

    public async Task OnGetAsync(int? claimId)
    {
        Claims      = await db.Claims.Where(c => c.Status == ClaimStatus.Open).ToListAsync();
        EmrDocuments = await db.EmrDocuments.OrderBy(d => d.PatientName).ToListAsync();

        if (claimId.HasValue)
        {
            var claim = await db.Claims.FindAsync(claimId.Value);
            if (claim is not null)
            {
                ProviderNPI   = claim.ProviderNPI;
                ProviderName  = claim.ProviderName;
                PatientName   = claim.PatientName;
                PatientDOBString = claim.PatientDOB.ToString("yyyy-MM-dd");
                ClaimNumber   = claim.ClaimNumber;
                ServiceDateString = claim.ServiceDate.ToString("yyyy-MM-dd");
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Claims       = await db.Claims.Where(c => c.Status == ClaimStatus.Open).ToListAsync();
        EmrDocuments = await db.EmrDocuments.OrderBy(d => d.PatientName).ToListAsync();

        if (!DateOnly.TryParse(PatientDOBString, out var patientDob))
            ModelState.AddModelError(nameof(PatientDOBString), "Invalid date of birth.");
        if (!DateOnly.TryParse(ServiceDateString, out var serviceDate))
            ModelState.AddModelError(nameof(ServiceDateString), "Invalid service date.");
        if (EmrDocumentId is null && UploadedFile is null)
            ModelState.AddModelError("", "Select an EMR document or upload a C-CDA file.");
        if (!ModelState.IsValid) return Page();

        // Resolve CDA content
        string cdaXml;
        string originalFileName;
        long fileSize;
        string contentType;
        string storedFileName;
        string fileHash;

        if (EmrDocumentId.HasValue)
        {
            var emr = await db.EmrDocuments.FindAsync(EmrDocumentId.Value);
            cdaXml = fileStorage.ReadSyntheaSample(emr!.FileName);
            originalFileName = emr.FileName;
            var bytes = System.Text.Encoding.UTF8.GetByteCount(cdaXml);
            fileSize = bytes;
            contentType = "text/xml";
            // Store a copy in uploads for reference
            (storedFileName, fileSize, fileHash) = await fileStorage.SaveTextAsync(cdaXml, ".xml");
        }
        else
        {
            (storedFileName, fileSize, fileHash) = await fileStorage.SaveAsync(UploadedFile!);
            originalFileName = UploadedFile!.FileName;
            contentType      = UploadedFile.ContentType;
            // For uploaded C-CDA, read the stored content back
            cdaXml = fileStorage.ReadText(storedFileName);
            if (string.IsNullOrEmpty(cdaXml))
            {
                ModelState.AddModelError("", "Could not read uploaded file as text. Please upload a C-CDA XML file.");
                return Page();
            }
        }

        var input = new PipelineInput(
            ProviderNPI:        ProviderNPI,
            ProviderName:       ProviderName,
            PatientName:        PatientName,
            PatientDOB:         patientDob,
            ClaimNumber:        ClaimNumber,
            ServiceDate:        serviceDate,
            DocumentType:       DocumentType,
            LoincCode:          LoincCode,
            SourceType:         SourceType.Direct,
            OriginalFileName:   originalFileName,
            StoredFileName:     storedFileName,
            FileSizeBytes:      fileSize,
            ContentType:        contentType,
            FileHash:           fileHash,
            CdaXml:             cdaXml,
            Notes:              Notes
        );

        var result = await pipeline.RunAsync(input);
        return RedirectToPage("/Payer/Attachments/Detail", new { id = result.TransactionId });
    }
}
