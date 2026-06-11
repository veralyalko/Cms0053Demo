using System.Xml.Linq;

namespace Cms0053Demo.Services;

public record CcdaTemplateResult(bool Valid, List<string> Errors, List<string> Warnings,
    string TemplateName, string TemplateOid);

public class CcdaTemplateValidator
{
    private const string Hl7Ns = "urn:hl7-org:v3";

    // Required sections per document type: LOINC code → display name
    private static readonly Dictionary<string, (string Name, string[] RequiredSections)> Templates = new()
    {
        ["18842-5"] = ("Discharge Summary (C-CDA R2.1)", ["10154-3", "8648-8", "11535-2", "75310-3"]),
        ["11504-8"] = ("Operative Note (C-CDA R2.1)",   ["29554-3", "10219-4"]),
        ["34117-2"] = ("History and Physical (C-CDA R2.1)", ["10164-2", "8716-3"]),
    };

    private static readonly Dictionary<string, string> SectionNames = new()
    {
        ["10154-3"] = "Chief Complaint",
        ["8648-8"]  = "Hospital Course",
        ["11535-2"] = "Discharge Diagnoses",
        ["75310-3"] = "Discharge Medications",
        ["29554-3"] = "Procedure Indications",
        ["10219-4"] = "Procedure Description",
        ["10164-2"] = "History of Present Illness",
        ["8716-3"]  = "Vital Signs",
    };

    public CcdaTemplateResult Validate(string cdaXml)
    {
        var errors   = new List<string>();
        var warnings = new List<string>();

        XDocument doc;
        try { doc = XDocument.Parse(cdaXml); }
        catch { return Fail(["XML parse error — cannot evaluate C-CDA templates"]); }

        XNamespace ns = Hl7Ns;
        var root = doc.Root!;

        // Identify document type from code element
        var loincCode = root.Element(ns + "code")?.Attribute("code")?.Value ?? "";

        if (!Templates.TryGetValue(loincCode, out var template))
        {
            warnings.Add($"No C-CDA template rules defined for LOINC {loincCode} — skipping template validation");
            return new CcdaTemplateResult(true, errors, warnings, "Unknown", "");
        }

        var (templateName, required) = template;

        // recordTarget is mandatory in every CDA R2 document — its absence is the Scenario B deliberate failure
        if (root.Element(ns + "recordTarget") is null)
            errors.Add("recordTarget element is missing — patient identity cannot be established for claim linkage");

        // Collect all section LOINC codes present in the document
        var presentSections = doc.Descendants(ns + "section")
            .Select(s => s.Element(ns + "code")?.Attribute("code")?.Value)
            .Where(c => c is not null)
            .ToHashSet()!;

        // Check each required section
        foreach (var code in required)
        {
            if (!presentSections.Contains(code))
            {
                var name = SectionNames.TryGetValue(code, out var n) ? n : code;
                errors.Add($"Required section '{name}' (LOINC {code}) is missing from {templateName}");
            }
        }

        // Check that templateId matches expected OID for this document type
        var templateIds = doc.Descendants(ns + "templateId")
            .Select(t => t.Attribute("root")?.Value)
            .ToHashSet();

        if (!templateIds.Any(t => t is not null && t.StartsWith("2.16.840.1.113883.10.20.22.1")))
            warnings.Add("No recognized C-CDA US Realm templateId found");

        var oid = loincCode switch
        {
            "18842-5" => "2.16.840.1.113883.10.20.22.1.8",
            "11504-8" => "2.16.840.1.113883.10.20.22.1.7",
            "34117-2" => "2.16.840.1.113883.10.20.22.1.3",
            _         => ""
        };

        return new CcdaTemplateResult(errors.Count == 0, errors, warnings, templateName, oid);
    }

    private static CcdaTemplateResult Fail(List<string> errors) =>
        new(false, errors, [], "Unknown", "");
}
