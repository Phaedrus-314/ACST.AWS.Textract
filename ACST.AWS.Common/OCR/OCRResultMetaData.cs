
namespace ACST.AWS.Common.OCR
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using ACST.AWS.Common;

    public class OCRResultMetaData
    {
        #region Fields & Properties
        
        [XmlIgnore]
        public string SourceFileName { get; set; }

        public bool ClaimIsValid { get; set; }

        public string ClaimNo { get; set; }

        public TextractClaimType ClaimType { get; set; }

        public ClaimFormType FormType { get; set; }

        public TextractImageType ImageType { get { return GetImageType(); } set { } }

        public string TextractClaimFileName { get; set; }
        
        public string MatchedFieldHtmlFileName { get; set; }

        public string NamedCoordinatesFileName { get; set; }

        public string ImageFileName { get; set; }

        // ToBe: Removed
        public string TextractJsonResultFileName_Page { get; set; }

        public string TextractJsonResultFileName { get; set; }

        public string TextractXmlResultFileName { get; set; }        

        public string WorkingFolder { get; set; }
        #endregion

        TextractImageType GetImageType()
        {
            string ext = Path.GetExtension(this.ImageFileName);

            bool f = Enum.TryParse(ext.TrimStart('.').ToUpper(), out TextractImageType textractImageType);

            return f ? textractImageType : TextractImageType.Unknown;
        }

        public string RootedPath(string fileName)
        {
            return Path.Combine(this.WorkingFolder, fileName);
        }

        public static string GenerateUniqueWorkingFolder(string folderName, string fileName)
        {
            string uniqFn = Path.GetFileNameWithoutExtension(fileName);

            return Path.Combine(folderName, uniqFn);
        }
    }

}
