using System.Security.Cryptography;
using System.Text;

namespace Cms0053Demo.Services;

// Real SHA-256 hash chain. Each event hashes the concatenation of the previous
// hash and the current event data, forming a tamper-evident chain.
public class AuditHashService
{
    public const string GenesisHash = "0000000000000000000000000000000000000000000000000000000000000000";

    public string ComputeHash(string previousHash, string eventType, string description, DateTime occurredAt)
    {
        var data = $"{previousHash}|{eventType}|{description}|{occurredAt:O}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(bytes).ToLower();
    }

    public bool VerifyChain(IEnumerable<(string EventType, string Description, DateTime OccurredAt, string EventHash, string PreviousHash)> events)
    {
        var list = events.ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var expected = ComputeHash(list[i].PreviousHash, list[i].EventType, list[i].Description, list[i].OccurredAt);
            if (!string.Equals(expected, list[i].EventHash, StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }
}
