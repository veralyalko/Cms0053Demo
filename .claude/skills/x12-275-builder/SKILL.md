---
name: x12-275-builder
description: "Use when generating or modifying X12N 275 v6020 transaction
  envelopes for demo scenarios. Provides validated patterns for envelope
  construction with the correct ISA/IEA/GS/GE/ST/SE/BGN/NM1/LX/BDS structure
  for v6020 (006020X314). References the C# implementation in this project."
---

# X12 275 v6020 Builder

## Implementation
This project uses raw segment parsing in C# — no third-party X12 library.

Key files:
- `Services/X12ParserService.cs` — parses raw X12 text into `X12ParseResult`
- `Services/ValidationPipelineService.cs` — generates synthetic X12 envelopes
  per scenario in `BuildX12Envelope(DemoScenario scenario)`
- `Models/X12ParseResult.cs` — parsed field model

## Envelope Construction Pattern

Delimiters used in this project:
- Element separator: `*` (asterisk)
- Segment terminator: `~` (tilde) followed by newline for readability
- Component separator: `:` (colon)

### Segment Order (v6020)
```
ISA*00*          *00*          *ZZ*{senderId}       *ZZ*{receiverId}     *{date}*{time}*^*00501*{controlNum}*0*P*:~
GS*HI*{senderId}*{receiverId}*{date}*{time}*{groupNum}*X*006020X314~
ST*275*0001~
BGN*{11=solicited|13=unsolicited}*{ACN}*{date}*{time}**2~
NM1*PR*2*{payerName}*****PI*{payerId}~
NM1*41*2*{providerOrgName}*****XX*{NPI}~
LX*1~
TRN*1*{claimNumber}*{payerId}~
DMG*D8*{DOB_YYYYMMDD}*{M|F|U}~
SE*{segmentCount}*0001~
GE*1*{groupNum}~
IEA*1*{controlNum}~
```

### ISA Field Breakdown
| Position | Field | Value in Demo |
|---|---|---|
| ISA01 | Auth Info Qualifier | 00 |
| ISA02 | Auth Information | (10 spaces) |
| ISA03 | Security Info Qualifier | 00 |
| ISA04 | Security Information | (10 spaces) |
| ISA05 | Sender ID Qualifier | ZZ |
| ISA06 | Sender ID | Provider NPI (padded to 15 chars) |
| ISA07 | Receiver ID Qualifier | ZZ |
| ISA08 | Receiver ID | MHP001 (payer, padded to 15 chars) |
| ISA09 | Date | YYMMDD |
| ISA10 | Time | HHMM |
| ISA11 | Repetition Separator | ^ |
| ISA12 | Version | 00501 |
| ISA13 | Control Number | 9-digit zero-padded |
| ISA14 | Ack Requested | 0 |
| ISA15 | Usage Indicator | P (production) or T (test) |
| ISA16 | Component Separator | : |

### BGN Segment
- BGN01: 11 = solicited attachment, 13 = unsolicited
- BGN02: ACN (Attachment Control Number) — links to claim
- BGN03: Date YYYYMMDD
- BGN04: Time HHMM
- BGN06: (blank)
- BGN08: 2 = original

### NM1 Qualifier Codes
- PR = Payer
- 41 = Submitter (provider)
- QC = Patient
- PI = Payer Identifier
- XX = NPI

## Parsed Fields Exposed by X12ParserService
After parsing, `X12ParseResult` exposes:
- `SenderId` — ISA06 trimmed
- `ReceiverId` — ISA08 trimmed
- `ControlNumber` — ISA13
- `TransactionDate` — ISA09
- `TransactionTime` — ISA10
- `TransactionType` — ST01 (should be "275")
- `ProviderName` — NM1*41 element 3+4
- `ProviderNPI` — NM1*41 element 9
- `PatientLastName` — NM1*QC element 3
- `PatientFirstName` — NM1*QC element 4
- `PatientDob` — DMG element 2 (YYYYMMDD)
- `PatientGender` — DMG element 3
- `ClaimNumber` — TRN element 2
- `TrackingNumber` — BGN02

## Common Pitfalls
- ISA06/ISA08 must be exactly 15 characters — pad with trailing spaces
- ISA13 must be exactly 9 digits — zero-pad left
- Segment count in SE01 must include ST and SE themselves
- BGN02 (ACN) must match TRN02 (claim reference) for proper linkage
- DMG date format is D8 qualifier + YYYYMMDD (no separators)
- Each scenario's X12 is stored in `AttachmentTransaction.X12Envelope` (full raw text)
