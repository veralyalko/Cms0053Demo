namespace Cms0053Demo.Models;

// Seeded Synthea-generated documents available for selection in provider submission forms.
// Files live under wwwroot/synthea-samples/.
public class EmrDocument
{
    public int Id { get; set; }
    public string DocumentName { get; set; } = "";
    public string DocumentType { get; set; } = "";
    public string LoincCode { get; set; } = "";
    public string PatientName { get; set; } = "";
    public DateOnly PatientDOB { get; set; }
    public string ProviderNPI { get; set; } = "";
    public string ProviderName { get; set; } = "";
    public DateOnly ServiceDate { get; set; }
    public string FileName { get; set; } = "";    // relative path under wwwroot/synthea-samples/
}
