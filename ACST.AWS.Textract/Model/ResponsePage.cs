using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{
    public class ResponsePage
    {
        public List<Block> Blocks { get; set; }

        public DocumentMetadata DocumentMetadata { get; set; }

        public ResponsePage() { }

        public ResponsePage(AnalyzeDocumentResponse page)
        {
            #region Validation & Logging

            if (page == null)
                throw new ArgumentNullException(nameof(page));
            #endregion

            this.Blocks = page.Blocks;
            this.DocumentMetadata = page.DocumentMetadata;
        }

        public ResponsePage(GetDocumentAnalysisResponse page)
        {
            #region Validation & Logging

            if (page == null)
                throw new ArgumentNullException(nameof(page));
            #endregion

            this.Blocks = page.Blocks;
            this.DocumentMetadata = page.DocumentMetadata;
        }

        public static explicit operator ResponsePage(AnalyzeDocumentResponse page) => new ResponsePage(page);

        public static explicit operator ResponsePage(GetDocumentAnalysisResponse page) => new ResponsePage(page);
    }
}
