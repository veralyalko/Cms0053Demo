using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<DemoScenario> Scenarios { get; set; } = [];
    public int TransactionCount { get; set; }

    public async Task OnGetAsync()
    {
        Scenarios = await db.DemoScenarios.OrderBy(s => s.Code).ToListAsync();
        TransactionCount = await db.AttachmentTransactions.CountAsync();
    }

    public IActionResult OnPost(int scenarioId)
    {
        return RedirectToPage("/Demo/Pipeline", new { scenarioId });
    }

    public async Task<IActionResult> OnPostResetAsync()
    {
        await db.AuditEvents.ExecuteDeleteAsync();
        await db.ValidationStageResults.ExecuteDeleteAsync();
        await db.AttachmentTransactions.ExecuteDeleteAsync();
        return RedirectToPage();
    }
}
