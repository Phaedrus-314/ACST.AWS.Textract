
namespace ACST.AWS.TextractClaimMapper.OCR
{
    using ACST.AWS.Textract.Model;
    using System.Collections.Generic;
    public interface IServiceLine
    {
        void ParseCompositeValues();

        bool Map(Page page, int row);

        void Map(List<MatchedLine> lines, int row);

        IEnumerable<ValidationResult> ValidationResults();
    }
}