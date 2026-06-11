# Cms0053Demo

## Project Purpose

This is the CMS-0053 sales demo platform — a technically credible, sales-grade demonstration of a payer-side CMS-0053-F compliant claim attachment workflow. It must be convincing to a mixed business and technical executive audience, including senior payer-side EDI architects. All data is synthetic. No real PHI is used.

The demo tells one story: **"From Provider to Payment in 12 Minutes."** A provider submits a CMS-0053-F compliant attachment. The payer's platform receives it, parses the X12 275 envelope, validates the CDA document, classifies it by LOINC code, verifies the digital signature, matches it to a claim, and routes it to adjudication — with a full cryptographic audit trail. Every step is visible to the audience.

---

## What Must Be Real (Not Simulated)

These are the technical credibility surface. A senior EDI architect will scrutinize these.

| Component | Library | Notes |
|---|---|---|
| X12 275 envelope parsing | `OopFactory.X12` or raw segment parsing | Real ISA/GS/ST/BHT/NM1/TRN/LS/LE/SE/GE/IEA segment parsing |
| CDA R2 schema validation | `System.Xml` + XSD | Validate against HL7 CDA R2 schema |
| C-CDA template validation | Custom rules engine | Check required sections, OIDs, loinc codes |
| XMLDSig signature verification | `System.Security.Cryptography.Xml` | Real signature verification against demo cert |
| LOINC classification | Embedded lookup table | 12-code subset, expandable |
| Audit hash chain | `System.Security.Cryptography.SHA256` | Each event hashes previous event — real chain |

Every other integration (trading partner gateway, claims platform, UM vendor) is simulated but must look real.

---

## CMS-0053-F Alignment

- **X12 275** — Additional Information to Support a Health Care Claim (provider → payer)
- **X12 277** — Claim Request for Additional Information (payer → provider)
- **X12 837** — Original claim (linkage via ICN/TCN)
- **HL7 CDA R2 / C-CDA** — Clinical document format for attachments
- **LOINC** — Document type classification codes

---

## Seven Demo Screens

### Screen 1 — Today's Reality (Pain State)
Shows the current manual, fax-based attachment world. Quantified pain: cycle time, denial rate, FTE cost. Sets up the "before" picture.

### Screen 2 — Compliance Dashboard
Payer-side compliance posture. CMS-0053-F mandate timeline, current gap assessment, readiness score. Business case for the platform.

### Screen 3 — Trading Partner Gateway
Live X12 275 envelope receipt and parsing. Shows real ISA/GS/ST segments. Trading partner authentication. Envelope validation results. This is where the technical audience leans in.

### Screen 4 — Validation Pipeline
The core technical screen. Real-time pipeline visualization showing each stage:
1. X12 envelope parse
2. CDA schema validation
3. C-CDA template validation
4. Schematron rule evaluation
5. LOINC document classification
6. XMLDSig signature verification
7. Claim match (ICN/TCN linkage)
8. Audit hash chain entry

Each stage shows pass/fail, timing, and detail. The deliberate-failure scenario triggers failure at stage 3.

### Screen 5 — Document Repository
All received attachments, searchable by LOINC code, patient, provider NPI, claim number. Document viewer with metadata panel (type, LOINC, cert status, hash).

### Screen 6 — Claims + UM Handoff
Cycle-time timeline visualization. Shows the attachment flowing to the claims platform and (for Scenario C) to the UM vendor. Simulated state machine. Before/after cycle time comparison.

### Screen 7 — Audit Trail
Full cryptographic hash chain. Each event listed with its hash, the previous hash, and a verification status. Separate verify button that re-computes and confirms the chain. Tamper-evidence demonstration.

---

## Presenter Console

A separate page at `/Presenter` for the sales engineer running the demo. Not visible to the customer audience. Shows:
- Current screen indicator
- Talk track for the current screen
- Top 3 anticipated questions with one-liner answers
- Scenario selector (A / B / C)
- Elapsed demo time
- Fallback trigger buttons (one per screen)

