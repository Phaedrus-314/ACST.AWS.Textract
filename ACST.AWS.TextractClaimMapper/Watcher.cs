
namespace ACST.AWS.TextractClaimMapper
{
    using System;
    using System.IO;
    using System.Security.Permissions;

    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.TransferUtility;
    using ACST.AWS.TextractClaimMapper.ADA;

    public class Watcher
    {
        static string ProcessedFolder => Configuration.Instance.Watch_ProcessedFolder;

        static string ProcessedErrorFolder => Configuration.Instance.Watch_ProcessedErrorFolder;

        public static bool UploadSource => Configuration.Instance.AWS_S3_UploadSource;


        public static bool OverwriteResults = true;


        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run(string folderName, string filter)
        {

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = folderName;

                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                watcher.Filter = filter;

                //watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                watcher.Deleted += OnChanged;
                //watcher.Renamed += OnRenamed;

                watcher.EnableRaisingEvents = true;

                Logger.TraceInfo($"Watching {folderName}\nfor file with extension: {filter}");
                //ToDo: chang this for task process
                Console.WriteLine("Press 'q' to quit the Folder Watcher.");
                while (Console.Read() != 'q') ;
            }
        }

        static async void OnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                string resultsFolderName = Configuration.Instance.Watch_TempResultsFolderTemplate.Replace("{guid}", Guid.NewGuid().ToString());

                Logger.TraceInfo($"Processing {e.ChangeType} File: {e.FullPath}\n\tAWSTextractClaimClient<ADAClaim> results to: {resultsFolderName}");

                System.Threading.Thread.Sleep(1000);  // protect from FileSystemWatcher OnCreate early event problem

                AWSTextractClaimClient<ADAClaim> client = new AWSTextractClaimClient<ADAClaim>(resultsFolderName);

                OCRResultMetaData metaData;

                if (UploadSource)
                {
                    metaData = await client.ProcessImageAsync(e.FullPath);
                    //Task<OCRResultMetaData> metaDataT = client.ProcessImageAsync(e.FullPath);
                    //metaData = metaDataT.Result; // force wait
                }
                else
                {
                    Logger.TraceInfo($"Configuration AWS_S3_UploadSource: {UploadSource}.  ReRun existing S3 source file.");
                    metaData = client.AnalyzeImage(e.FullPath);
                }

                string resultFileName = Configuration.Instance.Watch_ResultsFolderTemplate.Replace("{KeyName}", client.KeyName);

                Logger.TraceInfo($"Compressing: {client.KeyName}");

                FileTransfer.CompressOCRResults(resultsFolderName, resultFileName, e.FullPath, true);

                ArchiveSource(e.FullPath);
                //string archiveImageFn = Path.Combine(ProcessedFolder, Path.GetFileName(e.FullPath));

                //if (File.Exists(archiveImageFn))
                //    File.Delete(archiveImageFn);

                //File.Move(e.FullPath, archiveImageFn);
            }
            catch (Exception ex)
            {
                Logger.TraceInfo("OnCreate Ex");

                string fn = Path.Combine(ProcessedErrorFolder, Path.GetFileName(e.FullPath));
                if (File.Exists(fn)) File.Delete(fn);
                File.Move(e.FullPath, fn);

                if (ex.Message == "Cannot create a file when that file already exists.\r\n")
                    Logger.TraceInfo($"Overwrite: False, Writing results file to an alternate location: {fn}");
                else
                    Logger.TraceWarning(ex.ToString());
            }
        }

        static void ArchiveSource(string fileName)
        {
            try
            {
                string archiveImageFn = Path.Combine(ProcessedFolder, Path.GetFileName(fileName));

                if (File.Exists(archiveImageFn))
                    File.Delete(archiveImageFn);

                File.Move(fileName, archiveImageFn);
            }
            catch (Exception ex)
            {
                Logger.TraceWarning($"ArchiveSource: {ex.ToString()}");

                //throw;
            }
        }

        static void OnChanged(object source, FileSystemEventArgs e) =>
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

        static void OnRenamed(object source, RenamedEventArgs e) =>
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
    }
}