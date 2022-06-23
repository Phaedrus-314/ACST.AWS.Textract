
namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Amazon.Textract.Model;

    using ACST.AWS.Common;

    public class TextractDocument
    {

        #region Properties & Fields

        List<Block> blockMap = new List<Block>();

        readonly List<List<Block>> documentPages = new List<List<Block>>();

        [XmlIgnore]
        public NamedCoordinates NamedCoordinates { get; set; }

        public List<Page> Pages { get; set; }

        [XmlIgnore]
        public List<List<Block>> PageBlocks
        {
            get
            {
                return this.documentPages;
            }
        }

        public List<ResponsePage> ResponsePages { get; set; }
        #endregion

        public TextractDocument() 
        {
            this.ResponsePages = new List<ResponsePage>();
        }

        public TextractDocument(AnalyzeDocumentResponse response, NamedCoordinates namedCoordinates)
            : this()
        {
            #region Validation & Logging

            if (response == null)
                throw new ArgumentNullException(nameof(response));
            #endregion

            this.ResponsePages.Add((ResponsePage)response);

            ParseWithNamedCoordinates(namedCoordinates);
        }

        public TextractDocument(List<GetDocumentAnalysisResponse> responses)
            : this()
        {
            #region Validation & Logging

            if (responses == null)
                throw new ArgumentNullException(nameof(responses));

            //if (namedCoordinates == null)
            //    throw new ArgumentNullException(nameof(namedCoordinates));
            #endregion

            this.Pages = new List<Page>();

            responses.ForEach(r => this.ResponsePages.Add((ResponsePage)r));
        }

        public TextractDocument(AnalyzeDocumentResponse response)
            : this()
        {
            #region Validation & Logging

            if (response == null)
                throw new ArgumentNullException(nameof(response));

            //if (namedCoordinates == null)
            //    throw new ArgumentNullException(nameof(namedCoordinates));
            #endregion

            this.Pages = new List<Page>();

            this.ResponsePages.Add((ResponsePage)response);
        }

        public TextractDocument(GetDocumentAnalysisResponse response, NamedCoordinates namedCoordinates)
            : this()
        {
            #region Validation & Logging

            if (response == null)
                throw new ArgumentNullException(nameof(response));
            #endregion


            this.ResponsePages.Add((ResponsePage)response);

            ParseWithNamedCoordinates(namedCoordinates);
        }

        public TextractDocument(List<GetDocumentAnalysisResponse> responses, NamedCoordinates namedCoordinates)
            : this()
        {
            #region Validation & Logging

            if (responses == null)
                throw new ArgumentNullException(nameof(responses));
            #endregion

            responses.ForEach(r => this.ResponsePages.Add((ResponsePage)r));

            ParseWithNamedCoordinates(namedCoordinates);
        }


        public void ParseRaw()
        {
            //this.Pages = new List<Page>();
            this.ParseDocumentPagesAndBlockMap();
            this.Parse();
        }

        public void ParseWithNamedCoordinates(NamedCoordinates namedCoordinates)
        {
            #region Validation & Logging

            if (namedCoordinates == null)
                throw new ArgumentNullException(nameof(namedCoordinates));

            //Logger.TraceInfo($"{Environment.NewLine}Parse with NamedCoordinates: {namedCoordinates.Source}, {namedCoordinates.ConfigurationFileName}");
            Logger.TraceInfo($"Parse with NamedCoordinates: {namedCoordinates.Source}, {namedCoordinates.ConfigurationFileName}");
            #endregion

            this.blockMap.Clear();
            this.documentPages.Clear();
            this.Pages = new List<Page>();

            this.NamedCoordinates = namedCoordinates;

            this.ParseDocumentPagesAndBlockMap();
            this.Parse();
        }

        private void ParseDocumentPagesAndBlockMap()
        {
            List<Block> documentPage = null;
            
            this.ResponsePages.ForEach(page => {
                page.Blocks.ForEach(block => {
                    this.blockMap.Add(block);

                    if (block.BlockType == "PAGE")
                    {
                        if (documentPage != null)
                        {
                            this.documentPages.Add(documentPage);
                        }
                        documentPage = new List<Block>();
                        documentPage.Add(block);
                    }
                    else
                    {
                        documentPage.Add(block);
                    }
                });
            });

            if (documentPage != null)
            {
                this.documentPages.Add(documentPage);
            }
        }

        private void Parse()
        {
            this.documentPages.ForEach(documentPage => {
                var page = new Page(documentPage, this.blockMap, this.NamedCoordinates);
                this.Pages.Add(page);
            });
        }
               
        public Block GetBlockById(string blockId)
        {
            return this.blockMap.Find(x => x.Id == blockId);
        }
    }
}
