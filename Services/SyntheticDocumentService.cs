using Cms0053Demo.Models;

namespace Cms0053Demo.Services;

// Returns pre-built synthetic X12 275 envelopes and CDA R2 documents for each scenario.
// All documents are structurally valid for real parsers.
// Scenario B's "bad" CDA is missing the recordTarget element (patient identity block — required by CDA R2)
// so it fails stage 3 (C-CDA template validation) exactly — stages 1 and 2 pass.
public class SyntheticDocumentService
{
    public string GetX12Envelope(DemoScenario scenario) => scenario.Code switch
    {
        "A" => X12_A,
        "B" => X12_B,
        "C" => X12_C,
        _   => throw new ArgumentException($"Unknown scenario code: {scenario.Code}")
    };

    public string GetCdaDocument(DemoScenario scenario, bool corrected = false) => scenario.Code switch
    {
        "A" => CDA_A,
        "B" => corrected ? CDA_B_CORRECTED : CDA_B_BAD,
        "C" => CDA_C,
        _   => throw new ArgumentException($"Unknown scenario code: {scenario.Code}")
    };

    // ── X12 275 Envelopes ────────────────────────────────────────────────────────

    private const string X12_A = @"ISA*00*          *00*          *ZZ*LAKEWOODMEDCTR *ZZ*MERIDIANHLTHPLN*260610*0800*^*00501*000000001*0*P*:~
GS*HI*LAKEWOODMEDCTR*MERIDIANHLTHPLN*20260610*0800*1*X*006020X314~
ST*275*0001~
BHT*0010*11*TRN-A-2026-0047*20260610*0800*HC~
NM1*41*2*LAKEWOOD MEDICAL CENTER*****XX*1234567890~
NM1*PR*2*MERIDIAN HEALTH PLAN*****PI*MHP-001~
TRN*1*CLM-A-2026-0047*1234567890~
LS*2000~
NM1*QC*1*WHITFIELD*HAROLD****MI*MHP-A-000047~
DMG*D8*19480322*M~
LE*2000~
SE*10*0001~
GE*1*1~
IEA*1*000000001~";

    private const string X12_B = @"ISA*00*          *00*          *ZZ*RIVERSIDEWOMENS*ZZ*MERIDIANHLTHPLN*260610*0915*^*00501*000000002*0*P*:~
GS*HI*RIVERSIDEWOMENS*MERIDIANHLTHPLN*20260610*0915*2*X*006020X314~
ST*275*0001~
BHT*0010*11*TRN-B-2026-0112*20260610*0915*HC~
NM1*41*2*RIVERSIDE WOMENS HEALTH*****XX*9876543210~
NM1*PR*2*MERIDIAN HEALTH PLAN*****PI*MHP-001~
TRN*1*CLM-B-2026-0112*9876543210~
LS*2000~
NM1*QC*1*REYES*SOFIA****MI*MHP-B-000112~
DMG*D8*19910714*F~
LE*2000~
SE*10*0001~
GE*1*1~
IEA*1*000000002~";

    private const string X12_C = @"ISA*00*          *00*          *ZZ*SUMMITORTHO    *ZZ*MERIDIANHLTHPLN*260610*1030*^*00501*000000003*0*P*:~
GS*HI*SUMMITORTHO*MERIDIANHLTHPLN*20260610*1030*3*X*006020X314~
ST*275*0001~
BHT*0010*11*TRN-C-2026-0088*20260610*1030*HC~
NM1*41*2*SUMMIT ORTHOPEDIC GROUP*****XX*5551234567~
NM1*PR*2*MERIDIAN HEALTH PLAN*****PI*MHP-001~
TRN*1*CLM-C-2026-0088*5551234567~
LS*2000~
NM1*QC*1*PARK*DOUGLAS****MI*MHP-C-000088~
DMG*D8*19651109*M~
LE*2000~
SE*10*0001~
GE*1*1~
IEA*1*000000003~";

    // ── CDA R2 Documents ─────────────────────────────────────────────────────────

