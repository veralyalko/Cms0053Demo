namespace Cms0053Demo.Services;

public record LoincResult(bool Found, string Code, string DisplayName, string Category, string Component);

public class LoincClassifier
{
    private static readonly Dictionary<string, (string Display, string Category, string Component)> Codes = new()
    {
        ["18842-5"] = ("Discharge summary",              "Clinical note",   "Discharge summary"),
        ["11504-8"] = ("Surgical operation note",        "Clinical note",   "Operation note"),
        ["11526-1"] = ("Pathology study",                "Clinical note",   "Pathology report"),
        ["18748-4"] = ("Diagnostic imaging study",       "Clinical note",   "Radiology report"),
        ["11506-3"] = ("Progress note",                  "Clinical note",   "Progress note"),
        ["28570-0"] = ("Procedure note",                 "Clinical note",   "Procedure note"),
        ["34117-2"] = ("History and physical note",      "Clinical note",   "H&P note"),
        ["11488-4"] = ("Consultation note",              "Clinical note",   "Consultation"),
        ["28568-4"] = ("Emergency department note",      "Clinical note",   "ED note"),
        ["57133-1"] = ("Referral note",                  "Clinical note",   "Referral"),
        ["52030-1"] = ("Hospital course narrative",      "Clinical note",   "Hospital course"),
        ["18761-7"] = ("Transfer summary",               "Clinical note",   "Transfer summary"),
    };

    public LoincResult Classify(string loincCode)
    {
        if (Codes.TryGetValue(loincCode, out var entry))
            return new LoincResult(true, loincCode, entry.Display, entry.Category, entry.Component);

        return new LoincResult(false, loincCode, "Unknown document type", "Unclassified", "Unknown");
    }
}
