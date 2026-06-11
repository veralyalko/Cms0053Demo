using Cms0053Demo.Models;

namespace Cms0053Demo.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.DemoScenarios.Any()) return;

        db.DemoScenarios.AddRange(
            new DemoScenario
            {
                Code          = "A",
                Title         = "Cardiac Inpatient — Happy Path",
                Description   = "Discharge summary for a cardiac inpatient admission. All validation stages pass. Demonstrates the full happy-path lifecycle from X12 receipt to claim adjudication in under 4 hours.",
                PatientName   = "Harold Whitfield",
                PatientDOB    = new DateOnly(1948, 3, 22),
                ProviderNPI   = "1234567890",
                ProviderName  = "Lakewood Medical Center",
                ClaimNumber   = "CLM-A-2026-0047",
                ClaimAmount   = 18400.00m,
                DocumentType  = "Discharge Summary",
                LoincCode     = "18842-5",
                ScenarioType  = "HappyPath",
                ClinicianName = "Dr. Eleanor Marsh, MD"
            },
            new DemoScenario
            {
                Code          = "B",
                Title         = "Obstetric Outpatient — Failure & Recovery",
                Description   = "Operative report for an obstetric procedure. The submitted C-CDA document is missing a required section. Pipeline fails at stage 3, the error is surfaced, a corrected document is resubmitted, and the claim recovers.",
                PatientName   = "Sofia Reyes",
                PatientDOB    = new DateOnly(1991, 7, 14),
                ProviderNPI   = "9876543210",
                ProviderName  = "Riverside Women's Health",
                ClaimNumber   = "CLM-B-2026-0112",
                ClaimAmount   = 4750.00m,
                DocumentType  = "Surgical Operation Note",
                LoincCode     = "11504-8",
                ScenarioType  = "DeliberateFailure",
                ClinicianName = "Dr. Carmen Reyes, MD"
            },
            new DemoScenario
            {
                Code          = "C",
                Title         = "Orthopedic — UM Prior Auth Review",
                Description   = "History and physical for an orthopedic procedure requiring prior authorization. All validation stages pass. Attachment routes to the UM vendor for authorization review before claim adjudication.",
                PatientName   = "Douglas Park",
                PatientDOB    = new DateOnly(1965, 11, 9),
                ProviderNPI   = "5551234567",
                ProviderName  = "Summit Orthopedic Group",
                ClaimNumber   = "CLM-C-2026-0088",
                ClaimAmount   = 31200.00m,
                DocumentType  = "History and Physical Note",
                LoincCode     = "34117-2",
                ScenarioType  = "UmReview",
                ClinicianName = "Dr. Thomas Hale, MD"
            }
        );

        db.SaveChanges();
    }
}
