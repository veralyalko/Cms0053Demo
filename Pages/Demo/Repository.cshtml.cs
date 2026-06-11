using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Cms0053Demo.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Pages.Demo;

public class RepositoryModel(
    AppDbContext db,
    XmlSigVerifier sigVerifier,
    LoincClassifier loincClassifier,
    DemoCertificateService certService) : PageModel
{
    public AttachmentTransaction? Transaction { get; set; }
    public DemoScenario? Scenario { get; set; }
    public XmlSigResult? SigResult { get; set; }
    public LoincResult? LoincInfo { get; set; }
    public string CdaPreview { get; set; } = "";
    public int CdaTotalLines { get; set; }
    public string CertThumbprintShort => certService.Thumbprint[..16] + "…";
    public string CertValidUntil => certService.NotAfter;
    public string CertAlgorithm => certService.Algorithm;

    public async Task OnGetAsync(int? transactionId)
    {
        if (!transactionId.HasValue) return;

        Transaction = await db.AttachmentTransactions
            .Include(t => t.Scenario)
            .FirstOrDefaultAsync(t => t.Id == transactionId.Value);

        if (Transaction is null) return;

        Scenario = Transaction.Scenario;
        SigResult = sigVerifier.Verify(Transaction.CdaDocument);
        LoincInfo = loincClassifier.Classify(Transaction.LoincCode);

        var lines = Transaction.CdaDocument.Split('\n');
        CdaTotalLines = lines.Length;
        CdaPreview = string.Join('\n', lines.Take(35));
    }
}
