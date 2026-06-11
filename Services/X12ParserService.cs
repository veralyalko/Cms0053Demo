namespace Cms0053Demo.Services;

public record X12ParseResult(
    bool Valid,
    string Error,
    string SenderId,
    string ReceiverId,
    string ControlNumber,
    string TransactionDate,
    string TransactionTime,
    string TransactionType,
    string TrackingNumber,
    string ClaimNumber,
    string ProviderNPI,
    string ProviderName,
    string PatientLastName,
    string PatientFirstName,
    string PatientDob,
    string PatientGender,
    List<X12Segment> Segments
);

public record X12Segment(string Id, string[] Elements);

public class X12ParserService
{
    public X12ParseResult Parse(string x12)
    {
        var segments = new List<X12Segment>();

        if (string.IsNullOrWhiteSpace(x12))
            return Fail("Empty X12 input", segments);

        // Determine element and segment separators from ISA
        var lines = x12.Replace("\r\n", "\n").Split('\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            var raw = line.TrimEnd('~');
            if (string.IsNullOrWhiteSpace(raw)) continue;
            var parts = raw.Split('*');
            segments.Add(new X12Segment(parts[0], parts[1..]));
        }

        if (segments.Count == 0 || segments[0].Id != "ISA")
            return Fail("ISA segment not found — invalid X12 interchange", segments);

        var isa = segments[0].Elements;
        if (isa.Length < 15)
            return Fail("ISA segment has fewer than 16 elements", segments);

        var senderId     = isa[4].Trim();
        var receiverId   = isa[6].Trim();
        var controlNum   = isa[12];
        var txDate       = isa[8];
        var txTime       = isa[9];
        var txType       = "";
        var trackingNum  = "";
        var claimNum     = "";
        var providerNpi  = "";
        var providerName = "";
        var patLast      = "";
        var patFirst     = "";
        var patDob       = "";
        var patGender    = "";

        foreach (var seg in segments)
        {
            switch (seg.Id)
            {
                case "ST" when seg.Elements.Length >= 1:
                    txType = seg.Elements[0];
                    break;
                case "BHT" when seg.Elements.Length >= 3:
                    trackingNum = seg.Elements[2];
                    break;
                case "NM1" when seg.Elements.Length >= 9:
                    if (seg.Elements[0] == "41")
                    {
                        providerName = seg.Elements[2].Trim();
                        providerNpi  = seg.Elements[8].Trim();
                    }
                    break;
                case "TRN" when seg.Elements.Length >= 2:
                    claimNum = seg.Elements[1];
                    break;
                case "NM1" when seg.Elements[0] == "QC" && seg.Elements.Length >= 4:
                    patLast  = seg.Elements[2].Trim();
                    patFirst = seg.Elements[3].Trim();
                    break;
                case "DMG" when seg.Elements.Length >= 3:
                    patDob    = seg.Elements[1];
                    patGender = seg.Elements[2];
                    break;
            }
        }

        if (txType != "275")
            return Fail($"Expected ST01=275, got '{txType}' — not a valid X12 275 transaction", segments);

        return new X12ParseResult(
            Valid: true, Error: "",
            SenderId: senderId, ReceiverId: receiverId,
            ControlNumber: controlNum,
            TransactionDate: txDate, TransactionTime: txTime,
            TransactionType: txType, TrackingNumber: trackingNum,
            ClaimNumber: claimNum, ProviderNPI: providerNpi, ProviderName: providerName,
            PatientLastName: patLast, PatientFirstName: patFirst,
            PatientDob: patDob, PatientGender: patGender,
            Segments: segments);
    }

    private static X12ParseResult Fail(string error, List<X12Segment> segments) =>
        new(false, error, "", "", "", "", "", "", "", "", "", "", "", "", "", "", segments);
}