    private const string CDA_A = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ClinicalDocument xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <typeId root=""2.16.840.1.113883.1.3"" extension=""POCD_HD000040""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.1""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.8""/>
  <id root=""2.16.840.1.113883.19"" extension=""CLM-A-2026-0047-CDA""/>
  <code code=""18842-5"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Discharge summary""/>
  <title>Discharge Summary — Harold Whitfield</title>
  <effectiveTime value=""20260609""/>
  <confidentialityCode code=""N"" codeSystem=""2.16.840.1.113883.5.25""/>
  <recordTarget>
    <patientRole>
      <id extension=""MHP-A-000047"" root=""2.16.840.1.113883.19.5""/>
      <patient>
        <name><given>Harold</given><family>Whitfield</family></name>
        <administrativeGenderCode code=""M"" codeSystem=""2.16.840.1.113883.5.1""/>
        <birthTime value=""19480322""/>
      </patient>
    </patientRole>
  </recordTarget>
  <author>
    <time value=""20260609""/>
    <assignedAuthor>
      <id extension=""1234567890"" root=""2.16.840.1.113883.4.6""/>
      <assignedPerson>
        <name><prefix>Dr.</prefix><given>Eleanor</given><family>Marsh</family><suffix>MD</suffix></name>
      </assignedPerson>
      <representedOrganization>
        <name>Lakewood Medical Center</name>
      </representedOrganization>
    </assignedAuthor>
  </author>
  <custodian>
    <assignedCustodian>
      <representedCustodianOrganization>
        <id extension=""1234567890"" root=""2.16.840.1.113883.4.6""/>
        <name>Lakewood Medical Center</name>
      </representedCustodianOrganization>
    </assignedCustodian>
  </custodian>
  <component>
    <structuredBody>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.13""/>
          <code code=""10154-3"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Chief complaint""/>
          <title>Chief Complaint</title>
          <text>Chest pain and progressive shortness of breath on exertion for three days.</text>
        </section>
      </component>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.2.1""/>
          <code code=""8648-8"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Hospital course [Summarized]""/>
          <title>Hospital Course</title>
          <text>Patient admitted 2026-06-05 with NSTEMI. Urgent cardiac catheterization performed 2026-06-06 revealing 85% LAD stenosis. PCI with drug-eluting stent placement. Post-procedure course uneventful. Stable for discharge 2026-06-09.</text>
        </section>
      </component>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.24""/>
          <code code=""11535-2"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Discharge diagnosis""/>
          <title>Discharge Diagnoses</title>
          <text>1. Non-ST elevation myocardial infarction (NSTEMI), ICD-10 I21.4. 2. Coronary artery disease, native vessel, ICD-10 I25.10.</text>
        </section>
      </component>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.11.1""/>
          <code code=""75310-3"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Discharge medications""/>
          <title>Discharge Medications</title>
          <text>Aspirin 81mg daily; Clopidogrel 75mg daily x 12 months; Metoprolol succinate 25mg daily; Atorvastatin 80mg daily; Lisinopril 5mg daily.</text>
        </section>
      </component>
    </structuredBody>
  </component>
</ClinicalDocument>";

    // Scenario B — BAD: recordTarget element intentionally omitted per CMS-0053 spec.
    // Stage 1 (X12 parse) and Stage 2 (CDA schema) pass. Stage 3 (C-CDA template) fails.
    // recordTarget is required by CDA R2; its absence identifies the patient — critical for claim linkage.
    private const string CDA_B_BAD = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!-- INTENTIONALLY INVALID — recordTarget omitted for deliberate failure demo -->
<ClinicalDocument xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <typeId root=""2.16.840.1.113883.1.3"" extension=""POCD_HD000040""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.1""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.7""/>
  <id root=""2.16.840.1.113883.19"" extension=""CLM-B-2026-0112-CDA""/>
  <code code=""11504-8"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Surgical operation note""/>
  <title>Operative Report — Sofia Reyes</title>
  <effectiveTime value=""20260610""/>
  <confidentialityCode code=""N"" codeSystem=""2.16.840.1.113883.5.25""/>
  <!-- recordTarget intentionally absent — triggers Stage 3 C-CDA template failure -->
  <author>
    <time value=""20260610""/>
    <assignedAuthor>
      <id extension=""9876543210"" root=""2.16.840.1.113883.4.6""/>
      <assignedPerson>
        <name><prefix>Dr.</prefix><given>Carmen</given><family>Reyes</family><suffix>MD</suffix></name>
      </assignedPerson>
      <representedOrganization>
        <name>Riverside Women's Health</name>
      </representedOrganization>
    </assignedAuthor>
  </author>
  <custodian>
    <assignedCustodian>
      <representedCustodianOrganization>
        <id extension=""9876543210"" root=""2.16.840.1.113883.4.6""/>
        <name>Riverside Women's Health</name>
      </representedCustodianOrganization>
    </assignedCustodian>
  </custodian>
  <component>
    <structuredBody>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.29""/>
          <code code=""29554-3"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Procedure indications""/>
          <title>Procedure Indications</title>
          <text>Elective cesarean section at 39 weeks gestation due to prior uterine surgery.</text>
        </section>
      </component>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.28""/>
          <code code=""10219-4"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Procedure description""/>
          <title>Procedure Description</title>
          <text>Patient was brought to the operating suite and placed in dorsal supine position. Spinal anesthesia administered. Low transverse uterine incision performed. Male infant delivered in vertex presentation. Uterine incision closed in two layers. Estimated blood loss 450 mL.</text>
        </section>
      </component>
    </structuredBody>
  </component>
</ClinicalDocument>";

