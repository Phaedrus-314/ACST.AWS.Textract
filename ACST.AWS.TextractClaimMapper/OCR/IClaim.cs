
namespace ACST.AWS.TextractClaimMapper.OCR
{
    using ACST.AWS.Textract.Model;
    using System.Collections.Generic;
    using ACST.AWS.Common;

    public interface IClaim
    {

        //List<IServiceLine> ServiceLines { get; }

        TextractClaimType ClaimType { get; }

        string ClaimNo { get; }

        bool IsValid { get; }

        void AssignClaimNumber(bool requery = false);

        void Map(Page page);

        void ParseCompositeValues();

        void StoreClaimDetails(string imageFileName);

        IEnumerable<ValidationResult> ValidationResults();
    }
}