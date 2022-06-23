
namespace ACST.AWS.TextractClaimMapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Textract.Services;
    using ACST.AWS.TransferUtility;

    public class AWSTextractClaimClient<T>
        : AWSTextractClient where T : OCR.IClaim, new()
    {

        #region Fields & Proerties

        protected static Type[] AWSModelExtraTypes = new Type[] { typeof(NewGeometry), typeof(NewBoundingBox), typeof(MatchedFieldKey) };

        protected static string defaultTextractResultHTMLTransform => Configuration.Instance.Textract_ResultHTMLTransform;

        public string KeyName { get; set; }

        public string ResultsFolderName { get; set; }

        public bool SerializeResultClaim { get; set; }

        public bool SerializeResultsToHTML { get; set; }
        #endregion

        #region Constructors

        public AWSTextractClaimClient(string resultsFolderName)
            : base()
        {
            this.SerializeResultClaim = true;
            this.SerializeResultsToHTML = false;

            this.ResultsFolderName = resultsFolderName;
        }

        public AWSTextractClaimClient(NamedCoordinates namedCoordinates)
            : base(defaultS3Bucket, namedCoordinates) { }

        public AWSTextractClaimClient(string resultsFolderName, string namedCoordinatesFileName, string keyName = null)
            : this(defaultS3Bucket, resultsFolderName, namedCoordinatesFileName, keyName) { }

        public AWSTextractClaimClient(string s3Bucket, string resultsFolderName, string namedCoordinatesFileName, string keyName = null) 
            :base(s3Bucket, new NamedCoordinates(namedCoordinatesFileName))
        {
            this.SerializeResultClaim = true;
            this.SerializeResultsToHTML = false;

            this.KeyName = keyName;
            this.ResultsFolderName = resultsFolderName;
        }
        #endregion

        #region Public Methods

        public OCRResultMetaData AnalyzeImage(string imageFileName)
        {
            if (this.KeyName.IsNullOrEmpty())
                this.KeyName = Path.GetFileNameWithoutExtension(imageFileName);

            return SerializeTextractClaimResults(imageFileName);
        }

        public bool SkipImage(string imageFileName)
        {
            if (FileTransfer.FileSize(imageFileName) < 100000)
                return true;

            return false;
        }

        public Task<OCRResultMetaData> ProcessImageAsync(string imageFileName)
        {
            return Task.Run(() => ProcessImage(imageFileName));
        }

        public OCRResultMetaData ProcessImage(string imageFileName)
        {
            if (this.KeyName.IsNullOrEmpty())
                this.KeyName = Path.GetFileNameWithoutExtension(imageFileName);

            //Thread.Sleep(1000); // protect from FileSystemWatcher OnCreate early event problem

            try
            {
                //if (SkipImage(imageFileName))
                //{
                //    Logger.TraceInfo("Skip image");
                //}
                //else
                //{
                    Task t = FileTransfer.UploadFileAsync(imageFileName, this.KeyName);

                    t.Wait();

                    if (t.IsCompleted)
                        return SerializeTextractClaimResults(imageFileName);
                //}
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }

            return null;
        }
        #endregion

        #region Overrides and Private Methods

        protected override List<string> FormHeader()
        {
            List<string> ret = new List<string>();

            try
            {
                int linesToReview = Configuration.Instance.ADAForm_Header_LineToReview;
                string searchTerm = Configuration.Instance.ADAForm_Header_CommonSearchTerm;

                var lineArray = this.TextractDocument
                    .ResponsePages.FirstOrDefault()
                    .Blocks.Where(b => b.BlockType == "Line")
                    .ToArray();

                int maxIndex = lineArray?.Length ?? 0;

                linesToReview = linesToReview < maxIndex ? linesToReview : maxIndex;
                for (int i = 0; i < linesToReview; i++)
                {
                    ret.Add(lineArray[i]?.Text);
                    ret.Add(lineArray[maxIndex - 1 - i]?.Text);
                }
            }
            catch (Exception)
            {
                Common.Logger.TraceVerbose("This image may not be a Dental claim.  File moved to Error archive.");
                //throw new ApplicationException("This image may not be a Dental claim.  File moved to Error archive.");
            }

            return ret;
        }

        TextractClaim<T> CreateTextractClaim()
        {
            TextractClaim<T> textractClaim = new TextractClaim<T>(this.TextractDocument);
            textractClaim.TextractDocument = this.TextractDocument;

            textractClaim.Claim = new T();
            textractClaim.Claim.Map(textractClaim.Page);

            var results = textractClaim.Claim.ValidationResults().ToList();

            if (textractClaim.IsValid)
            {
                textractClaim.Claim.AssignClaimNumber();
                Logger.TraceInfo($"Valid TextractClaim assigned ClaimNo: {textractClaim.Claim.ClaimNo}");
            }
            else
            {
                Logger.TraceInfo($"TextractClaim validation issues: {results.Count()}");
                textractClaim.Claim
                    .ValidationResults().ToList()
                    .ForEach(c => Logger.TraceInfo($"\t{c.Result}"));
            }

            return textractClaim;
        }

        OCRResultMetaData SerializeTextractClaimResults(string imageFileName)
        {
            ClaimFormType claimFormType = base.AnalyseS3(this.KeyName, new string[] { "FORMS", "TABLES" });

            var textractClaim = CreateTextractClaim();

            if (!Directory.Exists(this.ResultsFolderName))
                Directory.CreateDirectory(this.ResultsFolderName);

            OCRResultMetaData metaData = new OCRResultMetaData
            {
                ClaimType = new T().ClaimType,
                ImageFileName = Path.GetFileName(imageFileName),
                FormType = claimFormType,
                ClaimNo = textractClaim.ClaimNo,
                ClaimIsValid = textractClaim.IsValid,

                //ToDo: don't need to serialize this anymore since it is in TextracDoument
                NamedCoordinatesFileName = Path.GetFileName(this.NamedCoordinates.ConfigurationFileName),

                TextractJsonResultFileName = Path.ChangeExtension(this.KeyName, "json"),
                TextractClaimFileName = Path.ChangeExtension(this.KeyName + "_Claim", "xml"),

                MatchedFieldHtmlFileName = Path.ChangeExtension(this.KeyName, "html"),
                TextractXmlResultFileName = Path.ChangeExtension(this.KeyName, "xml")
            };

            string metaDataFn = Path.Combine(this.ResultsFolderName, this.KeyName + "_MetaData.xml");
            string claimFn = Path.Combine(this.ResultsFolderName, metaData.TextractClaimFileName);

            //ToDo: don't need to serialize this anymore since it is in TextracDoument
            string ncFn = Path.Combine(this.ResultsFolderName, metaData.NamedCoordinatesFileName);
            string jsonFn = Path.Combine(this.ResultsFolderName, metaData.TextractJsonResultFileName);
            string xmlFn = Path.Combine(this.ResultsFolderName, metaData.TextractXmlResultFileName);
            string htmlFn = Path.Combine(this.ResultsFolderName, metaData.MatchedFieldHtmlFileName);

            string btsClaimfn = Configuration.Instance
                .Textract_ClaimExport_FileTemplate.Replace("{KeyName}", $"{metaData.ClaimNo}.xml");

            Serializer.SerializeToXML<OCRResultMetaData>(metaData, metaDataFn);
            Serializer.SerializeToXML<NamedCoordinates>(this.NamedCoordinates, ncFn);
            //Serializer.SerializeToJSON<TextractDocument>(this.TextractDoc, jsonFn, true);
            Serializer.SerializeToJSON<TextractDocument>(this.TextractDocument, jsonFn);

            if (this.SerializeResultClaim)
                Serializer.SerializeToXML<T>(textractClaim.Claim, claimFn);

            if (metaData.ClaimIsValid)
            {
                Serializer.SerializeToXML<T>(textractClaim.Claim, btsClaimfn);
                textractClaim.Claim.StoreClaimDetails(metaData.ImageFileName);
            }

            if (this.SerializeResultsToHTML)
            {
                Serializer.SerializeToXML<Page>(this.Pages, xmlFn, AWSModelExtraTypes);
                Serializer.TransformToHTML(xmlFn, htmlFn, defaultTextractResultHTMLTransform);
            }

            return metaData;
        }
        #endregion
    }
}