    // Scenario B — CORRECTED: includes the required Procedure Description section.
    private const string CDA_B_CORRECTED = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ClinicalDocument xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <typeId root=""2.16.840.1.113883.1.3"" extension=""POCD_HD000040""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.1""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.7""/>
  <id root=""2.16.840.1.113883.19"" extension=""CLM-B-2026-0112-CDA-R2""/>
  <code code=""11504-8"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Surgical operation note""/>
  <title>Operative Report — Sofia Reyes (Corrected)</title>
  <effectiveTime value=""20260610""/>
  <confidentialityCode code=""N"" codeSystem=""2.16.840.1.113883.5.25""/>
  <recordTarget>
    <patientRole>
      <id extension=""MHP-B-000112"" root=""2.16.840.1.113883.19.5""/>
      <patient>
        <name><given>Sofia</given><family>Reyes</family></name>
        <administrativeGenderCode code=""F"" codeSystem=""2.16.840.1.113883.5.1""/>
        <birthTime value=""19910714""/>
      </patient>
    </patientRole>
  </recordTarget>
  <author>
    <time value=""20260610""/>
    <assignedAuthor>
      <id extension=""9876543210"" root=""2.16.840.1.113883.4.6""/>
      <assignedPerson>
        <name><prefix>Dr.</prefix><given>Carmen</given><family>Reyes</family><suffix>MD</suffix></name>
      </assignedPerson>
      <representedOrganization>
        <name>Riverside Women's Health</name>
      </representedOrganization>
    </assignedAuthor>
  </author>
  <custodian>
    <assignedCustodian>
      <representedCustodianOrganization>
        <id extension=""9876543210"" root=""2.16.840.1.113883.4.6""/>
        <name>Riverside Women's Health</name>
      </representedCustodianOrganization>
    </assignedCustodian>
  </custodian>
  <component>
    <structuredBody>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.29""/>
          <code code=""29554-3"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Procedure indications""/>
          <title>Procedure Indications</title>
          <text>Elective cesarean section at 39 weeks gestation due to prior uterine surgery.</text>
        </section>
      </component>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.28""/>
          <code code=""10219-4"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Procedure description""/>
          <title>Procedure Description</title>
          <text>Patient was brought to the operating suite and placed in dorsal supine position. Spinal anesthesia administered. Low transverse uterine incision performed. Male infant delivered in vertex presentation, Apgar scores 8 and 9. Uterine incision closed in two layers. Estimated blood loss 450 mL. Patient tolerated procedure well.</text>
        </section>
      </component>
    </structuredBody>
  </component>
</ClinicalDocument>";

    private const string CDA_C = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ClinicalDocument xmlns=""urn:hl7-org:v3"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <typeId root=""2.16.840.1.113883.1.3"" extension=""POCD_HD000040""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.1""/>
  <templateId root=""2.16.840.1.113883.10.20.22.1.3""/>
  <id root=""2.16.840.1.113883.19"" extension=""CLM-C-2026-0088-CDA""/>
  <code code=""34117-2"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""History and physical note""/>
  <title>History and Physical — Douglas Park</title>
  <effectiveTime value=""20260608""/>
  <confidentialityCode code=""N"" codeSystem=""2.16.840.1.113883.5.25""/>
  <recordTarget>
    <patientRole>
      <id extension=""MHP-C-000088"" root=""2.16.840.1.113883.19.5""/>
      <patient>
        <name><given>Douglas</given><family>Park</family></name>
        <administrativeGenderCode code=""M"" codeSystem=""2.16.840.1.113883.5.1""/>
        <birthTime value=""19651109""/>
      </patient>
    </patientRole>
  </recordTarget>
  <author>
    <time value=""20260608""/>
    <assignedAuthor>
      <id extension=""5551234567"" root=""2.16.840.1.113883.4.6""/>
      <assignedPerson>
        <name><prefix>Dr.</prefix><given>Thomas</given><family>Hale</family><suffix>MD</suffix></name>
      </assignedPerson>
      <representedOrganization>
        <name>Summit Orthopedic Group</name>
      </representedOrganization>
    </assignedAuthor>
  </author>
  <custodian>
    <assignedCustodian>
      <representedCustodianOrganization>
        <id extension=""5551234567"" root=""2.16.840.1.113883.4.6""/>
        <name>Summit Orthopedic Group</name>
      </representedCustodianOrganization>
    </assignedCustodian>
  </custodian>
  <component>
    <structuredBody>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.20""/>
          <code code=""10164-2"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""History of present illness""/>
          <title>History of Present Illness</title>
          <text>Mr. Park is a 60-year-old male presenting with 18 months of progressive right knee pain, worse with weight-bearing activities. Conservative management including NSAIDs and physical therapy has failed. Radiographs demonstrate Kellgren-Lawrence grade 4 osteoarthritis of the right knee. Patient is a candidate for total knee arthroplasty.</text>
        </section>
      </component>
      <component>
        <section>
          <templateId root=""2.16.840.1.113883.10.20.22.2.4.1""/>
          <code code=""8716-3"" codeSystem=""2.16.840.1.113883.6.1"" displayName=""Vital signs""/>
          <title>Vital Signs</title>
          <text>BP 138/84 mmHg; HR 72 bpm; RR 16/min; Temp 98.4°F; SpO2 97% on room air; Ht 5'11""; Wt 194 lbs; BMI 27.1.</text>
        </section>
      </component>
    </structuredBody>
  </component>
</ClinicalDocument>";
}
