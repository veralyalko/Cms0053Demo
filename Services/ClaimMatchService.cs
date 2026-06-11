using Cms0053Demo.Data;
using Cms0053Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Services;

public record ClaimMatchResult(bool Matched, string ClaimNumber, string PatientName,
    decimal ClaimAmount, string ProviderName, string MatchMethod);

public class ClaimMatchService(AppDbContext db)
{
    public async Task<ClaimMatchResult> MatchAsync(string claimNumber, string providerNpi)
    {
        var scenario = await db.DemoScenarios
            .FirstOrDefaultAsync(s =>
                s.ClaimNumber == claimNumber &&
                s.ProviderNPI == providerNpi);

        if (scenario is not null)
            return new ClaimMatchResult(
                true,
                scenario.ClaimNumber,
                scenario.PatientName,
                scenario.ClaimAmount,
                scenario.ProviderName,
                "X12 TRN02 claim number + NM1*41 NPI exact match");

        // Fallback: match by NPI only (simulates partial match)
        var byNpi = await db.DemoScenarios
            .FirstOrDefaultAsync(s => s.ProviderNPI == providerNpi);

        if (byNpi is not null)
            return new ClaimMatchResult(
                true, byNpi.ClaimNumber, byNpi.PatientName, byNpi.ClaimAmount,
                byNpi.ProviderName, "NPI match (claim number not found — partial match)");

        return new ClaimMatchResult(false, claimNumber, "", 0, "", "No matching claim found");
    }
}
