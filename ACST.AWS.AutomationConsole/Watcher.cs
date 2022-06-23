
namespace ACST.AWS.AutomationConsole
{
    using System;
    using System.IO;
    using System.Security.Permissions;

    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.TransferUtility;
    using ACST.AWS.TextractClaimMapper.ADA;
    using ACST.AWS.TextractClaimMapper;

    public class Watcher
    {
        #region Properties

        static string ProcessedFolderWithReview => Configuration.Instance.Watch_ProcessedFolder_WithReview;

        static string ProcessedFolderWithoutReview => Configuration.Instance.Watch_ProcessedFolder_WithoutReview;

        static string ProcessedFolderError => Configuration.Instance.Watch_ProcessedFolder_Error;

        static string ProcessedFolderSkip => Configuration.Instance.Watch_ProcessedFolder_Skip;

        public static bool UploadSource => Configuration.Instance.AWS_S3_UploadSource;
        //public static bool UploadSource => false;

        public static bool OverwriteResults = true;
        #endregion

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        static void ProcessExistingFiles(string folderName, string filter)
        {
            DirectoryInfo d = new DirectoryInfo(folderName);

            FileInfo[] Files = d.GetFiles(filter); //Getting Text files

            foreach (FileInfo file in Files)
            {
                FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.Changed, file.DirectoryName, file.Name);
                ProcessFile(e);
                //str = str + ", " + file.Name;
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run(string folderName, string filter)
        {
            // Process existing files
            ProcessExistingFiles(folderName, filter);

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

                Logger.TraceInfo($"Watching {folderName}\nFor file with extension: {filter}{Environment.NewLine}");

                //ToDo: chang this for task schedule process & automationConsole 
                Console.WriteLine($"Press 'q' to quit the Folder Watcher.");
                while (Console.Read() != 'q') ;
            }
        }

        static async void OnCreated(object source, FileSystemEventArgs e)
        {
            ProcessFile(e);
            //try
            //{
            //    string resultsFolderName = Configuration.Instance.Watch_TempResultsFolderTemplate.Replace("{guid}", Guid.NewGuid().ToString());

            //    //Logger.TraceInfo($"Processing {e.ChangeType} File: {e.Name}");
            //    Logger.TraceInfo($"{e.Name}\tProcessing {e.ChangeType} File");
            //    Logger.TraceInfo($"{e.Name}\tSend AWSTextractClaimClient<ADAClaim> results to: {resultsFolderName}");

            //    FileInfo fileInfo = new FileInfo(e.FullPath);
            //    while (IsFileLocked(fileInfo))
            //        System.Threading.Thread.Sleep(250);

            //    AWSTextractClaimClient<ADAClaim> client = new AWSTextractClaimClient<ADAClaim>(resultsFolderName);

            //    OCRResultMetaData metaData;

            //    if (client.SkipImage(e.FullPath))
            //    {
            //        Logger.TraceInfo("Skip Image =========");
            //        ArchiveSource(e.FullPath, ArchiveType.Skip);
            //    }
            //    else
            //    {

            //        if (UploadSource)
            //        {
            //            metaData = await client.ProcessImageAsync(e.FullPath);
            //        }
            //        else
            //        {
            //            Logger.TraceInfo("UPLOAD TURNED OFF =========");
            //            Logger.TraceInfo("UPLOAD TURNED OFF =========");
            //            Logger.TraceInfo("UPLOAD TURNED OFF =========");
            //            Logger.TraceInfo($"{client.KeyName}\tConfiguration AWS_S3_UploadSource: {UploadSource}.  ReRun existing S3 source file.");
            //            metaData = client.AnalyzeImage(e.FullPath);
            //        }

            //        string resultFileName = Configuration.Instance.Watch_ResultsFolderTemplate.Replace("{KeyName}", client.KeyName);

            //        Logger.TraceInfo($"{client.KeyName}\tCompressing");

            //        FileTransfer.CompressOCRResults(resultsFolderName, resultFileName, e.FullPath, true);

            //        ArchiveSource(e.FullPath, metaData.ClaimIsValid ? ArchiveType.Success : ArchiveType.Review);

            //        Logger.TraceInfo($"{e.Name}\tProcess complete");

            //        if (!UploadSource)
            //        {
            //            Logger.TraceInfo("UPLOAD TURNED OFF =========");
            //            Logger.TraceInfo("UPLOAD TURNED OFF =========");
            //            Logger.TraceInfo("UPLOAD TURNED OFF =========");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string fn = Path.Combine(ProcessedFolderError, Path.GetFileName(e.FullPath));
            //    if (File.Exists(fn)) File.Delete(fn);
            //    File.Move(e.FullPath, fn);

            //    if (ex.Message == "Cannot create a file when that file already exists.\r\n")
            //        Logger.TraceInfo($"Overwrite: False, Writing results file to an alternate location: {fn}");
            //    else
            //        Logger.TraceWarning(ex.ToString());
            //}
            //finally
            //{
            //    //Logger.TraceInfo($""); //{Environment.NewLine}
            //    Logger.TraceFlush();
            //}
        }

