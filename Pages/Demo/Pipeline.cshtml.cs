using System.Text.Json;
using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Demo;

public class PipelineModel(AppDbContext db, ValidationPipelineService pipeline) : PageModel
{
    public AttachmentTransaction? Transaction { get; set; }
    public DemoScenario? Scenario { get; set; }
    public List<StageDisplayItem> StageDisplay { get; set; } = [];
    public int TotalMs => StageDisplay.Where(s => s.Ran).Sum(s => s.Result!.DurationMs);

    private static readonly string[] StageNames =
    [
        StageName.X12Parse, StageName.CdaSchema, StageName.CcdaTemplate,
        StageName.Schematron, StageName.LoincClassify, StageName.XmlSig,
        StageName.ClaimMatch, StageName.AuditHash
    ];

    public async Task<IActionResult> OnGetAsync(int? scenarioId, int? transactionId)
    {
        if (scenarioId.HasValue)
        {
            var result = await pipeline.RunAsync(scenarioId.Value);
            return RedirectToPage(new { transactionId = result.TransactionId });
        }

        if (!transactionId.HasValue) return Page();

        Transaction = await db.AttachmentTransactions
            .Include(t => t.Scenario)
            .Include(t => t.StageResults.OrderBy(s => s.StageOrder))
            .FirstOrDefaultAsync(t => t.Id == transactionId.Value);

        if (Transaction is null) return Page();

        Scenario = Transaction.Scenario;

        var dbStages = Transaction.StageResults.ToDictionary(s => s.StageOrder);
        for (var i = 0; i < StageNames.Length; i++)
            StageDisplay.Add(new StageDisplayItem(i + 1, StageNames[i], dbStages.GetValueOrDefault(i + 1)));

        return Page();
    }

    public async Task<IActionResult> OnPostResubmitAsync(int scenarioId)
    {
        var result = await pipeline.RunAsync(scenarioId, isResubmission: true);
        return RedirectToPage(new { transactionId = result.TransactionId });
    }

    public static string PrettyJson(string? json)
    {
        if (string.IsNullOrEmpty(json)) return "{}";
        try
        {
            var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch { return json; }
    }
}

public record StageDisplayItem(int Order, string Name, ValidationStageResult? Result)
{
    public bool Ran         => Result is not null;
    public bool Passed      => Result?.Passed ?? false;
    public string RowClass  => !Ran ? "text-muted" : Passed ? "table-success" : "table-danger";
    public string BadgeClass => !Ran ? "bg-secondary" : Passed ? "bg-success" : "bg-danger";
    public string StatusLabel => !Ran ? "—" : Passed ? "Pass" : "Failed";
    public string Duration  => Ran ? $"{Result!.DurationMs}ms" : "—";
    public string ErrorText
    {
        get
        {
            if (Result is null || Result.Passed) return "";
            try
            {
                using var doc = JsonDocument.Parse(Result.Detail);
                if (doc.RootElement.TryGetProperty("Errors", out var arr)
                    && arr.ValueKind == JsonValueKind.Array)
                {
                    var msgs = arr.EnumerateArray()
                        .Select(e => e.GetString())
                        .Where(s => !string.IsNullOrEmpty(s));
                    var joined = string.Join("; ", msgs);
                    if (!string.IsNullOrEmpty(joined)) return joined;
                }
            }
            catch { }
            return "Validation failed — see detail below";
        }
    }
}
