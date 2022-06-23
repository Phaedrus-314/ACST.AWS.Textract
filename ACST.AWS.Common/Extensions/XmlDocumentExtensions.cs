
namespace ACST.AWS.Common
{
    using System;
    using System.Xml;
    using System.Xml.Linq;
    
    public static class XmlDocumentExtensions
    {
        public static XDocument ToXDocument(this XmlDocument document)
        {
            return document.ToXDocument(LoadOptions.None);
        }

        public static XDocument ToXDocument(this XmlDocument document, LoadOptions options)
        {
            using (XmlNodeReader reader = new XmlNodeReader(document))
            {
                return XDocument.Load(reader, options);
            }
        }
    }
}
