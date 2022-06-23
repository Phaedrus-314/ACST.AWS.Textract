
namespace ACST.AWS.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Linq;

    //[DebuggerStepThrough()]
    [Obsolete("Replaced by FormattedPathName")]
    public class FormattedFileName
    {
        public static string TimeStampFormat(string formattedFileName)
        {
            string FileName = Path.GetFileName(formattedFileName);
            //string FilePath = Path.GetDirectoryName(formattedFileName);

            // Collect pattern: all text between braces in the filename
            string FormatStr = Regex.Match(FileName, @"{([^}]*)}").Groups[1].Value;

            // Build new file name by replacing pattern with current datetime, based on format pattern supplied 
            FileName = FileName.Replace("{" + FormatStr + "}", DateTime.Now.ToString(FormatStr));

            // remove any invalid characters from filename
            if (FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                FileName = r.Replace(FileName, "");
            }

            return FileName;
        }
    }
}
