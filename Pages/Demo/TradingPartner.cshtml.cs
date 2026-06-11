using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Demo;

public class TradingPartnerModel(AppDbContext db, X12ParserService x12Parser) : PageModel
{
    public AttachmentTransaction? Transaction { get; set; }
    public DemoScenario? Scenario { get; set; }
    public X12ParseResult? X12Result { get; set; }
    public List<X12DisplaySegment> DisplaySegments { get; set; } = [];

    public async Task OnGetAsync(int? transactionId)
    {
        if (!transactionId.HasValue) return;

        Transaction = await db.AttachmentTransactions
            .Include(t => t.Scenario)
            .FirstOrDefaultAsync(t => t.Id == transactionId.Value);

        if (Transaction is null) return;

        Scenario = Transaction.Scenario;
        X12Result = x12Parser.Parse(Transaction.X12Envelope);

        foreach (var seg in X12Result.Segments)
        {
            var cssClass = seg.Id switch
            {
                "ISA" or "IEA"  => "text-info",
                "GS"  or "GE"   => "text-primary",
                "ST"  or "SE"   => "text-success",
                "BHT"           => "text-warning",
                "TRN"           => "text-warning",
                "NM1"           => "text-danger",
                "DMG"           => "text-info",
                "LS"  or "LE"   => "text-secondary",
                _               => "text-light"
            };
            DisplaySegments.Add(new X12DisplaySegment(seg.Id, seg.Elements, cssClass));
        }
    }
}

public record X12DisplaySegment(string Id, string[] Elements, string CssClass);