        //static void ArchiveSource(string fileName, bool forReview = true)
        //{
        //    try
        //    {
        //        string processFolder = forReview ? ProcessedFolderWithReview : ProcessedFolderWithoutReview;

        //        string archiveImageFn = Path.Combine(processFolder, Path.GetFileName(fileName));

        //        if (File.Exists(archiveImageFn))
        //            File.Delete(archiveImageFn);

        //        File.Move(fileName, archiveImageFn);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.TraceWarning($"Warning ArchiveSource: {ex.ToString()}");

        //        //throw;
        //    }
        //}

        static void OnChanged(object source, FileSystemEventArgs e) =>
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

        static void OnRenamed(object source, RenamedEventArgs e) =>
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");

        static async void ProcessFile(FileSystemEventArgs e)
        {
            try
            {
                string resultsFolderName = Configuration.Instance.Watch_TempResultsFolderTemplate.Replace("{guid}", Guid.NewGuid().ToString());

                //Logger.TraceInfo($"Processing {e.ChangeType} File: {e.Name}");
                Logger.TraceInfo($"{e.Name}\tProcessing {e.ChangeType} File");
                Logger.TraceInfo($"{e.Name}\tSend AWSTextractClaimClient<ADAClaim> results to: {resultsFolderName}");

                FileInfo fileInfo = new FileInfo(e.FullPath);
                while (IsFileLocked(fileInfo))
                    System.Threading.Thread.Sleep(250);

                AWSTextractClaimClient<ADAClaim> client = new AWSTextractClaimClient<ADAClaim>(resultsFolderName);

                OCRResultMetaData metaData;

                if (client.SkipImage(e.FullPath))
                {
                    Logger.TraceInfo("Skip Image =========");
                    ArchiveSource(e.FullPath, ArchiveType.Skip);
                }
                else
                {

                    if (UploadSource)
                    {
                        metaData = await client.ProcessImageAsync(e.FullPath);
                    }
                    else
                    {
                        Logger.TraceInfo("UPLOAD TURNED OFF =========");
                        Logger.TraceInfo("UPLOAD TURNED OFF =========");
                        Logger.TraceInfo("UPLOAD TURNED OFF =========");
                        Logger.TraceInfo($"{client.KeyName}\tConfiguration AWS_S3_UploadSource: {UploadSource}.  ReRun existing S3 source file.");
                        metaData = client.AnalyzeImage(e.FullPath);
                    }

                    string resultFileName = Configuration.Instance.Watch_ResultsFolderTemplate.Replace("{KeyName}", client.KeyName);

                    Logger.TraceInfo($"{client.KeyName}\tCompressing");

                    FileTransfer.CompressOCRResults(resultsFolderName, resultFileName, e.FullPath, true);

                    ArchiveSource(e.FullPath, metaData.ClaimIsValid ? ArchiveType.Success : ArchiveType.Review);

                    Logger.TraceInfo($"{e.Name}\tProcess complete");

                    if (!UploadSource)
                    {
                        Logger.TraceInfo("UPLOAD TURNED OFF =========");
                        Logger.TraceInfo("UPLOAD TURNED OFF =========");
                        Logger.TraceInfo("UPLOAD TURNED OFF =========");
                    }
                }
            }
            catch (Exception ex)
            {
                string fn = Path.Combine(ProcessedFolderError, Path.GetFileName(e.FullPath));
                if (File.Exists(fn)) File.Delete(fn);
                File.Move(e.FullPath, fn);

                if (ex.Message == "Cannot create a file when that file already exists.\r\n")
                    Logger.TraceInfo($"Overwrite: False, Writing results file to an alternate location: {fn}");
                else
                    Logger.TraceWarning(ex.ToString());
            }
            finally
            {
                //Logger.TraceInfo($""); //{Environment.NewLine}
                Logger.TraceFlush();
            }
        }

        static void ArchiveSource(string fileName, ArchiveType archiveType = ArchiveType.Review)
        {
            try
            {
                string processFolder;

                switch (archiveType)
                {
                    case ArchiveType.Error:
                        processFolder = ProcessedFolderError;
                        break;
                    case ArchiveType.Review:
                        processFolder = ProcessedFolderWithReview;
                        break;
                    case ArchiveType.Skip:
                        processFolder = ProcessedFolderSkip;
                        break;
                    case ArchiveType.Success:
                        processFolder = ProcessedFolderWithoutReview;
                        break;
                    default:
                        processFolder = ProcessedFolderError;
                        break;
                }

                string archiveImageFn = Path.Combine(processFolder, Path.GetFileName(fileName));

                if (File.Exists(archiveImageFn))
                    File.Delete(archiveImageFn);

                File.Move(fileName, archiveImageFn);
            }
            catch (Exception ex)
            {
                Logger.TraceWarning($"Warning ArchiveSource: {ex.ToString()}");

                //throw;
            }
        }

    }
}