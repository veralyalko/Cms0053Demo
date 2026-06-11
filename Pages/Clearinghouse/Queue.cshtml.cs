using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Clearinghouse;

public class QueueModel(AppDbContext db, ValidationPipelineService pipeline,
    FileStorageService fileStorage) : PageModel
{
    public List<ClearinghouseRecord> NewRecords { get; set; } = [];
    public List<ClearinghouseRecord> PulledRecords { get; set; } = [];
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        NewRecords    = await db.ClearinghouseRecords.Where(r => r.Status == ClearinghouseStatus.New)
                                .OrderByDescending(r => r.SubmittedAt).ToListAsync();
        PulledRecords = await db.ClearinghouseRecords.Where(r => r.Status == ClearinghouseStatus.Pulled)
                                .Include(r => r.Transactions)
                                .OrderByDescending(r => r.PulledAt).Take(20).ToListAsync();

        if (TempData.ContainsKey("SuccessTrackingNumber"))
            SuccessMessage = $"Submitted to clearinghouse: {TempData["SuccessTrackingNumber"]}";
    }

    // [X12-275-CLEARINGHOUSE-PLACEHOLDER] — in production, this replaces the DB query
    // with a real clearinghouse API or SFTP retrieval of X12 275 transactions.
    public async Task<IActionResult> OnPostPullAsync(int recordId)
    {
        var record = await db.ClearinghouseRecords.FindAsync(recordId);
        if (record is null || record.Status != ClearinghouseStatus.New)
            return RedirectToPage();

        // Read the stored CDA document
        var cdaXml = fileStorage.ReadText(record.StoredFileName);
        if (string.IsNullOrEmpty(cdaXml))
        {
            TempData["Error"] = $"Could not read document for record {record.TrackingNumber}.";
            return RedirectToPage();
        }

        var input = new PipelineInput(
            ProviderNPI:          record.ProviderNPI,
            ProviderName:         record.ProviderName,
            PatientName:          record.PatientName,
            PatientDOB:           record.PatientDOB,
            ClaimNumber:          record.ClaimNumber,
            ServiceDate:          record.ServiceDate,
            DocumentType:         record.DocumentType,
            LoincCode:            record.LoincCode,
            SourceType:           SourceType.UnsolicitedClearinghouse,
            OriginalFileName:     record.OriginalFileName,
            StoredFileName:       record.StoredFileName,
            FileSizeBytes:        record.FileSizeBytes,
            ContentType:          record.ContentType,
            FileHash:             "",
            CdaXml:               cdaXml,
            Notes:                record.Notes,
            ClearinghouseRecordId: record.Id
        );

        var result = await pipeline.RunAsync(input);
        return RedirectToPage("/Payer/Attachments/Detail", new { id = result.TransactionId });
    }

    public async Task<IActionResult> OnPostPullAllAsync()
    {
        var records = await db.ClearinghouseRecords
            .Where(r => r.Status == ClearinghouseStatus.New)
            .ToListAsync();

        if (!records.Any()) return RedirectToPage();

        int lastTxId = 0;
        foreach (var record in records)
        {
            var cdaXml = fileStorage.ReadText(record.StoredFileName);
            if (string.IsNullOrEmpty(cdaXml)) continue;

            var input = new PipelineInput(
                ProviderNPI:          record.ProviderNPI,
                ProviderName:         record.ProviderName,
                PatientName:          record.PatientName,
                PatientDOB:           record.PatientDOB,
                ClaimNumber:          record.ClaimNumber,
                ServiceDate:          record.ServiceDate,
                DocumentType:         record.DocumentType,
                LoincCode:            record.LoincCode,
                SourceType:           SourceType.UnsolicitedClearinghouse,
                OriginalFileName:     record.OriginalFileName,
                StoredFileName:       record.StoredFileName,
                FileSizeBytes:        record.FileSizeBytes,
                ContentType:          record.ContentType,
                FileHash:             "",
                CdaXml:               cdaXml,
                Notes:                record.Notes,
                ClearinghouseRecordId: record.Id
            );

            var result = await pipeline.RunAsync(input);
            lastTxId = result.TransactionId;
        }

        return lastTxId > 0
            ? RedirectToPage("/Payer/Attachments/Detail", new { id = lastTxId })
            : RedirectToPage("/Payer/Dashboard");
    }
}
