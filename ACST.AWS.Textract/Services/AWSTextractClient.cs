namespace ACST.AWS.Textract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ACST.AWS.Common;
    using Amazon.Textract;

    using ACST.AWS.Textract.Model;
    using Amazon.Textract.Model;
    using FuzzySharp;

    public class AWSTextractClient
    {
        // ToDo: Handle configuration using AWS config for .net in App.config
        //currently credentials and config are stored in C:\Users\Phaed\.aws files

        //this.FlowDefinitionARN = @"arn:aws:sagemaker:us-east-1:450590327631:flow-definition/testocrreview1";

        #region Properties and Fields

        IAmazonTextract Instance { get; }

        readonly TextractTextAnalysisService textractAnalysisService;

        public string FlowDefinitionARN { get; set; }

        protected NamedCoordinates NamedCoordinates { get; set; }

        protected List<Page> Pages { get { return TextractDocument.Pages; } }

        protected static string defaultS3Bucket => Configuration.Instance.AWS_S3_BucketName;

        public string S3Bucket { get; protected set; }

        protected TextractDocument TextractDocument { get; private set; }
        #endregion

        #region Constructors

        public AWSTextractClient()
        {
            //// Default NamedCoordinates
            //this.NamedCoordinates = new NamedCoordinates(Configuration.Instance.ADANamedCoordinates_FileName_Template.Replace("{FormType}", ClaimFormType.ADA2019.ToString()));

            this.S3Bucket = defaultS3Bucket;
            this.Instance = new AmazonTextractClient();
            this.textractAnalysisService = new TextractTextAnalysisService(Instance);
        }

        public AWSTextractClient(string s3Bucket, string namedCoordinatesFileName)
        {
            this.S3Bucket = s3Bucket;
            this.Instance = new AmazonTextractClient();
            this.NamedCoordinates = new NamedCoordinates(namedCoordinatesFileName);
            //this.Instance = new AmazonTextractClient(new Amazon.Runtime.BasicAWSCredentials("accesskey", "secretekey"), new Amazon.RegionEndpoint());
            this.textractAnalysisService = new TextractTextAnalysisService(Instance);
        }

        public AWSTextractClient(string s3Bucket, NamedCoordinates namedCoordinates)
        {
            this.S3Bucket = s3Bucket;
            this.Instance = new AmazonTextractClient();
            this.NamedCoordinates = namedCoordinates;
            //this.Instance = new AmazonTextractClient(new Amazon.Runtime.BasicAWSCredentials("accesskey", "secretekey"), new Amazon.RegionEndpoint());
            this.textractAnalysisService = new TextractTextAnalysisService(Instance);
        }
        #endregion

        #region Methods

        public void RawS3Analysis(string s3Name, string destFileName, string[] features)
        {
            var task = textractAnalysisService.StartDocumentAnalysis(this.S3Bucket, s3Name, features);

            var jobId = task.Result;

            textractAnalysisService.WaitForJobCompletion(jobId);

            var response = textractAnalysisService.GetJobResults(jobId);

            List<GetDocumentAnalysisResponse> responseList = new List<GetDocumentAnalysisResponse>();

            responseList.Add(response);

            string nextToken = response.NextToken;

            while (nextToken != null)
            {
                response = textractAnalysisService.GetJobResults(jobId, nextToken);
                responseList.Add(response);
                nextToken = response.NextToken;
            }

            TextractDocument = new TextractDocument(responseList);

            TextractDocument.ParseRaw();

            Serializer.SerializeToJSON<TextractDocument>(this.TextractDocument, destFileName, true);
        }

        /// <summary>
        /// Textract analysis of S3 sourced file
        /// </summary>
        /// <param name="s3Bucket"></param>
        /// <param name="fileName"></param>
        /// <param name="features"></param>
        /// 
        public ClaimFormType AnalyseS3(string fileName, string[] features)
        {
            #region Validation & Logging

            if (this.S3Bucket.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(this.S3Bucket));

            if (fileName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(fileName));
            
            Logger.TraceInfo($"{fileName}\tAWSTextract Client, Handling S3: {this.S3Bucket}");
            Logger.TraceInfo($"{fileName}\tFeatures: {string.Join(", ",features)}");

            if (NamedCoordinates == null)
            {
                //string fn = NamedCoordinatesFileNameByFormType(ParseClaimFormType(FormHeader()));
                //this.NamedCoordinates = new NamedCoordinates(fn);
                //Logger.TraceInfo($"NamedCoordinates derived from FileType: {fn}");
                //Logger.TraceInfo($"Default NamedCoordinates: null");
            }
            else
                Logger.TraceInfo($"NamedCoordinates: {this.NamedCoordinates.Source}, {this.NamedCoordinates.ConfigurationFileName}");
            #endregion
            
            GetDocumentAnalysisResponse response = null;
            string jobId = null;

            try
            {
                var task = textractAnalysisService.StartDocumentAnalysis(this.S3Bucket, fileName, features);

                jobId = task.Result;

                textractAnalysisService.WaitForJobCompletion(jobId);

                response = textractAnalysisService.GetJobResults(jobId);
            }
            catch (Exception ex)
            {
                Logger.TraceWarning("AWS Throtteling TextractAnalysisServices GetJobResults.");
                Logger.TraceVerbose(ex.ToString());
                //throw;
            }

            List<GetDocumentAnalysisResponse> responseList = new List<GetDocumentAnalysisResponse>();

            responseList.Add(response);

            string nextToken = response.NextToken;

            while (nextToken != null)
            {
                response = textractAnalysisService.GetJobResults(jobId, nextToken);
                responseList.Add(response);
                nextToken = response.NextToken;
            }

            //TextractDoc = new TextractDocument(responseList, this.NamedCoordinates);
            this.TextractDocument = new TextractDocument(responseList);

            ClaimFormType formType = ParseClaimFormHeader(FormHeader());
            var nc = NamedCoordinatesFileNameByFormType(formType);
            
            this.NamedCoordinates = new NamedCoordinates(nc);

            this.TextractDocument.ParseWithNamedCoordinates(this.NamedCoordinates);

            return formType;
        }

        /// <summary>
        /// Textract analysis of locally sourced image file (jpg, png) but not PDF
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="features"></param>
        public ClaimFormType AnalyseLocal(string fileName, string[] features)
        {
            #region Validation & Logging

            if (fileName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(fileName));

            Logger.TraceInfo($"AWSTextract Client, Handling Local: {fileName}");
            Logger.TraceInfo($"Features: {string.Join(", ", features)}");

            if (NamedCoordinates == null)
            {
                //string fn = NamedCoordinatesFileNameByFormType(ParseClaimFormType(FormHeader()));
                //this.NamedCoordinates = new NamedCoordinates(fn);
                //Logger.TraceInfo($"NamedCoordinates derived from FileType: {fn}");
                Logger.TraceInfo($"NamedCoordinates is null");
            }
            else
                Logger.TraceInfo($"NamedCoordinates: {this.NamedCoordinates.Source}, {this.NamedCoordinates.ConfigurationFileName}");
            #endregion

            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                file.CopyTo(ms);

            //this.FlowDefinitionARN = @"arn:aws:sagemaker:us-east-1:450590327631:flow-definition/testocrreview1";
            AnalyzeDocumentResponse response;
            AnalyzeDocumentRequest request = new AnalyzeDocumentRequest()
            {
                Document = new Document() { Bytes = ms },
                FeatureTypes = features.ToList()
            };

            response = this.Instance.AnalyzeDocument(request);

            //ToDo: add Coordinate selector here, to pick the correct NamedCoordinates

            //TextractDocument = new TextractDocument(response, this.NamedCoordinates);

            TextractDocument = new TextractDocument(response);

            ClaimFormType formType = ParseClaimFormHeader(FormHeader());

            var nc = NamedCoordinatesFileNameByFormType(formType);

            this.NamedCoordinates = new NamedCoordinates(nc);

            TextractDocument.ParseWithNamedCoordinates(this.NamedCoordinates);

            return formType;
        }
        #endregion

        #region Protected Methods
        
        protected virtual List<string> FormHeader()
        {
            throw new NotImplementedException();
        }

        protected virtual string NamedCoordinatesFileNameByFormType(ClaimFormType type)
        {
            return NamedCoordinates.GetFileNameFromFormType(type);
        }

        public int MatchByFuzzyText(string target, string value)
        {
            return Fuzz.Ratio(target, value);
            ////Todo: consolidate this with FieldKeyMatch
            //string log = "";

            //var results = this.Where(nc => nc?.GroupName == "Table" && nc?.ExactTextMatch != null)
            //        .Select(nc => new { NC = nc, Score = Fuzz.Ratio(nc?.ExactTextMatch, cell?.Text) })
            //        .Where(x => x?.Score >= Configuration.Instance.FuzzyMatch_MinimumScore)
            //        //.Select(x => x.NC);
            //        ;

            //foreach (var r in results)
            //{
            //    var n = r.NC?.ExactTextMatch;
            //    if (n != null)
            //    {
            //        log += $"Fuzzy Score = {r.Score}:\n\tOCR.Field:\t'{cell?.Text}'\n\tNamedCoord:\t'{n}'\n";
            //        //log += $"Fuzzy Score = {Fuzz.Ratio(n, fieldKey?.Text)}:\n\tOCR.Field:\t'{fieldKey?.Text}'\n\tNamedCoord:\t'{n}'";
            //        //log = log.Length < 120 
            //        //    ? log.Replace("~", string.Empty).Replace("`", " ") 
            //        //    : log.Replace("~", "\t" + Environment.NewLine + "\t").Replace("`", "\t");
            //        //log = log.Replace("~", "\t" + Environment.NewLine + "\t").Replace("`", "\t");
            //    }
            //}
            //if (log.IsNotNullOrWhiteSpace())
            //    Logger.TraceVerbose(log.TrimEnd('\n'));

            ////return results.Select(s => s.NC);

            //if (results.Count() == 0) return null;

            //// return NC with max score
            //return results.Where(r => r != null).Aggregate((i1, i2) => i1.Score > i2.Score ? i1 : i2).NC;
        }

        protected virtual ClaimFormType ParseClaimFormHeader(List<string> header)
        {
            // ToDo: This needs to be configured or removed...
            //ClaimFormType cft = ClaimFormType.Unknown;
            ClaimFormType cft = ClaimFormType.ADA2012;
            //List<Tuple<ClaimFormType, int>> versionRainking = new List<Tuple<ClaimFormType, int>>();


            if (header == null || header.Count() == 0)
                return cft;

            //header.ForEach(h => {
            //    versionRainking.Add(new Tuple<ClaimFormType, int>( ClaimFormType.ADA2006, MatchByFuzzyText(h, "Dental Claim Form")));
            //    versionRainking.Add(new Tuple<ClaimFormType, int>(ClaimFormType.ADA2012, MatchByFuzzyText(h, "ADA American Dental Association Dental Claim Form")));
            //    versionRainking.Add(new Tuple<ClaimFormType, int>(ClaimFormType.ADA2006, MatchByFuzzyText(h, "Dental Claim Form")));
            //    versionRainking.Add(new Tuple<ClaimFormType, int>(ClaimFormType.ADA2006, MatchByFuzzyText(h, "Dental Claim Form")));


            //    //else if (header.StartsWith("ADA American Dental", StringComparison.OrdinalIgnoreCase))
            //    //    cft = ClaimFormType.ADA2012;
            //});


            

            if (header.FirstOrDefault(s => s.Contains("2006")) != null)
                cft = ClaimFormType.ADA2006;
            else if (header.FirstOrDefault(s => s.Contains("2012")) != null)
                cft = ClaimFormType.ADA2012;
            else if (header.Contains("Dental Claim Form"))
                cft = ClaimFormType.ADA2006;
            else if (header.Contains("ADA American Dental Association Dental Claim Form"))
                cft = ClaimFormType.ADA2012;

            //else if (header.StartsWith("ADA American Dental", StringComparison.OrdinalIgnoreCase))
            //    cft = ClaimFormType.ADA2012;

            return cft;
        }
        #endregion
    }
}
