
namespace ACST.AWS.TextractClaimMapper.OCR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Common;
    using ACST.AWS.TextractClaimMapper.OCR;

    public class BaseServiceLine : IServiceLine
    {

        #region Properties and Fields

        //public virtual TextractClaimTableRowType RowType { get { return ((MatchedCell)ProcedureDate).RowType; } }
        public virtual TextractClaimTableRowType RowType { get; set; }

        public virtual bool HasData { get; set; }

        public virtual bool IsHeader { get { return IsHeaderLine(); } }

        public bool IsValid
        {
            get { return (ValidationResults().Count() == 0); }
        }

        public virtual string LineItemNO { get; set; }
        
        public virtual string ProductIDQualifier { get; set; }
        
        public virtual string ProcedureCode { get; set; }

        public virtual string ProcedureModifier1 { get; set; }

        public virtual string ProcedureModifier2 { get; set; }

        public virtual string ProcedureModifier3 { get; set; }

        public virtual string ProcedureModifier4 { get; set; }

        [XmlIgnore]
        public virtual Cell DiagnosisCodePointers { get; set; }

        public virtual string DiagnosisCodePointer1 { get; set; }
        
        public virtual string DiagnosisCodePointer2 { get; set; }

        public virtual string DiagnosisCodePointer3 { get; set; }

        public virtual string DiagnosisCodePointer4 { get; set; }


        //public virtual decimal ChargedAmount { get; set; }
        public virtual string ChargedAmount { get; set; }

        public virtual string UnitsMeasurementCode { get; set; }

        public virtual string ServiceUnitCount { get; set; }
        
        public virtual string NoteReferenceCode { get; set; }
        
        public virtual string LineNoteText { get; set; }

        [XmlIgnore]
        public virtual Cell ProcedureDate { get; set; }

        public virtual string PlaceOfService { get; set; }

        // Format 2018-03-20
        public virtual string ServiceStartDate { get; set; }

        public virtual string ServiceEndDate { get; set; }

        public virtual string LineItemControlNO { get; set; }
        #endregion

        public BaseServiceLine() 
        {
            this.RowType = TextractClaimTableRowType.Unknown;
        }

        #region Methods

        //protected virtual bool IsHeaderLine(Row row)
        //{
        //    throw new NotImplementedException();
        //}

        protected virtual bool IsHeaderLine()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<ValidationResult> ValidationResults()
        {
            yield break;
        }

        //public void OnValidate(ChangeAction action)
        //{
        //    if (!IsValid)
        //        throw new ApplicationException("Rule violations prevent exporting");
        //}

        protected string[] ParseCellContents(Cell cell)
        {
            if (cell == null)
                return null;

            return cell.Content.Select(c => ((Word)c).Text).ToArray();
        }

        public bool  Map(Page page, int row)
        {
            return Map(page.Tables[0], row);
        }

        public bool Map(Table table, int row)
        {
            this.LineItemNO = row.ToString();
            return Mapper.TableValueMapping<BaseServiceLine>(this, table, row);
        }

        public void Map(List<MatchedLine> lines, int row)
        {
            // Not used with dental yet
            //Mapper.GridValueMapping<BaseServiceLine>(this, lines, row);
            Mapper.MatchedElementMapping<BaseServiceLine, MatchedLine>(this, lines, row);
        }

        public virtual void ParseCompositeValues()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
