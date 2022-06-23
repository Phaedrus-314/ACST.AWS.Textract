
namespace ACST.AWS.TransferUtility
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon;
    using S3 = Amazon.S3;
    using Amazon.S3.Transfer;

    using System.IO.Compression;
    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using System.Xml.Serialization;

    public class FileTransfer
    {

        #region Properties & Fields
        
        static S3.IAmazonS3 S3Client;

        static string defaultS3Bucket => Configuration.Instance.AWS_S3_BucketName;

        static readonly RegionEndpoint defaultBucketRegion = RegionEndpoint.USEast1;

        public static string BucketName { get; private set; }

        public static string S3Bucket { get; private set; }
        #endregion

        static FileTransfer()
        {
            S3Bucket = defaultS3Bucket;
            S3Client = new S3.AmazonS3Client(defaultBucketRegion);
        }

        #region Methods
        
        public static long FileSize(string fileName)
        {
            #region Validation & Logging

            if (fileName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fileName));

            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            //Logger.TraceInfo($"Upload File: {Path.GetFileName(fileName)}");
            
            #endregion

            FileInfo fi = new FileInfo(fileName);
            string length;
            if (fi.Length >= (1 << 10))
                length = string.Format("{0} Kb", fi.Length >> 10);
            else
                length = "0";

            Logger.TraceInfo($"{Path.GetFileName(fileName)}\tFile Size: {length}");

            return fi.Length;
        }

        public static async Task UploadDirectoryAsync(string directoryPath, string wildCard = "*")
        {
            #region Validation & Logging

            if (directoryPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
                throw new FileNotFoundException(directoryPath);

            Logger.TraceInfo($"Upload Directory: {directoryPath} [ {wildCard} ]");
            #endregion

            try
            {
                var directoryTransferUtility = new S3.Transfer.TransferUtility(S3Client);

                var request = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = S3Bucket,
                    Directory = directoryPath,
                    SearchOption = SearchOption.TopDirectoryOnly,
                    SearchPattern = wildCard
                };
                request.UploadDirectoryProgressEvent +=
                    new EventHandler<UploadDirectoryProgressArgs>
                        (UploadRequest_UploadPartProgressEvent);

                await directoryTransferUtility.UploadDirectoryAsync(request);

                Console.WriteLine("Upload statement 3 completed");
            }
            catch (S3.AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        public static async Task UploadFileAsync(string fileName, string keyName = null)
        {
            #region Validation & Logging
            
            if (fileName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(fileName));

            if (!File.Exists(fileName)) 
                throw new FileNotFoundException(fileName);

            //Logger.TraceInfo($"Upload File: {Path.GetFileName(fileName)}");
            Logger.TraceInfo($"{Path.GetFileName(fileName)}\tUpload File");
            #endregion

            try
            {
                if (keyName.IsNullOrEmpty())
                    keyName = Path.GetFileNameWithoutExtension(fileName);
                
                var fileTransferUtility = new S3.Transfer.TransferUtility(S3Client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = S3Bucket,
                    FilePath = fileName,
                    StorageClass = S3.S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456, // 6 MB.
                    Key = keyName,
                    CannedACL = S3.S3CannedACL.PublicRead
                };

                //// Add any relevant metadata
                //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

                fileTransferUtilityRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>(UploadRequest_UploadPartProgressEvent);

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                Logger.TraceVerbose("{Path.GetFileName(fileName)}\tUpload complete");
            }
            catch (S3.AmazonS3Exception e)
            {
                Logger.TraceError("AWS Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Exception[] list = new Exception[] { e };
                throw new AggregateException("Exception rethrown as Aggregate", list);
            }
        }

        public static void CompressOCRResults(OCRResultMetaData metaData, bool deleteSourceFolder = false)
        {
            CompressOCRResults(metaData.WorkingFolder, metaData.SourceFileName, metaData.ImageFileName, deleteSourceFolder);
        }

        public static void CompressOCRResults(string sourceFolder, string destinationArchiveFileName, string sourceImageFile = null, bool deleteSourceFolder = false)
        {
            
            if (!Directory.Exists(sourceFolder))
                throw new DirectoryNotFoundException(sourceFolder);


            if (!sourceFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                sourceFolder = sourceFolder + Path.DirectorySeparatorChar;

            if (sourceImageFile != null && File.Exists(sourceImageFile))
                File.Copy(sourceImageFile, Path.Combine(sourceFolder, Path.GetFileName(sourceImageFile)), true);

            if (File.Exists(destinationArchiveFileName))
                File.Delete(destinationArchiveFileName);

            ZipFile.CreateFromDirectory(sourceFolder, destinationArchiveFileName);

            if (deleteSourceFolder)
                Directory.Delete(sourceFolder, true);
        }

        public static OCRResultMetaData DecompressOCRResults(string archiveFileName, bool overwrite = false)
        {
            string uniqFolder = OCRResultMetaData.GenerateUniqueWorkingFolder(Configuration.Instance.Textract_TempFolder, archiveFileName);

            return DecompressOCRResults(archiveFileName, uniqFolder, overwrite); ;
        }

        public static OCRResultMetaData DecompressOCRResults(string archiveFileName, string destinationFolder, bool overwrite = false)
        {
            #region Validation & Logging
            
            if (!File.Exists(archiveFileName))
                throw new FileNotFoundException(archiveFileName);

            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            if (overwrite && Directory.Exists(destinationFolder))
            {
                Directory.Delete(destinationFolder, true);
            }
            #endregion

            ZipFile.ExtractToDirectory(archiveFileName, destinationFolder);

            string prefix = Path.GetFileNameWithoutExtension(archiveFileName);

            var metaData = Serializer.DeserializeFromXmlFile<OCRResultMetaData>(Path.Combine(destinationFolder, prefix + "_MetaData.xml"));
            
            metaData.WorkingFolder = Path.GetFullPath(destinationFolder);

            return metaData;
        }

        static void UploadRequest_UploadPartProgressEvent(object sender, UploadProgressArgs e)
        {
            Logger.TraceVerbose($"\t{e.TransferredBytes}/{e.TotalBytes}: {e.PercentDone}%");
        }

        static void UploadRequest_UploadPartProgressEvent(object sender, UploadDirectoryProgressArgs e)
        {
            Logger.TraceVerbose($"\t{e.CurrentFile}:\t {e.TransferredBytes}/{e.TotalBytes}");
        }
        #endregion
    }
}
