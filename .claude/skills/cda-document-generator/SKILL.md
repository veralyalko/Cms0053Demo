---
name: cda-document-generator
description: "Use when creating or modifying HL7 C-CDA documents for demo
  scenarios. Provides validated structure for the required document types
  (discharge summary, operative report, history and physical) ensuring
  conformance to the CDA R2 schema and C-CDA March 2022 IG. References the
  C# implementation in this project."
---

# C-CDA Document Generator

## Implementation
CDA documents in this project are generated as C# string literals inside
`Services/ValidationPipelineService.cs`. They are stored in
`AttachmentTransaction.CdaDocument` (full raw XML).

Validation services:
- `Services/CdaSchemaValidator.cs` — validates against embedded CDA R2 XSD (Stage 2)
- `Services/CcdaTemplateValidator.cs` — checks required sections and OIDs (Stage 3)
- `Services/XmlSigVerifier.cs` — verifies XMLDSig enveloped signature (Stage 6)
- `Services/DemoCertificateService.cs` — singleton holding the self-signed demo cert

## Required Skeleton (Every Document)
```xml
<?xml version="1.0" encoding="UTF-8"?>
<ClinicalDocument xmlns="urn:hl7-org:v3"
                  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

  <realmCode code="US"/>
  <typeId root="2.16.840.1.113883.1.3" extension="POCD_HD000040"/>

  <!-- Document-type-specific templateId (see below) -->
  <templateId root="2.16.840.1.113883.10.20.22.1.1" extension="2015-08-01"/>
  <templateId root="{docTypeTemplateOid}" extension="2015-08-01"/>

  <id root="{uuid}"/>
  <code code="{loincCode}" codeSystem="2.16.840.1.113883.6.1"
        codeSystemName="LOINC" displayName="{loincDisplayName}"/>
  <title>{documentTitle}</title>
  <effectiveTime value="{YYYYMMDDHHmmss+0000}"/>
  <confidentialityCode code="N" codeSystem="2.16.840.1.113883.5.25"/>
  <languageCode code="en-US"/>

  <!-- recordTarget IS REQUIRED — omitting it is the Scenario B deliberate failure -->
  <recordTarget>
    <patientRole>
      <id root="2.16.840.1.113883.4.6" extension="{patientId}"/>
      <patient>
        <name><given>{firstName}</given><family>{lastName}</family></name>
        <administrativeGenderCode code="{M|F}" codeSystem="2.16.840.1.113883.5.1"/>
        <birthTime value="{YYYYMMDD}"/>
      </patient>
    </patientRole>
  </recordTarget>

  <author>
    <time value="{YYYYMMDDHHmmss+0000}"/>
    <assignedAuthor>
      <id root="2.16.840.1.113883.4.6" extension="{NPI}"/>
      <assignedPerson>
        <name><prefix>Dr.</prefix><given>{firstName}</given><family>{lastName}</family></name>
      </assignedPerson>
    </assignedAuthor>
  </author>

  <custodian>
    <assignedCustodian>
      <representedCustodianOrganization>
        <id root="2.16.840.1.113883.4.6" extension="{orgNPI}"/>
        <name>{orgName}</name>
      </representedCustodianOrganization>
    </assignedCustodian>
  </custodian>

  <component>
    <structuredBody>
      <!-- Document-type-specific sections (see below) -->
    </structuredBody>
  </component>

  <!-- XMLDSig Signature element appended by DemoCertificateService -->
</ClinicalDocument>
```

## Document Type Templates and Required Sections

### Discharge Summary (LOINC 18842-5)
- templateId: `2.16.840.1.113883.10.20.22.1.8`
- Required sections:
  - Hospital Course (LOINC 8648-8)
  - Discharge Diagnosis (LOINC 11535-2)
  - Discharge Medications (LOINC 10183-2)
  - Discharge Disposition (LOINC 8650-4)

### Surgical Operation Note (LOINC 11504-8)
- templateId: `2.16.840.1.113883.10.20.22.1.7`
- Required sections:
  - Pre-procedure Diagnosis
  - Post-procedure Diagnosis
  - Procedure Description (LOINC 10219-4) — used in current Stage 3 failure
  - Procedure Findings
  - Anesthesia

### History and Physical (LOINC 34117-2)
- templateId: `2.16.840.1.113883.10.20.22.1.3`
- Required sections:
  - Chief Complaint (LOINC 10154-3)
  - History of Present Illness (LOINC 11348-0)
  - Physical Exam (LOINC 29545-1)
  - Assessment and Plan (LOINC 51847-2)

## Section Template
```xml
<component>
  <section>
    <templateId root="2.16.840.1.113883.10.20.22.2.{sectionOid}"/>
    <code code="{loincCode}" codeSystem="2.16.840.1.113883.6.1"
          codeSystemName="LOINC" displayName="{sectionTitle}"/>
    <title>{sectionTitle}</title>
    <text>{narrative text}</text>
  </section>
</component>
```

## Deliberate Failure (Scenario B)
Per spec, Scenario B must fail by **omitting the `recordTarget` element**.
The current implementation omits a required section instead — this is a known
deviation. To fix: remove the `<recordTarget>` block from the Scenario B CDA
in `ValidationPipelineService.cs` and update `CcdaTemplateValidator.cs` to
check for `recordTarget` presence as a Stage 3 rule.

## XMLDSig Signature
`DemoCertificateService` (Singleton) holds a self-signed RSA-2048 cert generated
at startup. `XmlSigVerifier.Sign(XmlDocument doc)` appends an enveloped
`<Signature>` element. Verification uses `SignedXml.CheckSignature()`.

The signature is appended AFTER document construction. Never embed it manually.
Call `Services/XmlSigVerifier.cs` → `Sign()` to add it.

## Validation Before Persisting
Run the pipeline against any new document via:
```
/Demo/Pipeline?scenarioId={id}
```
Stage 2 (CDA Schema) and Stage 3 (C-CDA Template) will catch structural errors.
Stage 6 (XMLDSig) will catch signature issues.

## PHI Policy
All patient names, DOBs, NPIs, and identifiers must be fictional.
Never use a real NPI. Never copy a name from any real source.
Synthetic NPIs in this project use the format `{digit}{9 more digits}` —
none match real NPPES records.
