
namespace ACST.AWS.Textract.Services
{
    using System;
    using System.Collections.Generic;

    using ACST.AWS.Common;
    using ACST.AWS.Textract.Model;
    using Amazon.Textract.Model;

    public class AWSTextractCache
    {
        #region Properties and Fields

        public TextractDocument TextractDocument { get; protected set; }
        #endregion

        #region Constructors
        
        public AWSTextractCache() { }
        #endregion

        protected virtual string FormHeader()
        {
            throw new NotImplementedException();
        }

        protected virtual string NamedCoordinatesFileNameByFormType(ClaimFormType type)
        {
            throw new NotImplementedException();
        }

        protected virtual ClaimFormType ParseClaimFormType(string value)
        {
            throw new NotImplementedException();
        }

        #region Methods

        public void AnalyseLocalDocument(string fileName, NamedCoordinates namedCoordinates, bool overrideIgnoreAttributes = false)
        {
            this.TextractDocument = Serializer.DeserializeFromJsonFile<TextractDocument>(fileName, overrideIgnoreAttributes);
            List<Page> ps = this.TextractDocument.Pages;

            GetDocumentAnalysisResponse g = new GetDocumentAnalysisResponse
            {
                Blocks = ps[0].Blocks,
                //Blocks = ps.SelectMany(b => b.Blocks).ToList(),
                DocumentMetadata = new DocumentMetadata() { Pages = 1 },
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                JobStatus = Amazon.Textract.JobStatus.SUCCEEDED,
                NextToken = null,
                ResponseMetadata = null,
                StatusMessage = "Success",
                Warnings = null
            };

            this.TextractDocument = new TextractDocument(g, namedCoordinates);
        }

        public void AnalyseLocalDocument(string fileName)
        {
            this.TextractDocument = Serializer.DeserializeFromJsonFile<TextractDocument>(fileName, true);

            this.TextractDocument.ParseWithNamedCoordinates(this.TextractDocument.NamedCoordinates);
        }

        public void OpenLocalDocument(string fileName)
        {
            this.TextractDocument = Serializer.DeserializeFromJsonFile<TextractDocument>(fileName, true);
        }
        #endregion

    }
}
