
namespace ACST.AWS.TextractClaimMapper.ADA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.TextractClaimMapper.OCR;

    public class ADAServiceLine
        : OCR.BaseServiceLine, OCR.IServiceLine
    {

        public const int TableHeaderMinMatchScore = 70;
        public const string RecordOfServicesProvidedLiteral = "Record of Services Provided";
        
        #region Properties and Fields

        public override bool HasData { get => base.HasData; set => base.HasData = value; }

        [XmlIgnore]
        [TableColumnPositionalValueAttribute("ServiceLine_RowId", 0)]
        public Cell RowId { get; set; }

        [XmlIgnore]
        [TableColumnPositionalValueAttribute("ServiceLine_ProcedureDate", 1, true)]
        public override Cell ProcedureDate { get; set; }

        string _ServiceStartDate;
        public override string ServiceStartDate { get => _ServiceStartDate; set => _ServiceStartDate = value.ToDateFormat(); }

        string _ServiceEndDate;
        public override string ServiceEndDate { get => _ServiceEndDate; set => _ServiceEndDate = value.ToDateFormat(); }

        //public override string ServiceStartDate { get => base.ServiceStartDate; set => base.ServiceStartDate = value; }

        //public override string ServiceEndDate { get => base.ServiceStartDate; set => base.ServiceStartDate = value; }

        [TableColumnPositionalValueAttribute("ServiceLine_OralCavityArea", 2)]
        public string OralCavityArea { get; set; }

        //public string CrownInlayCode { get; set; }

        public bool Orthodontia { get; set; }

        [TableColumnPositionalValueAttribute("ServiceLine_ToothSystem", 3)]
        public string ToothCode { get; set; }

        [XmlIgnore]
        [TableColumnPositionalValueAttribute("ServiceLine_ToothNumbers", 4)]
        public Cell ToothSurfaces { get; set; }

        [TableColumnCompositeValueAttribute("ServiceLine_ToothNo", 4)]
        public string ToothSurfaceCode1 { get; set; }
        [TableColumnCompositeValueAttribute("ServiceLine_ToothNo", 4)]
        public string ToothSurfaceCode2 { get; set; }
        [TableColumnCompositeValueAttribute("ServiceLine_ToothNo", 4)]
        public string ToothSurfaceCode3 { get; set; }
        [TableColumnCompositeValueAttribute("ServiceLine_ToothNo", 4)]
        public string ToothSurfaceCode4 { get; set; }


        [XmlIgnore]
        [TableColumnPositionalValueAttribute("ServiceLine_ToothSurfaces", 5)]
        public Cell CavityDesignationCodes { get; set; }

        [TableColumnCompositeValueAttribute("ServiceLine_ToothSurface", 5)]
        public string CavityDesignationCode1 { get; set; }
        [TableColumnCompositeValueAttribute("ServiceLine_ToothSurface", 5)]
        public string CavityDesignationCode2 { get; set; }
        [TableColumnCompositeValueAttribute("ServiceLine_ToothSurface", 5)]
        public string CavityDesignationCode3 { get; set; }
        [TableColumnCompositeValueAttribute("ServiceLine_ToothSurface", 5)]
        public string CavityDesignationCode4 { get; set; }

        [TableColumnPositionalValueAttribute("ServiceLine_ProcedureCode", 6)]
        public override string ProcedureCode { get; set; }

        [XmlIgnore]
        [TableColumnPositionalValueAttribute("ServiceLine_DiagnosisPointers", 7)]
        public override Cell DiagnosisCodePointers { get; set; }

        [TableColumnCompositeValueAttribute("ServiceLine_DiagnosisPointer", 7)]
        public override string DiagnosisCodePointer1 { get; set; }

        [TableColumnCompositeValueAttribute("ServiceLine_DiagnosisPointer", 7)]
        public override string DiagnosisCodePointer2 { get; set; }

        [TableColumnCompositeValueAttribute("ServiceLine_DiagnosisPointer", 7)]
        public override string DiagnosisCodePointer3 { get; set; }

        [TableColumnCompositeValueAttribute("ServiceLine_DiagnosisPointer", 7)]
        public override string DiagnosisCodePointer4 { get; set; }


        [TableColumnPositionalValueAttribute("ServiceLine_Qty", 8)]
        public override string ServiceUnitCount { get; set; }


        [TableColumnPositionalValueAttribute("ServiceLine_Description", 9)]
        public override string LineNoteText { get; set; }

        string _ChargedAmount;
        [TableColumnPositionalValueAttribute("ServiceLine_Fee", 10, true)]
        public override string ChargedAmount { get => _ChargedAmount; set => _ChargedAmount = value.CleanupForCurrency(); }
        #endregion

        public ADAServiceLine()
            : base() { }

        protected override bool IsHeaderLine()
        {
            //bool f;
            
            //int SuperHeaderScore = NamedCoordinates.FuzzyScore($"{this.ProcedureDate} {this.OralCavityArea} {this.ToothCode}".Trim(), RecordOfServicesProvidedLiteral);

            //f = this.LineNoteText != null && this.LineNoteText.Contains("Description");
            //f |= this.ChargedAmount != null && this.ChargedAmount.Contains("Fee");
            //f |= this.LineItemNO != null && this.LineItemNO.Trim() == "0";

            int descriptionScore = NamedCoordinates.FuzzyScore(this.LineNoteText ?? "", "30. Description");
            int feeScore = NamedCoordinates.FuzzyScore(this.ChargedAmount ?? "", "31. Fee");
            int dateScore = NamedCoordinates.FuzzyScore(this.ServiceStartDate ?? "", "24. Procedure Date");
            int procedureScore = NamedCoordinates.FuzzyScore(this.ProcedureCode ?? "", "29. Procedure Code");

            //f = NamedCoordinates.FuzzyScore(this.LineNoteText, "Description") > TableHeaderMinMatchScore;

            return descriptionScore > TableHeaderMinMatchScore
                    | feeScore > TableHeaderMinMatchScore
                    | dateScore > TableHeaderMinMatchScore
                    | procedureScore > TableHeaderMinMatchScore
                    | this.LineItemNO.SafeTrim() == "0";
        }

        public override IEnumerable<ValidationResult> ValidationResults()
        {
            DateTime dt;
            decimal amount;

            if (this.HasData)
            {
                if (!DateTime.TryParse(this.ServiceStartDate, out dt) | dt > DateTime.Now)
                    yield return new ValidationResult("ServiceLine_ProcedureDate", "24. Procedure Date is required.  (Line-Item, Prior to today)");

                if (this.ProcedureCode.IsNullOrWhiteSpace())
                    yield return new ValidationResult("ServiceLine_ProcedureCode", "29. Procedure Code is required.");

                if (!decimal.TryParse(this.ChargedAmount, out amount) | amount <= 0)
                    yield return new ValidationResult("ServiceLine_Fee", "31. Fee is required.  (Line-Item, Positive non-zero decimal)");
            }
        }

        [CompositeValueParserAttribute]
        public override void ParseCompositeValues()
        {
            ParseProcedureDate();
            ParseToothSurfaces();
            ParseCavityDesignationCodes();
            ParseDiagnosisPointers();
            ParseUnitCount();
        }

        #region Private Methods

        void ParseProcedureDate()
        {
            string[] parts = ParseCellContents(this.ProcedureDate);

            if (parts == null || parts.Length < 1)
                return;

            DateTime dt;
            bool f = DateTime.TryParse(parts?.Last(), out dt);

            if (f)
            {
                this.ServiceStartDate = dt.ToString("yyyy-MM-dd");
            }
        }

        void ParseToothSurfaces()
        {
            int i = 0;
            string[] parts = ParseCellContents(ToothSurfaces);

            if (parts == null || parts.Length < 1)
                return;

            foreach (var part in parts.Take(4))
            {
                i++;
                switch (i)
                {
                    case 1:
                        ToothSurfaceCode1 = part;
                        break;
                    case 2:
                        ToothSurfaceCode2 = part;
                        break;
                    case 3:
                        ToothSurfaceCode3 = part;
                        break;
                    case 4:
                        ToothSurfaceCode4 = part;
                        break;
                }
            }
        }

        void ParseDiagnosisPointers()
        {
            int i = 0;
            string[] parts = ParseCellContents(DiagnosisCodePointers);

            if (parts == null || parts.Length < 1)
                return;

            foreach (var part in parts.Take(4))
            {
                i++;
                switch (i)
                {
                    case 1:
                        DiagnosisCodePointer1 = part;
                        break;
                    case 2:
                        DiagnosisCodePointer2 = part;
                        break;
                    case 3:
                        DiagnosisCodePointer3 = part;
                        break;
                    case 4:
                        DiagnosisCodePointer4 = part;
                        break;
                }
            }
        }

        void ParseCavityDesignationCodes()
        {
            int i = 0;
            string[] parts = ParseCellContents(CavityDesignationCodes);

            if (parts == null || parts.Length < 1)
                return;

            foreach (var part in parts.Take(4))
            {
                i++;
                switch (i)
                {
                    case 1:
                        CavityDesignationCode1 = part;
                        break;
                    case 2:
                        CavityDesignationCode2 = part;
                        break;
                    case 3:
                        CavityDesignationCode3 = part;
                        break;
                    case 4:
                        CavityDesignationCode4 = part;
                        break;
                }
            }
        }

        void ParseUnitCount()
        {
            if (this.IsHeader)
                return;

            try
            {
                if (this.ServiceUnitCount.IsNullOrWhiteSpace() || int.Parse(this.ServiceUnitCount) < 1)
                    this.ServiceUnitCount = 1.ToString();
            }
            catch (Exception)
            {
                // just in case QTY gets mapped on 2006 form without quantity in the table
                this.ServiceUnitCount = 1.ToString();
                //throw;
            }
        }
        #endregion
    }
}
