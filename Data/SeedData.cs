using Cms0053Demo.Models;
using Microsoft.AspNetCore.Hosting;

namespace Cms0053Demo.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db, IWebHostEnvironment env)
    {
        if (db.Claims.Any()) return;

        // ── Claims (payer's existing open claims awaiting attachments) ───────────
        db.Claims.AddRange(
            new Claim
            {
                ClaimNumber          = "CLM-A-2026-0047",
                PatientName          = "James Harrison",
                PatientDOB           = new DateOnly(1948, 3, 22),
                ProviderNPI          = "1234567890",
                ProviderName         = "Lakewood Medical Center",
                ServiceDate          = new DateOnly(2026, 4, 15),
                DiagnosisCode        = "I21.19",
                DiagnosisDescription = "Inferior ST-elevation myocardial infarction",
                AmountBilled         = 48_250.00m,
                Status               = ClaimStatus.Open,
            },
            new Claim
            {
                ClaimNumber          = "CLM-B-2026-0112",
                PatientName          = "Sofia Reyes",
                PatientDOB           = new DateOnly(1991, 7, 14),
                ProviderNPI          = "9876543210",
                ProviderName         = "Riverside Women's Health",
                ServiceDate          = new DateOnly(2026, 3, 22),
                DiagnosisCode        = "O82",
                DiagnosisDescription = "Encounter for cesarean delivery without indication",
                AmountBilled         = 4_750.00m,
                Status               = ClaimStatus.Open,
            },
            new Claim
            {
                ClaimNumber          = "CLM-C-2026-0088",
                PatientName          = "Douglas Park",
                PatientDOB           = new DateOnly(1965, 11, 9),
                ProviderNPI          = "5551234567",
                ProviderName         = "Summit Orthopedic Group",
                ServiceDate          = new DateOnly(2026, 5, 1),
                DiagnosisCode        = "M17.11",
                DiagnosisDescription = "Primary osteoarthritis, right knee",
                AmountBilled         = 32_800.00m,
                Status               = ClaimStatus.Open,
            },
            new Claim
            {
                ClaimNumber          = "CLM-D-2026-0155",
                PatientName          = "Patricia Williams",
                PatientDOB           = new DateOnly(1945, 9, 30),
                ProviderNPI          = "4441234567",
                ProviderName         = "Valley General Hospital",
                ServiceDate          = new DateOnly(2026, 4, 28),
                DiagnosisCode        = "J18.9",
                DiagnosisDescription = "Pneumonia, unspecified organism",
                AmountBilled         = 18_500.00m,
                Status               = ClaimStatus.Open,
            },
            new Claim
            {
                ClaimNumber          = "CLM-E-2026-0203",
                PatientName          = "Michael Torres",
                PatientDOB           = new DateOnly(1962, 4, 18),
                ProviderNPI          = "3331234567",
                ProviderName         = "Metro Gastro Associates",
                ServiceDate          = new DateOnly(2026, 5, 10),
                DiagnosisCode        = "Z12.11",
                DiagnosisDescription = "Encounter for screening for malignant neoplasm of colon",
                AmountBilled         = 3_200.00m,
                Status               = ClaimStatus.Open,
            }
        );

        // ── Synthea EMR Documents (Synthea-generated C-CDA files in wwwroot/synthea-samples/) ──
        db.EmrDocuments.AddRange(
            new EmrDocument
            {
                DocumentName = "Discharge Summary — James Harrison (04/15/2026)",
                DocumentType = "Discharge Summary",
                LoincCode    = "18842-5",
                PatientName  = "James Harrison",
                PatientDOB   = new DateOnly(1948, 3, 22),
                ProviderNPI  = "1234567890",
                ProviderName = "Lakewood Medical Center",
                ServiceDate  = new DateOnly(2026, 4, 15),
                FileName     = "harrison-james-discharge.xml",
            },
            new EmrDocument
            {
                DocumentName = "Operative Note — Sofia Reyes (03/22/2026)",
                DocumentType = "Operative Note",
                LoincCode    = "11504-8",
                PatientName  = "Sofia Reyes",
                PatientDOB   = new DateOnly(1991, 7, 14),
                ProviderNPI  = "9876543210",
                ProviderName = "Riverside Women's Health",
                ServiceDate  = new DateOnly(2026, 3, 22),
                FileName     = "reyes-sofia-operative.xml",
            },
            new EmrDocument
            {
                DocumentName = "History and Physical — Douglas Park (05/01/2026)",
                DocumentType = "History and Physical Note",
                LoincCode    = "34117-2",
                PatientName  = "Douglas Park",
                PatientDOB   = new DateOnly(1965, 11, 9),
                ProviderNPI  = "5551234567",
                ProviderName = "Summit Orthopedic Group",
                ServiceDate  = new DateOnly(2026, 5, 1),
                FileName     = "park-douglas-hp.xml",
            },
            new EmrDocument
            {
                DocumentName = "Discharge Summary — Patricia Williams (04/28/2026)",
                DocumentType = "Discharge Summary",
                LoincCode    = "18842-5",
                PatientName  = "Patricia Williams",
                PatientDOB   = new DateOnly(1945, 9, 30),
                ProviderNPI  = "4441234567",
                ProviderName = "Valley General Hospital",
                ServiceDate  = new DateOnly(2026, 4, 28),
                FileName     = "williams-patricia-discharge.xml",
            },
            new EmrDocument
            {
                DocumentName = "Operative Note — Michael Torres (05/10/2026)",
                DocumentType = "Operative Note",
                LoincCode    = "11504-8",
                PatientName  = "Michael Torres",
                PatientDOB   = new DateOnly(1962, 4, 18),
                ProviderNPI  = "3331234567",
                ProviderName = "Metro Gastro Associates",
                ServiceDate  = new DateOnly(2026, 5, 10),
                FileName     = "torres-michael-operative.xml",
            }
        );

        db.SaveChanges();
    }
}
