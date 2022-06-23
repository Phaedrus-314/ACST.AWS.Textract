
namespace ACST.AWS.Common
{
    using System;
    using System.Configuration;
    using System.IO;

    public sealed class Configuration
    {
        private static readonly Lazy<Configuration>
            lazy =
            new Lazy<Configuration>
                (() => new Configuration());

        public static Configuration Instance { get { return lazy.Value; } }

        private Configuration()
        {
            SetConfigurationValues();

            if (this.EnsureAllFolders)
                EnsureConfiguredFolders();
        }

        public string AWS_S3_BucketName { get; set; }

        public string AWS_S3_BucketRegionName { get; set; }

        public bool AWS_S3_UploadSource { get; set; }

        public string AWS_FlowDefinitionARN { get; set; }

        public int ADAForm_Header_LineToReview { get; set; }

        public string ADAForm_Header_CommonSearchTerm { get; set; }

        public string ADANamedCoordinates_FileName { get; set; }

        public string CMSNamedCoordinates_FileName { get; set; }

        public bool EnsureAllFolders { get; set; }

        public int FuzzyMatch_MinimumScore { get; set; }

        public TextractClaimMatchStrategy NamedCoordinates_ClaimMatch_Strategy { get; set; }

        public string NamedCoordinates_FileName_Template { get; set; }

        public string Textract_ClaimExport_FileTemplate { get; set; }

        public string Textract_ResultHTMLTransform { get; set; }

        public string Textract_TempFolder { get; set; }
        
        public string Textract_ArchiveFolder { get; set; }

        public string Textract_ReviewFolder { get; set; }

        public string Watch_TempResultsFolderTemplate { get; set; }

        public string Watch_Folder { get; set; }

        public string Watch_FileFilter { get; set; }
        
        public string Watch_ResultsFolderTemplate { get; set; }

        public string Watch_ProcessedFolder_WithReview { get; set; }

        public string Watch_ProcessedFolder_WithoutReview { get; set; }

        public string Watch_ProcessedFolder_Error { get; set; }

        public string Watch_ProcessedFolder_Skip { get; set; }

        void EnsureConfiguredFolders()
        {
            // Verify all standard folders
            if (!Directory.Exists(this.Textract_TempFolder))
                Directory.CreateDirectory(this.Textract_TempFolder);

            if (!Directory.Exists(this.Textract_ReviewFolder))
                Directory.CreateDirectory(this.Textract_ReviewFolder);

            if (!Directory.Exists(this.Watch_Folder))
                Directory.CreateDirectory(this.Watch_Folder);

            if (!Directory.Exists(this.Watch_ProcessedFolder_WithReview))
                Directory.CreateDirectory(this.Watch_ProcessedFolder_WithReview);

            if (!Directory.Exists(this.Watch_ProcessedFolder_WithoutReview))
                Directory.CreateDirectory(this.Watch_ProcessedFolder_WithoutReview);

            if (!Directory.Exists(this.Watch_ProcessedFolder_Error))
                Directory.CreateDirectory(this.Watch_ProcessedFolder_Error);

            if (!Directory.Exists(this.Watch_ProcessedFolder_Skip))
                Directory.CreateDirectory(this.Watch_ProcessedFolder_Skip);

            if (!Directory.Exists(this.Textract_ArchiveFolder))
                Directory.CreateDirectory(this.Textract_ArchiveFolder);

            // Verify all template folders
            string testFolder = Path.GetDirectoryName(this.Watch_ResultsFolderTemplate);

            if (!Directory.Exists(testFolder))
                Directory.CreateDirectory(testFolder);

            testFolder = Path.GetDirectoryName(this.Textract_ClaimExport_FileTemplate);

            if (!Directory.Exists(testFolder))
                Directory.CreateDirectory(testFolder);

            testFolder = Path.GetDirectoryName(this.Watch_TempResultsFolderTemplate);

            if (!Directory.Exists(testFolder))
                Directory.CreateDirectory(testFolder);
        }

        void SetConfigurationValues()
        {
            try
            {
                AWS_S3_BucketName = ConfigurationManager.AppSettings["AWS_S3_BucketName"]; 
                AWS_S3_BucketRegionName = ConfigurationManager.AppSettings["AWS_S3_BucketRegionName"];
                AWS_S3_UploadSource = bool.Parse(ConfigurationManager.AppSettings["AWS_S3_UploadSource"].ToString());
                AWS_FlowDefinitionARN = ConfigurationManager.AppSettings["AWS_FlowDefinitionARN"];

                ADAForm_Header_CommonSearchTerm = ConfigurationManager.AppSettings["ADAForm_Header_CommonSearchTerm"];
                ADAForm_Header_LineToReview = Int32.Parse(ConfigurationManager.AppSettings["ADAForm_Header_LineToReview"]);

                ADANamedCoordinates_FileName = ConfigurationManager.AppSettings["ADANamedCoordinates_FileName"];
                CMSNamedCoordinates_FileName = ConfigurationManager.AppSettings["CMSNamedCoordinates_FileName"];

                EnsureAllFolders = bool.Parse(ConfigurationManager.AppSettings["EnsureAllFolders"]);

                FuzzyMatch_MinimumScore = Int32.Parse(ConfigurationManager.AppSettings["FuzzyMatch_MinimumScore"]);

                NamedCoordinates_FileName_Template = ConfigurationManager.AppSettings["NamedCoordinates_FileName_Template"];
                NamedCoordinates_ClaimMatch_Strategy = (TextractClaimMatchStrategy)Enum.Parse(typeof(TextractClaimMatchStrategy), ConfigurationManager.AppSettings["NamedCoordinates_ClaimMatch_Strategy"] as string);

                Textract_ResultHTMLTransform = ConfigurationManager.AppSettings["Textract_ResultHTMLTransform"];

                Textract_ArchiveFolder = ConfigurationManager.AppSettings["Textract_ArchiveFolder"];
                Textract_ReviewFolder = ConfigurationManager.AppSettings["Textract_ReviewFolder"];
                Textract_TempFolder = ConfigurationManager.AppSettings["Textract_TempFolder"];

                Textract_ClaimExport_FileTemplate = ConfigurationManager.AppSettings["Textract_ClaimExport_FileTemplate"];

                Watch_Folder = ConfigurationManager.AppSettings["Watch_Folder"];
                Watch_FileFilter = ConfigurationManager.AppSettings["Watch_FileFilter"];
                Watch_ResultsFolderTemplate = ConfigurationManager.AppSettings["Watch_ResultsFolderTemplate"];
                Watch_TempResultsFolderTemplate = ConfigurationManager.AppSettings["Watch_TempResultsFolderTemplate"];

                Watch_ProcessedFolder_WithoutReview = ConfigurationManager.AppSettings["Watch_ProcessedFolder_WithoutReview"];
                Watch_ProcessedFolder_WithReview = ConfigurationManager.AppSettings["Watch_ProcessedFolder_WithReview"];
                Watch_ProcessedFolder_Error = ConfigurationManager.AppSettings["Watch_ProcessedFolder_Error"];
                Watch_ProcessedFolder_Skip = ConfigurationManager.AppSettings["Watch_ProcessedFolder_Skip"];

            }
            catch(System.FormatException fx)
            {
                Logger.TraceError($"Error loading App.Configuration settings.  Review all non-string values.{Environment.NewLine}{fx.Message}");
                throw;

            }
            catch (Exception ex)
            {
                Logger.TraceError($"Error loading App.Configuration settings.  Review all values.{Environment.NewLine}{ex.Message}");
                throw;
            }
        }
    }
}
