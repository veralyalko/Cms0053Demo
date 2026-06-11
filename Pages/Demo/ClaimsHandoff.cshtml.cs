using System.Text.Json;
using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Demo;

public class ClaimsHandoffModel(AppDbContext db) : PageModel
{
    public AttachmentTransaction? Transaction { get; set; }
    public DemoScenario? Scenario { get; set; }
    public ValidationStageResult? ClaimMatchStage { get; set; }
    public string ClaimMatchDetail { get; set; } = "";
    public ClaimMatchInfo? MatchInfo { get; set; }

    public async Task OnGetAsync(int? transactionId)
    {
        if (!transactionId.HasValue) return;

        Transaction = await db.AttachmentTransactions
            .Include(t => t.Scenario)
            .Include(t => t.StageResults)
            .FirstOrDefaultAsync(t => t.Id == transactionId.Value);

        if (Transaction is null) return;

        Scenario = Transaction.Scenario;
        ClaimMatchStage = Transaction.StageResults.FirstOrDefault(s => s.StageOrder == 7);

        if (ClaimMatchStage is not null)
        {
            try
            {
                var doc = JsonDocument.Parse(ClaimMatchStage.Detail);
                ClaimMatchDetail = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });

                MatchInfo = new ClaimMatchInfo(
                    ClaimNumber: doc.RootElement.GetProperty("ClaimNumber").GetString() ?? "",
                    PatientName: doc.RootElement.GetProperty("PatientName").GetString() ?? "",
                    ClaimAmount: doc.RootElement.GetProperty("ClaimAmount").GetDecimal(),
                    ProviderName: doc.RootElement.GetProperty("ProviderName").GetString() ?? "",
                    MatchMethod: doc.RootElement.GetProperty("MatchMethod").GetString() ?? "");
            }
            catch { ClaimMatchDetail = ClaimMatchStage.Detail; }
        }
    }
}

public record ClaimMatchInfo(string ClaimNumber, string PatientName, decimal ClaimAmount,
    string ProviderName, string MatchMethod);
