# Cms0053Demo

**"From Provider to Payment in 12 Minutes"**

A sales-grade demonstration platform for CMS-0053-F compliant claim attachment workflows. Built for a mixed business and technical executive audience, including senior payer-side EDI architects.

The demo tells one end-to-end story: a provider submits a CMS-0053-F compliant attachment, the payer's platform receives it, parses the X12 275 envelope, validates the CDA document, classifies it by LOINC code, verifies the digital signature, matches it to a claim, and routes it to adjudication — with a full cryptographic audit trail. Every step is visible.

All data is synthetic. No real PHI is used.

---

## What's Real (Not Simulated)

A senior EDI architect will scrutinize these. All are genuine implementations, not mocks.

| Component | Implementation |
|---|---|
| X12 275 envelope parsing | Raw ISA/GS/ST/BHT/NM1/TRN/LS/LE/SE/GE/IEA segment parsing |
| CDA R2 schema validation | `System.Xml` + HL7 CDA R2 XSD |
| C-CDA template validation | Rules engine — required sections, OIDs, LOINC codes |
| XMLDSig signature verification | `System.Security.Cryptography.Xml.SignedXml` — RSA-2048/SHA-256 |
| LOINC classification | Embedded 12-code lookup table |
| Audit hash chain | `SHA256` — each event hashes previous event, blockchain-style |
| Self-signed X.509 demo cert | `CertificateRequest` + `X509CertificateLoader.LoadPkcs12` |

---

## Three Demo Scenarios

### Scenario A — Cardiac Inpatient (Happy Path)
- Patient: Harold Whitfield · Provider: Lakewood Medical Center (NPI 1234567890)
- Claim: CLM-A-2026-0047, $18,400 · LOINC 18842-5 (Discharge summary)
- Result: All 8 validation stages pass → auto-adjudication → ERA 835

### Scenario B — Obstetric Outpatient (Deliberate Failure + Recovery)
- Patient: Sofia Reyes · Provider: Riverside Women's Health (NPI 9876543210)
- Claim: CLM-B-2026-0112, $4,750 · LOINC 11504-8 (Operative report)
- Result: Fails at Stage 3 (missing C-CDA section 10219-4) → structured error returned → resubmit with corrected document → all stages pass

### Scenario C — Orthopedic with UM Review
- Patient: Douglas Park · Provider: Summit Orthopedic Group (NPI 5551234567)
- Claim: CLM-C-2026-0088, $31,200 · LOINC 34117-2 (History and physical)
- Result: All stages pass → prior auth trigger → routes to PriorPath UM Solutions

---

## Seven Demo Screens

| # | Screen | Description |
|---|---|---|
| 1 | Today's Reality | Pain state — 21-day cycle time, $3.60/transaction, 35% still faxed |
| 2 | Compliance Dashboard | CMS-0053-F mandate timeline (Jan 2027/2028), gap assessment, readiness score |
| 3 | Trading Partner Gateway | Live X12 275 receipt — raw ISA/GS/ST terminal + parsed fields panel |
| 4 | Validation Pipeline | 8-stage animated pipeline — pass/fail per stage with JSON detail |
| 5 | Document Repository | LOINC-indexed store — signed CDA viewer, XMLDSig cert card |
| 6 | Claims + UM Handoff | Routing to CoreClaim adjudication (Scenario A) or PriorPath UM (Scenario C) |
| 7 | Audit Trail | SHA-256 cryptographic hash chain — every event, tamper-evident |

Screens 1–2 require no scenario. Screens 3–7 are activated by running a scenario.

---

## 8-Stage Validation Pipeline

```
1. X12 Envelope Parse       — ISA/GS/ST/BHT/NM1/TRN/DMG segments
2. CDA Schema Validation    — HL7 CDA R2 XSD
3. C-CDA Template           — Required sections, OIDs, LOINC codes   ← Scenario B fails here
4. Schematron Evaluation    — Business rule evaluation
5. LOINC Classification     — Document type lookup (12-code subset)
6. XMLDSig Verification     — RSA-2048/SHA-256 enveloped signature
7. Claim Match              — ICN/TCN linkage via TRN02 + NM1*41 NPI
8. Audit Hash Chain         — SHA-256 append to tamper-evident log
```

All stages complete in under 1,600 ms. Scenario B halts at Stage 3 with a structured error; the Resubmit flow demonstrates recovery.

---

## Presenter Console

`/Presenter/Console` — a separate dark-themed page for the sales engineer running the demo.

- Talk track for each of the 7 screens
- Top 3 anticipated questions with prepared answers
- Scenario selector with last tracking number and status
- 12-minute countdown timer (sessionStorage-persisted across screen navigation)
- Quick-open links for every screen (new tab)
- 12-minute flow guide with timestamps

Not linked from the main navigation. Open it in a separate window during the presentation.

---

## Running Locally

**Prerequisites:** .NET 10 SDK

```bash
git clone https://github.com/veralyalko/Cms0053Demo.git
cd Cms0053Demo
dotnet run
```

Open `http://localhost:5286`. The database is seeded automatically on first run.

**Before each presentation**, use the Reset Demo button on the home page to clear previous runs.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10, Razor Pages |
| ORM | Entity Framework Core 10 + SQLite |
| UI | Bootstrap 5 |
| X12 parsing | Raw segment parsing (no third-party library) |
| XML validation | `System.Xml` + embedded CDA R2 XSD |
| Digital signatures | `System.Security.Cryptography.Xml` |
| Hashing | `System.Security.Cryptography.SHA256` |

---

## Synthetic Entities

| Role | Name | ID |
|---|---|---|
| Payer | Meridian Health Plan | MHP-001 |
| Clearinghouse | Veridian Health Exchange | ISA06: 123456789 |
| Claims Platform | CoreClaim Adjudication | — |
| UM Vendor | PriorPath UM Solutions | — |

All patient names, NPIs, claim numbers, and provider names are entirely fictional.

---

## CMS-0053-F Context

CMS-0053-F requires covered health plans to support electronic claim attachments using X12 275. Large plans must comply by January 2027; all plans by January 2028.

This demo represents a payer-side implementation of the receiving end of that workflow. For a production deployment, the simulated components (trading partner enrollment, clearinghouse connectivity, claims platform API, UM vendor routing) would be replaced with real integrations. The real components demonstrated here — X12 parsing, CDA validation, digital signature verification, cryptographic audit chain — are production-grade implementations that would carry over directly.
