namespace Cms0053Demo.Models;

public class DemoScenario
{
    public int Id { get; set; }
    public string Code { get; set; } = "";         // "A", "B", "C"
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateOnly PatientDOB { get; set; }
    public string ProviderNPI { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public string ClaimNumber { get; set; } = "";
    public decimal ClaimAmount { get; set; }
    public string DocumentType { get; set; } = "";
    public string LoincCode { get; set; } = "";
    public string ScenarioType { get; set; } = ""; // HappyPath / DeliberateFailure / UmReview
    public string ClinicianName { get; set; } = "";

    public ICollection<AttachmentTransaction> Transactions { get; set; } = [];
}
