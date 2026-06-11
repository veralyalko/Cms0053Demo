namespace Cms0053Demo.Services;

public record SchematronResult(bool Passed, List<string> Errors, List<string> Warnings, int RulesEvaluated);

// Simulated Schematron evaluation. Returns realistic output consistent with each scenario.
// In production, replace with a real Schematron processor (e.g. Saxon-HE via IKVM or a
// dedicated XProc pipeline).
public class SchematronEvaluator
{
    public SchematronResult Evaluate(string loincCode, bool isDeliberateFailureDoc)
    {
        // Scenario B bad doc never reaches this stage — C-CDA template validation (stage 3) blocks it.
        // This service is only called for passing documents.
        var warnings = new List<string>();

        switch (loincCode)
        {
            case "18842-5": // Discharge Summary
                warnings.Add("CONF:1198-8460: Plan of Treatment section is recommended for discharge summaries but not present");
                return new SchematronResult(true, [], warnings, 47);

            case "11504-8": // Operative Note (corrected Scenario B)
                return new SchematronResult(true, [], warnings, 39);

            case "34117-2": // H&P
                warnings.Add("CONF:1198-2138: Social History section is recommended for H&P documents but not present");
                warnings.Add("CONF:1198-9503: Review of Systems section is recommended but not present");
                return new SchematronResult(true, [], warnings, 52);

            default:
                return new SchematronResult(true, [], warnings, 30);
        }
    }
}
