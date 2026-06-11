using System.Xml;
using System.Xml.Linq;

namespace Cms0053Demo.Services;

public record CdaSchemaResult(bool Valid, string Error, string LoincCode, string LoincDisplay,
    string DocumentId, string PatientId, string AuthorNpi);

public class CdaSchemaValidator
{
    private const string Hl7Ns = "urn:hl7-org:v3";

    public CdaSchemaResult Validate(string cdaXml)
    {
        XDocument doc;
        try { doc = XDocument.Parse(cdaXml); }
        catch (XmlException ex) { return Fail($"XML is not well-formed: {ex.Message}"); }

        XNamespace ns = Hl7Ns;
        var root = doc.Root;

        if (root is null || root.Name.LocalName != "ClinicalDocument")
            return Fail("Root element must be ClinicalDocument");
        if (root.Name.NamespaceName != Hl7Ns)
            return Fail($"Root namespace must be '{Hl7Ns}', found '{root.Name.NamespaceName}'");

        // typeId — must have root=2.16.840.1.113883.1.3
        var typeId = root.Element(ns + "typeId");
        if (typeId is null)
            return Fail("Required element <typeId> is missing");
        if (typeId.Attribute("root")?.Value != "2.16.840.1.113883.1.3")
            return Fail("typeId/@root must be '2.16.840.1.113883.1.3' (CDA R2 identifier)");

        // Required top-level elements
        foreach (var req in new[] { "id", "code", "title", "effectiveTime", "confidentialityCode" })
        {
            if (root.Element(ns + req) is null)
                return Fail($"Required element <{req}> is missing");
        }

        // code must have codeSystem 2.16.840.1.113883.6.1 (LOINC)
        var codeEl = root.Element(ns + "code")!;
        if (codeEl.Attribute("codeSystem")?.Value != "2.16.840.1.113883.6.1")
            return Fail("Document code/@codeSystem must be LOINC OID '2.16.840.1.113883.6.1'");

        var loincCode    = codeEl.Attribute("code")?.Value ?? "";
        var loincDisplay = codeEl.Attribute("displayName")?.Value ?? "";

        // recordTarget > patientRole > patient
        var patient = root.Element(ns + "recordTarget")
                         ?.Element(ns + "patientRole")
                         ?.Element(ns + "patient");
        if (patient is null)
            return Fail("Required path recordTarget/patientRole/patient is missing");

        var patientId = root.Element(ns + "recordTarget")
                           ?.Element(ns + "patientRole")
                           ?.Element(ns + "id")
                           ?.Attribute("extension")?.Value ?? "";

        // author
        if (root.Element(ns + "author") is null)
            return Fail("Required element <author> is missing");

        var authorNpi = root.Element(ns + "author")
                           ?.Element(ns + "assignedAuthor")
                           ?.Element(ns + "id")
                           ?.Attribute("extension")?.Value ?? "";

        // custodian
        if (root.Element(ns + "custodian") is null)
            return Fail("Required element <custodian> is missing");

        // component (body)
        if (root.Element(ns + "component") is null)
            return Fail("Required element <component> (document body) is missing");

        var docId = root.Element(ns + "id")?.Attribute("extension")?.Value ?? "";

        return new CdaSchemaResult(true, "", loincCode, loincDisplay, docId, patientId, authorNpi);
    }

    private static CdaSchemaResult Fail(string error) =>
        new(false, error, "", "", "", "", "");
}
