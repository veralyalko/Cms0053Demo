using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Services;

public record ClaimMatchResult(
    bool Matched,
    int? ClaimId,
    string ClaimNumber,
    string PatientName,
    decimal ClaimAmount,
    string ProviderName,
    string MatchMethod);

public class ClaimMatchService(AppDbContext db)
{
    // [X12-837-LINKAGE-PLACEHOLDER] — supplement with X12 837 ICN for exact linkage in production
    public async Task<ClaimMatchResult> MatchAsync(string claimNumber, string providerNpi)
    {
        var claim = await db.Claims.FirstOrDefaultAsync(c =>
            c.ClaimNumber == claimNumber && c.ProviderNPI == providerNpi);

        if (claim is not null)
            return new ClaimMatchResult(true, claim.Id, claim.ClaimNumber,
                claim.PatientName, claim.AmountBilled, claim.ProviderName,
                "X12 TRN02 claim number + NM1*41 NPI exact match");

        // Partial match by NPI — flags for manual review in production
        var byNpi = await db.Claims.FirstOrDefaultAsync(c => c.ProviderNPI == providerNpi);
        if (byNpi is not null)
            return new ClaimMatchResult(true, byNpi.Id, byNpi.ClaimNumber,
                byNpi.PatientName, byNpi.AmountBilled, byNpi.ProviderName,
                "NPI match — claim number not found, partial match flagged for review");

        return new ClaimMatchResult(false, null, claimNumber, "", 0, "",
            "No matching claim — manual review required");
    }
}
