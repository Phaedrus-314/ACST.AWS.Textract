
namespace ACST.AWS.Textract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Amazon.Textract;
    using Amazon.Textract.Model;

    public class TextractTextAnalysisService
    {
        #region Properties and Fields

        IAmazonTextract Textract;
        #endregion

        public TextractTextAnalysisService(IAmazonTextract textract)
        {
            this.Textract = textract;
        }

        #region Methods

        public GetDocumentAnalysisResponse GetJobResults(string jobId, string nextToken = null)
        {
            var response = this.Textract.GetDocumentAnalysisAsync(new GetDocumentAnalysisRequest
            {
                JobId = jobId, NextToken = nextToken
            });
            response.Wait();
            return response.Result;
        }

        public async Task<string> StartDocumentAnalysis(string bucketName, string key, string[] featureTypes)
        {
            var request = new StartDocumentAnalysisRequest();
            var s3Object = new S3Object
            {
                Bucket = bucketName,
                Name = key
            };
            request.DocumentLocation = new DocumentLocation
            {
                S3Object = s3Object
            };
            request.FeatureTypes = new List<string>(featureTypes);
            var response = await this.Textract.StartDocumentAnalysisAsync(request);
            return response.JobId;
        }

        public bool IsJobComplete(string jobId)
        {
            var response = this.Textract.GetDocumentAnalysisAsync(new GetDocumentAnalysisRequest
            {
                JobId = jobId
            });
            response.Wait();
            return !response.Result.JobStatus.Equals("IN_PROGRESS");
        }

        public void WaitForJobCompletion(string jobId, int delay = 5000)
        {
            while (!IsJobComplete(jobId))
            {
                Wait(delay);
            }
        }

        public static void PrintDebug(GetDocumentAnalysisResponse response, int pageCnt = 1)
        {
            response.Blocks.ForEach(y => {
                Trace.WriteLine(string.Format("<block pageCnt='{0}'>", pageCnt));
                Trace.WriteLine(y.Id + ":" + y.BlockType + ":" + y.Text);
                if (y.BlockType == "KEY_VALUE_SET")
                {
                    Trace.WriteLine(" <KEY_VALUE_SET>");
                    PrintBlock(y);
                    Trace.WriteLine(" </KEY_VALUE_SET>");
                }
                else if (y.BlockType == "TABLE")
                {
                    Trace.WriteLine(" <TABLE>");
                    PrintBlock(y);
                    Trace.WriteLine(" </TABLE>");
                }
                else if (y.BlockType == "CELL")
                {
                    Trace.WriteLine(" <CELL>");
                    PrintBlock(y);
                    Trace.WriteLine(" </CELL>");
                }
                Trace.WriteLine("</block>");
            });
        }

        static void PrintBlock(Block block)
        {
            Trace.WriteLine("  <entity>");
            block.EntityTypes.ForEach(z => Trace.WriteLine("   " + z));
            Trace.WriteLine("  </entity>");
            block.Relationships.ForEach(z => {
                Trace.WriteLine("  <relation>");
                Trace.WriteLine("   " + z.Type);
                Trace.WriteLine("   <id>");
                z.Ids.ForEach(a => Trace.WriteLine("    " + a));
                Trace.WriteLine("   </id>");
                Trace.WriteLine("  </relation>");
            });
        }

        static void Wait(int delay = 5000)
        {
            Task.Delay(delay).Wait();
            Console.Write(".");
        }
        #endregion
    }
}