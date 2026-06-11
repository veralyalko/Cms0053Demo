namespace Cms0053Demo.Services;

public class X12BuilderService
{
    private static int _counter = 1;

    public string Build275(
        string providerNpi, string providerName,
        string patientName, DateOnly patientDob,
        string claimNumber, string trackingNumber)
    {
        var ctrl = System.Threading.Interlocked.Increment(ref _counter).ToString().PadLeft(9, '0');
        var now = DateTime.UtcNow;
        var date = now.ToString("yyMMdd");
        var fullDate = now.ToString("yyyyMMdd");
        var time = now.ToString("HHmm");

        // ISA06 sender ID: 15 chars, space-padded
        var raw = providerName.Replace(" ", "").ToUpperInvariant();
        var senderId = (raw.Length >= 15 ? raw[..15] : raw.PadRight(15));

        // Patient name split
        var parts = patientName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var lastName  = (parts.Length > 0 ? parts[0] : patientName).ToUpperInvariant();
        var firstName = (parts.Length > 1 ? parts[1] : "").ToUpperInvariant();

        var dob = patientDob.ToString("yyyyMMdd");
        var provUpper = providerName.ToUpperInvariant();
        if (provUpper.Length > 35) provUpper = provUpper[..35];

        // Member ID suffix from claim number (last 6 chars)
        var memberSuffix = claimNumber.Length >= 6 ? claimNumber[^6..] : claimNumber;

        return
$"ISA*00*          *00*          *ZZ*{senderId}*ZZ*MERIDIANHLTHPLN*{date}*{time}*^*00501*{ctrl}*0*P*:~\n" +
$"GS*HI*{senderId.Trim()}*MERIDIANHLTHPLN*{fullDate}*{time}*{ctrl}*X*006020X314~\n" +
$"ST*275*0001~\n" +
$"BHT*0010*11*{trackingNumber}*{fullDate}*{time}*HC~\n" +
$"NM1*41*2*{provUpper}*****XX*{providerNpi}~\n" +
$"NM1*PR*2*MERIDIAN HEALTH PLAN*****PI*MHP-001~\n" +
$"TRN*1*{claimNumber}*{providerNpi}~\n" +
$"LS*2000~\n" +
$"NM1*QC*1*{lastName}*{firstName}****MI*MHP-{memberSuffix}~\n" +
$"DMG*D8*{dob}*~\n" +
$"LE*2000~\n" +
$"SE*10*0001~\n" +
$"GE*1*{ctrl}~\n" +
$"IEA*1*{ctrl}~";
    }
}
