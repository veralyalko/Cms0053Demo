using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Cms0053Demo.Services;

public record XmlSigResult(bool Valid, string Error, string CertSubject, string CertThumbprint,
    string SignatureAlgorithm, bool SignaturePresent);

public class XmlSigVerifier(DemoCertificateService certService)
{
    // Signs a CDA XML document using the demo certificate's private key.
    // Returns the signed XML string with an enveloped Signature element appended.
    public string SignDocument(string cdaXml)
    {
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.LoadXml(cdaXml);

        var signedXml = new SignedXml(doc);
        signedXml.SigningKey = certService.Certificate.GetRSAPrivateKey();

        var reference = new Reference { Uri = "" };
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(reference);

        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new KeyInfoX509Data(certService.Certificate));
        signedXml.KeyInfo = keyInfo;

        signedXml.ComputeSignature();
        var sigElement = signedXml.GetXml();
        doc.DocumentElement!.AppendChild(doc.ImportNode(sigElement, true));

        return doc.OuterXml;
    }

    // Verifies the enveloped XMLDSig signature against the demo certificate's public key.
    public XmlSigResult Verify(string signedXml)
    {
        var cert = certService.Certificate;

        XmlDocument doc;
        try
        {
            doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(signedXml);
        }
        catch (XmlException ex)
        {
            return Fail($"XML parse error: {ex.Message}", cert);
        }

        var signatureNodes = doc.GetElementsByTagName("Signature");
        if (signatureNodes.Count == 0)
            return new XmlSigResult(false, "No <Signature> element found in document",
                cert.Subject, cert.Thumbprint, "RSA-SHA256", false);

        try
        {
            var signed = new SignedXml(doc);
            signed.LoadXml((XmlElement)signatureNodes[0]!);

            var valid = signed.CheckSignature(cert.GetRSAPublicKey()!);

            return new XmlSigResult(valid,
                valid ? "" : "Signature verification failed — document may have been tampered with",
                cert.Subject, cert.Thumbprint, "RSA-SHA256 / http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
                true);
        }
        catch (Exception ex)
        {
            return Fail($"Signature check error: {ex.Message}", cert);
        }
    }

    private static XmlSigResult Fail(string error, X509Certificate2 cert) =>
        new(false, error, cert.Subject, cert.Thumbprint, "RSA-SHA256", true);
}
