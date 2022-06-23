
namespace ACST.AWS.TextractClaimMapper.CMS
{
    using System.Xml.Serialization;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;

    public class CMSServiceLine
        : OCR.BaseServiceLine, OCR.IServiceLine
    {
        // The position attributes below are default values and may be suplanted by data from imported NamedCoordinates.XML file.

        #region Properties and Fields

        [GridPositionalValueAttribute("ServiceLine_Description", 45, 4, 63)]
        public override string LineNoteText { get => base.LineNoteText; set => base.LineNoteText = value; }

        [GridPositionalValueAttribute("ServiceLine_AdditionalID", 45, 71, 12)]
        public string AdditionalID { get; set; }


        [GridPositionalValueAttribute("ServiceLine_StartDate", 46, 4, 9)]
        public override string ServiceStartDate { get; set; }

        [GridPositionalValueAttribute("ServiceLine_EndDate", 46, 13, 9)]
        public override string ServiceEndDate { get; set; }

        [GridPositionalValueAttribute("ServiceLine_PlaceOfService", 46, 22, 3)]
        public override string PlaceOfService { get => base.PlaceOfService; set => base.PlaceOfService = value; }

        [GridPositionalValueAttribute("ServiceLine_ProcedureCode", 46, 27, 7)]
        public override string ProcedureCode { get => base.ProcedureCode; set => base.ProcedureCode = value; }

        [GridPositionalValueAttribute("ServiceLine_ProcedureModifier1", 46, 34, 3)]
        public override string ProcedureModifier1 { get => base.ProcedureModifier1; set => base.ProcedureModifier1 = value; }

        [GridPositionalValueAttribute("ServiceLine_ProcedureModifier2", 46, 37, 3)]
        public override string ProcedureModifier2 { get => base.ProcedureModifier2; set => base.ProcedureModifier2 = value; }
        
        [GridPositionalValueAttribute("ServiceLine_ProcedureModifier3", 46, 40, 3)]
        public override string ProcedureModifier3 { get => base.ProcedureModifier3; set => base.ProcedureModifier3 = value; }
        
        [GridPositionalValueAttribute("ServiceLine_ProcedureModifier4", 46, 43, 3)]
        public override string ProcedureModifier4 { get => base.ProcedureModifier4; set => base.ProcedureModifier4 = value; }

        [XmlIgnore]
        [GridPositionalValueAttribute("ServiceLine_DiagnosisPointers", 46, 48, 5)]
        public new string DiagnosisCodePointers { get; set; }

        [GridCompositeValueAttribute("ServiceLine_DiagnosisPointer1", 46, 48, 5)]
        public override string DiagnosisCodePointer1 { get => base.DiagnosisCodePointer1; set => base.DiagnosisCodePointer1 = value; }

        [GridCompositeValueAttribute("ServiceLine_DiagnosisPointer2", 46, 48, 5)]
        public override string DiagnosisCodePointer2 { get => base.DiagnosisCodePointer2; set => base.DiagnosisCodePointer2 = value; }

        [GridCompositeValueAttribute("ServiceLine_DiagnosisPointer3", 46, 48, 5)]
        public override string DiagnosisCodePointer3 { get => base.DiagnosisCodePointer3; set => base.DiagnosisCodePointer3 = value; }

        [GridCompositeValueAttribute("ServiceLine_DiagnosisPointer4", 46, 48, 5)]
        public override string DiagnosisCodePointer4 { get => base.DiagnosisCodePointer4; set => base.DiagnosisCodePointer4 = value; }

        //[GridPositionalValueAttribute("ServiceLine_Charges", 46, 53, 9)]
        //public override decimal ChargedAmount { get; set; }

        [GridPositionalValueAttribute("ServiceLine_DaysUnits", 46, 62, 4)]
        public override string ServiceUnitCount { get; set; }

        [GridPositionalValueAttribute("ServiceLine_NPI", 46, 71, 12)]
        public string NPI { get; set; }

        #endregion

        [CompositeValueParserAttribute]
        public void ParseCompositeValues()
        {
            
        }

    }
}
