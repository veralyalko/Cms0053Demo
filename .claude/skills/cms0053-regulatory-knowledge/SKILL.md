---
name: cms0053-regulatory-knowledge
description: "Use this skill whenever a task involves CMS-0053 regulatory
  specifics: X12 275/277 v6020 envelope structure, HL7 CDA R2 March 2022 IG
  requirements, LOINC document type codes, XMLDSig signature requirements,
  ACN reconciliation rules, or compliance dates and deadlines. Provides the
  authoritative reference so the same regulatory details are used consistently
  across all components."
---

# CMS-0053 Regulatory Knowledge

## Key Dates
- Final rule effective: May 26, 2026
- Mandatory compliance deadline: May 26, 2028

## Adopted Standards
1. Transport: X12N 275 v6020 (006020X314) and X12N 277 v6020 (006020X313)
2. Document content: HL7 CDA R2, C-CDA March 2022 Implementation Guide
3. Classification: LOINC (active version at time of receipt)
4. Signatures: HL7 CDA R2 Digital Signatures and Delegation of Rights,
   Release 1 (XMLDSig profile)

## X12 275 v6020 Envelope Structure

Required segments in order:
- ISA (interchange envelope start)
- GS (functional group start)
- ST (transaction set start; ID = 275)
- BGN (beginning segment)
    - BGN01: 11 = solicited, 13 = unsolicited
    - BGN02: Attachment Control Number (ACN)
    - BGN03: date YYYYMMDD
    - BGN04: time HHMM
- NM1 (payer)
- NM1 (source/provider)
- LX (transaction iteration loop, supports multiple documents)
    - TRN (trace number)
    - DMG (patient demographics, optional)
    - CAT (category, optional)
    - BDS (Binary Data Segment carrying the document)
- SE (transaction set end)
- GE (functional group end)
- IEA (interchange envelope end)

## C-CDA Required Elements

Every C-CDA document must contain:
- realmCode code="US"
- typeId root="2.16.840.1.113883.1.3" extension="POCD_HD000040"
- templateId for the specific document type (e.g., 2.16.840.1.113883.10.20.22.1.8
  for Discharge Summary)
- id with root and extension
- code (LOINC document type)
- title
- effectiveTime
- confidentialityCode
- languageCode
- recordTarget — THE element used for the deliberate failure scenario (Scenario B)
- author
- custodian
- component/structuredBody with required sections per document type

## Deliberate Failure Scenario (Scenario B)
The correct deliberate failure per spec is a **missing `recordTarget` element**.
The current implementation fails on a missing procedure description section — this
is a known deviation that should be corrected.

## Acknowledgments
- TA1: interchange envelope acknowledgment
- 999: implementation acknowledgment (functional level)
- 824: application advice (business-level acceptance/rejection)

## Important Distinctions
- CMS-0053 covers CLAIMS attachments only (post-service documentation)
- CMS-0057 covers PRIOR AUTHORIZATION (pre-service) — different rule
- The two do NOT share standards (X12/CDA vs FHIR)

## LOINC Code Subset (12 codes)
| Code     | Document Type                        |
|----------|--------------------------------------|
| 18842-5  | Discharge summary                    |
| 11504-8  | Surgical operation note              |
| 11526-1  | Pathology study                      |
| 18748-4  | Diagnostic imaging study             |
| 11506-3  | Progress note                        |
| 28570-0  | Procedure note                       |
| 34117-2  | History and physical note            |
| 11488-4  | Consultation note                    |
| 28568-4  | Emergency department note            |
| 57133-1  | Referral note                        |
| 52030-1  | Hospital course narrative            |
| 18761-7  | Provider-unspecified transfer summary|

## References
- Final Rule: 45 CFR Part 162 (effective May 26, 2026)
- X12N 006020X314 Type 3 Technical Report
- HL7 C-CDA Implementation Guide R2.1, March 2022
- HL7 CDA R2 Digital Signatures and Delegation of Rights, Release 1
