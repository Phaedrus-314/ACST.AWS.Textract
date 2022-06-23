
namespace ACST.AWS.TextractClaimMapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ACST.AWS.Common;
    using ACST.AWS.TransferUtility;

    public class TestWorkingLocal
    {
        public static void TestTransmissionInsert()
        {
            ACST.AWS.Data.EDI.Context edi = new Data.EDI.Context();
            //edi.SetImageTransmission();
            edi.TestInsert();
        }

        public static async Task BuildTextractClaimAsyncTest()
        {
            //string namedCoordinatesFileName = @".\NamedCoordinatesData\ADANamedCoordinates_2.xml";
            //string sourceFile = @"C:\temp\NewDental\Individuals_ClaimsOnly\Dental Claims 060820 23.jpg";

            //string keyName = "dental_23";
            ////string resultsFolderName = FormattedPathName.TimeStampFolder(@"c:\temp\dental\{keyName}_{yyyyMMdd_HHmmss}\");
            //string resultsFolderName = FormattedPathName.TimeStampFolder(Configuration.Instance.Watch_TempResultsFolderTemplate);

            //AWSTextractClaimBuilder<ADAClaim> client = new AWSTextractClaimBuilder<ADAClaim>(resultsFolderName, namedCoordinatesFileName, keyName);
            //TestOCRSerializer.TextractClaim<ADAClaim> t = await client.BuildTextractClaimAsync(sourceFile);

        }

        public static async Task ProcessImageAsyncTest(string imageFileName, string namedCoordinatesFileName)
        {
            //string keyName = null;
            ////string resultsFolderName = FormattedPathName.TimeStampFolder(@"c:\temp\dental\{keyName}_{yyyyMMdd_HHmmss}\");
            //string resultsFolderName = FormattedPathName.TimeStampFolder(Configuration.Instance.Watch_TempResultsFolderTemplate);

            ////resultsFolderName = resultsFolderName.Replace("{keyName}", keyName);

            ////Task<OCRResultMetaData> t = AWSClient.OCRDentalJpgAsync(sourceFile, resultsFolder, keyName, namedCoordinatesFileName);
            ////t.Wait();

            ////if (t.IsCompleted)
            ////{
            ////    OCRResultMetaData metaData = t.Result;
            ////    AWSClient.CompressOCRResults(resultsFolder, Path.Combine(@"c:\temp\dental\", keyName + ".zip"), sourceFile, false);
            ////    AWSClient.DecompressOCRResults(Path.Combine(@"c:\temp\dental\", keyName + ".zip"), @"c:\temp\dental\Decompress", true);
            ////}
            //AWSTextractClaimBuilder<ADAClaim> client = new AWSTextractClaimBuilder<ADAClaim>(resultsFolderName, namedCoordinatesFileName, keyName);
            //OCRResultMetaData metaData = await client.ProcessImageAsync(imageFileName);
                        
            //FileTransfer.CompressOCRResults(resultsFolderName, Configuration.Instance.Textract_ResultsFileTemplate.Replace("{KeyName}", client.KeyName), imageFileName, false);
        }

        public static void SimulateADAtextractClient(string fn)
        {
            // Load from compressed TextractClaimarchive
            AWSTextractClaimCache<ADA.ADAClaim> s
                = new AWSTextractClaimCache<ADA.ADAClaim>(fn);

            s.BuildTextractClaim();

            //TestOCRSerializer.TextractClaim<TestOCRSerializer.ADA.ADAClaim> textractClaim
            //    = new TestOCRSerializer.TextractClaim<ADAClaim>(s.TextractDocument);


            ////"Geometry": {
            ////    "BoundingBox": {
            ////        "Height": 0.0140520968,
            ////          "Left": 0.0371363126,
            ////          "Top": 0.4292703,
            ////          "Width": 0.119889483
            ////        },

            //var x1 = textractClaim.Page
            //    .GetCellsByRowCoordinate(new System.Drawing.PointF() { X = 0.039F, Y = 0.43F });

            ////context.Parents.Where(p => p.Childs.Any(y=> y.ParentId == yourId));
            //var x = textractClaim.Page.Tables[0].Rows
            //    .Where(r => r.Cells.Any(c => c.MappedToClaim));
            ////.Where(r => r.Cells.Any(c => c.ColumnIndex > 1 && c.MappedToClaim))
            ////.Where(r => r.Cells.Any(c => c.Geometry.ContainsPointF(coordinate)))
            //;

            //var rr = textractClaim.Claim.ServiceLines[2].ProcedureDate;
        }

        public static void BuildSampleADAData()
        {
            var claim = ConfigurationBuilder.SamplePopulatedTypeBuilder.GenerateDental();

            string fn = @"C:\temp\dental\_Export\SampleADA.xml";
            Serializer.SerializeToXML<ADA.ADAClaim>(claim, fn);
        }

        public static void TestConfigurationGenerator_IntermediateFiles()
        {
            //string fn = @"C:\temp\generator\Dental Claims 060820 3.json";
            string fn = @"C:\temp\dental\_Archive\Dental Claims 060820 3.zip";
            string ncfn = @"C:\Users\Phaed\source\repos\ACST\ACST.AWS.TextractViewer\bin\Debug\NamedCoordinatesData\NamedCoordinates_ADA2012.xml";

            // Intermediate generator
            ConfigurationBuilder.NamedCoordinatesConfigurationGenerator
                .GenerateIntermediateConfig(new ADA.ADANamedCoordinates(ncfn), fn, @"C:\temp\generator\");
        }

        public static void TestConfigurationGenerator_FinalFiles()
        {
            //NamedCoordinates nc = ConfigurationBuilder.NamedCoordinatesConfigurationGenerator.CMSDefaultNamedCoordinateInstance();
            //nc.SerializeToXMLFile(@"c:\temp\CMSNamedCoordinates_DefaultInstance.xml");

            var nc = ConfigurationBuilder.NamedCoordinatesConfigurationGenerator.ADADefaultNamedCoordinateInstance2();
            ConfigurationBuilder.NamedCoordinatesConfigurationGenerator.GenerateConfig(nc, @"C:\Users\Phaed\source\repos\ACST\ACST.AWS.TextractViewer\bin\Debug\NamedCoordinatesData\NamedCoordinates_ADA2012.xml");
        }
    }
}
