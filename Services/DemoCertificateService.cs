using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Cms0053Demo.Services;

// Singleton. Creates a self-signed demo cert once per process lifetime.
// All CDA documents are signed with this key; XmlSigVerifier verifies against it.
public class DemoCertificateService
{
    public X509Certificate2 Certificate { get; }

    public DemoCertificateService()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest(
            "CN=Demo Healthcare CA - TEST CERTIFICATE NOT FOR PRODUCTION, O=Meridian Health Plan Demo, C=US",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        req.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));
        var raw = req.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddYears(1));
        // Re-import so the private key is bound to the cert object on all platforms
        Certificate = X509CertificateLoader.LoadPkcs12(
            raw.Export(X509ContentType.Pfx),
            password: (string?)null,
            keyStorageFlags: X509KeyStorageFlags.Exportable);
    }

    public string Thumbprint  => Certificate.Thumbprint;
    public string Subject     => Certificate.Subject;
    public string NotAfter    => Certificate.NotAfter.ToString("yyyy-MM-dd");
    public string Algorithm   => "RSA-2048 / SHA-256";
    public string Issuer      => Certificate.Issuer;
}
