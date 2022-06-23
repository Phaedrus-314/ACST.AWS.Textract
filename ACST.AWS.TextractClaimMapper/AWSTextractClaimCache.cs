
namespace ACST.AWS.TextractClaimMapper
{
    using System;
    using System.IO;
    using System.Linq;
    using ACST.AWS.TransferUtility;
    using ACST.AWS.Common;
    using ACST.AWS.Textract.Services;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Common.OCR;

    public class AWSTextractClaimCache<T> 
        : AWSTextractCache where T : OCR.IClaim, new()
    {

        #region Fields & Properties

        protected static Type[] AWSModelExtraTypes = new Type[] { typeof(NewGeometry), typeof(NewBoundingBox), typeof(MatchedFieldKey) };

        protected static string defaultTextractResultHTMLTransform => Configuration.Instance.Textract_ResultHTMLTransform;

        public string CompressedOCRResultsFileName { get; protected set; }

        public T Claim { get; set; }

        public OCRResultMetaData MetaData { get; protected set; }
        
        public Page Page { get { return base.TextractDocument?.Pages[0]; } }
        #endregion

        #region Constructors

        public AWSTextractClaimCache()
            : base() { }

        public AWSTextractClaimCache(string compressedOCRFileName)
            : base()
        {
            this.CompressedOCRResultsFileName = compressedOCRFileName;
            this.MetaData = FileTransfer.DecompressOCRResults(this.CompressedOCRResultsFileName, true);
            this.MetaData.SourceFileName = compressedOCRFileName;
        }
        #endregion

        #region Public Methods

        public void BuildTextractClaim()
        {
            NamedCoordinates nc = new NamedCoordinates();
            nc.DeserializeFromXML(this.MetaData.RootedPath(this.MetaData.NamedCoordinatesFileName));

            BuildTextractClaim(nc);
        }

        public void BuildTextractClaim(NamedCoordinates namedCoordinates)
        {
            base.AnalyseLocalDocument(this.MetaData.RootedPath(this.MetaData.TextractJsonResultFileName), namedCoordinates);

            this.Claim = new T();
            this.Claim.Map(this.Page);
        }

        public void ReBuildClaim(NamedCoordinates namedCoordinates)
        {
            base.TextractDocument.ParseWithNamedCoordinates(namedCoordinates);
            this.Claim = new T();
            this.Claim.Map(this.Page);
        }

        public void OpenTextractClaim(bool reBuild = false)
        {
            base.OpenLocalDocument(this.MetaData.RootedPath(this.MetaData.TextractJsonResultFileName));

            this.Claim = Serializer.DeserializeFromXmlFile<T>(this.MetaData.RootedPath(this.MetaData.TextractClaimFileName));

            if (reBuild)
            {
                // bring over Fields not serialized...ToDo consider serializing these
                // this is so confidense comparison will work dirng clim validation
                ReBuildClaim(this.TextractDocument.NamedCoordinates);
            }
        }

        public static void SaveTextractClaim(TextractClaim<T> textractClaim, OCRResultMetaData metaData)
        {
            try
            {
                string claimFn = Path.Combine(metaData.WorkingFolder, metaData.TextractClaimFileName);
                //string ncFn = Path.Combine(metaData.WorkingFolder, metaData.NamedCoordinatesFileName);
                string jsonFn = Path.Combine(metaData.WorkingFolder, metaData.TextractJsonResultFileName);
                //string jsonPageFn = Path.Combine(metaData.WorkingFolder, metaData.TextractJsonResultFileName_Page);
                string xmlFn = Path.Combine(metaData.WorkingFolder, metaData.TextractXmlResultFileName);
                //string htmlFn = Path.Combine(metaData.WorkingFolder, metaData.MatchedFieldHtmlFileName);

                Serializer.SerializeToXML<T>(textractClaim.Claim, claimFn);

                //Serializer.SerializeToJSON<Page>(textractClaim.TextractDocument.Pages, jsonPageFn, true);
                Serializer.SerializeToJSON<TextractDocument>(textractClaim.TextractDocument, jsonFn, true);

                //Serializer.SerializeToXML<Page>(textractClaim.TextractDocument.Pages, xmlFn, AWSModelExtraTypes);
                //string xmlFn2 = Path.Combine(metaData.WorkingFolder, metaData.TextractXmlResultFileName + "TD");
                //Serializer.SerializeToXML<TextractDocument>(textractClaim.TextractDocument, xmlFn2, AWSModelExtraTypes);
                //Serializer.TransformToHTML(xmlFn, htmlFn, defaultTextractResultHTMLTransform);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void ExportTextractClaim(TextractClaim<T> textractClaim, OCRResultMetaData metaData)
        {
            try
            {
                string claimFn = Configuration.Instance.Textract_ClaimExport_FileTemplate.Replace("{KeyName}", $"{metaData.ClaimNo}.xml");
                Serializer.SerializeToXML<T>(textractClaim.Claim, claimFn);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Overrides and Private Methods

        protected override string FormHeader()
        {
            return this.Page.Lines.FirstOrDefault().Text;
        }

        protected override string NamedCoordinatesFileNameByFormType(ClaimFormType type)
        {
            return Configuration.Instance.NamedCoordinates_FileName_Template.Replace("{FormType}", type.ToString());
        }

        protected override ClaimFormType ParseClaimFormType(string value)
        {
            //ToDo: configure or remove this... this is a bad way of doing this...
            value = value?.Trim();

            if (value.StartsWith("ADA American Dental Association") || value.Contains("American Dent"))
                return ClaimFormType.ADA2012;

            if (value.StartsWith("Dental Claim Form") || value.Contains("Dental Claim"))
                return ClaimFormType.ADA2006;

            return ClaimFormType.Unknown;
        }
        #endregion
    }
}
