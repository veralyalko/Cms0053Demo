using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Provider;

public class UploadModel(AppDbContext db, ValidationPipelineService pipeline,
    FileStorageService fileStorage) : PageModel
{
    public new AttachmentRequest? Request { get; set; }
    public Claim? Claim { get; set; }
    public List<EmrDocument> EmrDocuments { get; set; } = [];

    [BindProperty] public int? EmrDocumentId { get; set; }
    [BindProperty] public IFormFile? UploadedFile { get; set; }
    [BindProperty] public string Notes { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(string token)
    {
        Request = await db.AttachmentRequests
            .Include(r => r.Claim)
            .FirstOrDefaultAsync(r => r.SecureUploadToken == token && r.Status == RequestStatus.Pending);

        if (Request is null) return NotFound();
        Claim = Request.Claim;

        // Show matching EMR documents for this claim's provider/document type
        EmrDocuments = await db.EmrDocuments
            .Where(d => d.ProviderNPI == Claim.ProviderNPI &&
                        d.DocumentType == Request.DocumentTypeRequested)
            .ToListAsync();

        if (!EmrDocuments.Any())
            EmrDocuments = await db.EmrDocuments.Where(d => d.ProviderNPI == Claim.ProviderNPI).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string token)
    {
        Request = await db.AttachmentRequests
            .Include(r => r.Claim)
            .FirstOrDefaultAsync(r => r.SecureUploadToken == token && r.Status == RequestStatus.Pending);

        if (Request is null) return NotFound();
        Claim = Request.Claim;
        EmrDocuments = await db.EmrDocuments.Where(d => d.ProviderNPI == Claim.ProviderNPI).ToListAsync();

        if (EmrDocumentId is null && UploadedFile is null)
        {
            ModelState.AddModelError("", "Select an EMR document or upload a file.");
            return Page();
        }

        string cdaXml, originalFileName, contentType, storedFileName, fileHash;
        long fileSize;

        if (EmrDocumentId.HasValue)
        {
            var emr = await db.EmrDocuments.FindAsync(EmrDocumentId.Value);
            cdaXml = fileStorage.ReadSyntheaSample(emr!.FileName);
            originalFileName = emr.FileName;
            contentType = "text/xml";
            (storedFileName, fileSize, fileHash) = await fileStorage.SaveTextAsync(cdaXml, ".xml");
        }
        else
        {
            (storedFileName, fileSize, fileHash) = await fileStorage.SaveAsync(UploadedFile!);
            originalFileName = UploadedFile!.FileName;
            contentType = UploadedFile.ContentType;
            cdaXml = fileStorage.ReadText(storedFileName);
        }

        // Resolve document type/LOINC from the EMR doc or the original request
        var docType  = EmrDocumentId.HasValue ? (await db.EmrDocuments.FindAsync(EmrDocumentId.Value))!.DocumentType : Request.DocumentTypeRequested;
        var loincCode = EmrDocumentId.HasValue ? (await db.EmrDocuments.FindAsync(EmrDocumentId.Value))!.LoincCode : "18842-5";

        var input = new PipelineInput(
            ProviderNPI:        Claim.ProviderNPI,
            ProviderName:       Claim.ProviderName,
            PatientName:        Claim.PatientName,
            PatientDOB:         Claim.PatientDOB,
            ClaimNumber:        Claim.ClaimNumber,
            ServiceDate:        Claim.ServiceDate,
            DocumentType:       docType,
            LoincCode:          loincCode,
            SourceType:         SourceType.Solicited,
            OriginalFileName:   originalFileName,
            StoredFileName:     storedFileName,
            FileSizeBytes:      fileSize,
            ContentType:        contentType,
            FileHash:           fileHash,
            CdaXml:             cdaXml,
            Notes:              Notes,
            AttachmentRequestId: Request.Id
        );

        var result = await pipeline.RunAsync(input);
        return RedirectToPage("/Payer/Attachments/Detail", new { id = result.TransactionId });
    }
}
