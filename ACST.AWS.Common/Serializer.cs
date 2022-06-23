
namespace ACST.AWS.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml;
    using Newtonsoft.Json;

    public static class Serializer
    {

        public static T DeseralizeFromXml<T>(string xml, Type[] extraTypes = null)
        {
            var serializer = new XmlSerializer(typeof(T), extraTypes);
            T result;

            using (TextReader reader = new StringReader(xml))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        public static string SerializeToXML<T>(T obj, Type[] extraTypes = null)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType(), extraTypes);
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
        }

        public static string SerializeToXML<T>(List<T> obj, Type[] extraTypes = null)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType(), extraTypes);
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
        }

        //public static List<T> DeserializeFromJsonFile<T>(string fileName, bool OverrideIgnoreAttribute = false)
        //{
        //    JsonSerializerSettings settings = new JsonSerializerSettings();
        //    if (OverrideIgnoreAttribute)
        //        settings.ContractResolver = new IgnoreJsonAttributesResolver();

        //    return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(fileName), settings);
        //}

        public static T DeserializeFromJsonFile<T>(string fileName, bool OverrideIgnoreAttribute = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            
            if (OverrideIgnoreAttribute)
                settings.ContractResolver = new IgnoreJsonAttributesResolver();

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName), settings);
        }

        public static T DeserializeFromXmlFile<T>(string fileName)
        {
            T ret;
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader(fileName))
            {
                object obj = deserializer.Deserialize(reader);
                ret = (T)obj;
            }

            return ret;
        }

        public static void SerializeToJSON<T>(List<T> items, string destFileName, bool OverrideIgnoreAttribute = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (OverrideIgnoreAttribute)
                settings.ContractResolver = new IgnoreJsonAttributesResolver();

            settings.Formatting = Newtonsoft.Json.Formatting.Indented;

            string jsonString = JsonConvert.SerializeObject(items, settings);
            File.WriteAllText(destFileName, jsonString);

            //string jsonString = JsonConvert.SerializeObject(pages);
            //File.WriteAllText(destFile, jsonString);
        }

        public static void SerializeToJSON<T>(T item, string destFileName, bool OverrideIgnoreAttribute = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (OverrideIgnoreAttribute)
                settings.ContractResolver = new IgnoreJsonAttributesResolver();

            settings.Formatting = Newtonsoft.Json.Formatting.Indented;

            string jsonString = JsonConvert.SerializeObject(item, settings);
            File.WriteAllText(destFileName, jsonString);
        }

        public static void SerializeToXML<T>(List<T> items, string destFileName, Type[] extraTypes = null)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<T>), extraTypes);
            
            // To write to a file, create a StreamWriter object.  
            using (StreamWriter writer = new StreamWriter(destFileName))
            {
                mySerializer.Serialize(writer, items);
            }
        }

        public static void SerializeToXML<T>(T item, string destFileName, Type[] extraTypes = null)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T), extraTypes);

            // To write to a file, create a StreamWriter object.  
            using (StreamWriter writer = new StreamWriter(destFileName))
            {
                mySerializer.Serialize(writer, item);
            }
        }

        public static void TransformToHTML(string fileName, string destFileName, string xsltFile)
        {
            XPathDocument myXPathDoc = new XPathDocument(fileName);
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(xsltFile);
            using (XmlTextWriter myWriter = new XmlTextWriter(destFileName, null))
            {
                myXslTrans.Transform(myXPathDoc, null, myWriter);
            }
        }
    }
}