---

## Three Demo Scenarios

### Scenario A — Cardiac Inpatient (Happy Path)
- Patient: Harold Whitfield, DOB 1948-03-22
- Provider: Lakewood Medical Center, NPI 1234567890
- Payer: Meridian Health Plan
- Document: Discharge summary, LOINC 18842-5
- Claim: CLM-A-2026-0047, $18,400
- Result: All validation stages pass, claim accepted, 4-hour cycle time

### Scenario B — Obstetric Outpatient (Deliberate Failure + Recovery)
- Patient: Sofia Reyes, DOB 1991-07-14
- Provider: Riverside Women's Health, NPI 9876543210
- Document: Operative report, LOINC 11504-8 — **deliberately malformed C-CDA** (missing required section)
- Result: Pipeline fails at stage 3 (C-CDA template validation), error surfaced, corrected document resubmitted, recovery demonstrated

### Scenario C — Orthopedic with UM Review
- Patient: Douglas Park, DOB 1965-11-09
- Provider: Summit Orthopedic Group, NPI 5551234567
- Document: History and physical, LOINC 34117-2
- Claim: Requires UM prior authorization review
- Result: Passes all validation, routes to UM vendor, authorization returned, claim adjudicated

---

## LOINC Code Subset

| Code | Document Type |
|---|---|
| 18842-5 | Discharge summary |
| 11504-8 | Surgical operation note |
| 11526-1 | Pathology study |
| 18748-4 | Diagnostic imaging study |
| 11506-3 | Progress note |
| 28570-0 | Procedure note |
| 34117-2 | History and physical note |
| 11488-4 | Consultation note |
| 28568-4 | Emergency department note |
| 57133-1 | Referral note |
| 52030-1 | Hospital course narrative |
| 18761-7 | Transfer summary |

---

## Synthetic Entities (Fixed Across All Scenarios)

| Role | Name | ID |
|---|---|---|
| Payer | Meridian Health Plan | Payer ID: MHP-001 |
| Clearinghouse | Veridian Health Exchange | ISA06: 123456789 |
| Claims Platform | CoreClaim Adjudication | Simulated |
| UM Vendor | PriorPath UM Solutions | Simulated |
| Provider (A) | Lakewood Medical Center | NPI: 1234567890 |
| Provider (B) | Riverside Women's Health | NPI: 9876543210 |
| Provider (C) | Summit Orthopedic Group | NPI: 5551234567 |
| Clinician (A) | Dr. Eleanor Marsh, MD | DEA: Not used |
| Clinician (B) | Dr. Carmen Reyes, MD | DEA: Not used |
| Clinician (C) | Dr. Thomas Hale, MD | DEA: Not used |

All names, NPIs, claim numbers, and identifiers are entirely fictional.

---

## Technology Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core, Razor Pages |
| ORM | Entity Framework Core |
| Database | SQLite |
| UI | Bootstrap 5 |
| File storage | Local disk (`wwwroot/documents/`) |
| Language | C# |
| X12 parsing | Raw segment parsing (ISA/GS/ST loop) |
| XML validation | `System.Xml` + embedded XSD |
| Signature verification | `System.Security.Cryptography.Xml` |
| Hashing | `System.Security.Cryptography.SHA256` |

---

## Database Entities

### DemoScenario
Seeded. One row per scenario (A, B, C). Drives scenario selector in presenter mode.

### AttachmentTransaction
Core entity. One row per attachment submission.

| Field | Type | Notes |
|---|---|---|
| Id | int | PK |
| ScenarioId | int | FK → DemoScenario |
| TrackingNumber | string | ISA13 control number |
| ProviderNPI | string | |
| ProviderName | string | |
| PatientName | string | |
| PatientDOB | DateOnly | |
| ClaimNumber | string | |
| DocumentType | string | LOINC display name |
| LoincCode | string | |
| SubmittedAt | DateTime | |
| X12Envelope | string | Full raw X12 275 text |
| CdaDocument | string | Full raw CDA XML |
| Status | string | see Statuses |
| IsTampered | bool | for tamper-evidence demo |

