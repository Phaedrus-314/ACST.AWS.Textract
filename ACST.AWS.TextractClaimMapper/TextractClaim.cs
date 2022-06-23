
namespace ACST.AWS.TextractClaimMapper
{
    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.TextractClaimMapper.OCR;
    using System;

    public class TextractClaim<T> 
        where T: IClaim
    {
        public string ClaimNo { get { return Claim.ClaimNo; } }

        public bool IsValid { get { return Claim.IsValid;  } }

        public TextractDocument TextractDocument { get; set; }

        public Page Page { get { return TextractDocument?.Pages[0]; } }

        public T Claim { get; set; }

        public TextractClaim(TextractDocument textractDocument)
        {
            this.TextractDocument = textractDocument;
        }

        //public void Export(OCRResultMetaData metaData)
        //{
        //    try
        //    {
        //        string claimFn = Configuration.Instance.Textract_ClaimExport_FileTemplate.Replace("{KeyName}", $"{metaData.ClaimNo}.xml");
        //        Serializer.SerializeToXML<T>(Claim, claimFn);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

    }
}