### ValidationStageResult
One row per pipeline stage per transaction.

| Field | Type | Notes |
|---|---|---|
| Id | int | PK |
| AttachmentTransactionId | int | FK |
| StageOrder | int | 1–8 |
| StageName | string | |
| Passed | bool | |
| DurationMs | int | simulated realistic timing |
| Detail | string | JSON: errors, warnings, parsed fields |
| ExecutedAt | DateTime | |

### AuditEvent
Immutable. One row per pipeline event.

| Field | Type | Notes |
|---|---|---|
| Id | int | PK |
| AttachmentTransactionId | int | FK |
| EventType | string | |
| Description | string | |
| EventHash | string | SHA-256 of (previous hash + event data) |
| PreviousHash | string | |
| OccurredAt | DateTime | |

### PresenterSession
Tracks the current demo state for the presenter console.

| Field | Type | Notes |
|---|---|---|
| Id | int | PK |
| ActiveScenarioId | int | FK |
| CurrentScreen | int | 1–7 |
| StartedAt | DateTime | |
| IsActive | bool | |

---

## Validation Pipeline Stages

All eight stages run in sequence via `ValidationPipelineService.RunAsync()`.

| # | Stage | Service | Real or Simulated |
|---|---|---|---|
| 1 | X12 Envelope Parse | `X12ParserService` | **Real** |
| 2 | CDA Schema Validation | `CdaSchemaValidator` | **Real** (XSD) |
| 3 | C-CDA Template Validation | `CcdaTemplateValidator` | **Real** (rules engine) |
| 4 | Schematron Evaluation | `SchematronEvaluator` | Simulated (realistic output) |
| 5 | LOINC Classification | `LoincClassifier` | **Real** (lookup table) |
| 6 | XMLDSig Verification | `XmlSigVerifier` | **Real** |
| 7 | Claim Match | `ClaimMatchService` | Real (DB lookup) |
| 8 | Audit Hash Chain | `AuditHashService` | **Real** |

---

## Phased Build Plan

| Phase | What gets built |
|---|---|
| 1 — Foundation | `Program.cs`, `AppDbContext`, Models, `SeedData`, EF migrations, base layout, home/scenario selector |
| 2 — Pipeline Core | All 8 validation services, `ValidationPipelineService`, synthetic X12 + CDA documents for all 3 scenarios |
| 3 — Screens 3 & 4 | Trading Partner Gateway, Validation Pipeline (the technical heart) |
| 4 — Screens 5, 6, 7 | Document Repository, Claims + UM Handoff, Audit Trail |
| 5 — Screens 1 & 2 | Today's Reality (pain state), Compliance Dashboard |
| 6 — Presenter Console | `/Presenter` page with talk track, Q&A, scenario selector, timing |
| 7 — Polish | Customization, performance, scenario B failure/recovery, rehearsal |

---

## Coding Rules

- No real PHI. All names, DOBs, NPIs, and claim numbers are fictional.
- No real clearinghouse, claims platform, or UM vendor APIs.
- Synthetic X12 and CDA documents must be structurally valid — a real parser must parse them without errors (except Scenario B's deliberate-failure document).
- Do not over-engineer. No unnecessary abstractions.
- No comments that describe what the code does. Only comment the non-obvious why.
- Prefer editing existing files over creating new ones.
- Bootstrap for all UI. No custom CSS frameworks.
- File storage is local disk only.
- SQLite only.
- The deliberate-failure scenario (B) must fail at exactly stage 3 (C-CDA template validation), not earlier. The X12 envelope and CDA schema must be valid; only the C-CDA template check fails.
- Performance target: every screen loads under 1.5 seconds. Pipeline animation completes in under 3 seconds.
