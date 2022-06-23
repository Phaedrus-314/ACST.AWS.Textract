
namespace ACST.AWS.TextractClaimMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ACST.AWS.TransferUtility;
    using ACST.AWS.Common;
    using System.Reflection;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.Textract.Model;

    using FuzzySharp;
    using USAddress;
    using NameParser;

    public static class TestSpeculativeLocal
    {
        private static readonly USAddress.AddressParser Parser = USAddress.AddressParser.Default;

        public static void LocalTest()
        {
            //LocalTextractLinqTests();

            TestRegExAddress();
        }

        public static void LocalTextractLinqTests()
        {
            //// Create TextractAnalysis raw JSON output from uploaded source
            //RawS3Analysis("Dental Claims 0828201024_43", @"c:\temp\RawTetract.json");

            LinqTests(@"c:\temp\RawTetract.json");

            TestWorkingLocal.SimulateADAtextractClient(@"C:\BTS Folders\Dental\Working\Review\Dental Claims 0828201024_43.zip");

            //DetermineADAversion();

            //GenerateNamedCoordinateFromField();



            //RowOrder();

            //FuzzyTests();
        }

        //static void FuzzyTests()
        //{
        //    //string testA1 = "23. Patent ID/Account # (Assigned by Dentist)";
        //    //string testA2 = "23. Patent ID/Account # (Assigned by Derifist";

        //    //string testB1 = "20. Patent ID/Account # (Assigned by Dentist)";
        //    //string testB2 = "2O. Patent ID/Account # (Assigned by Dentist)";

        //    //string testC1 = "23. Patent ID/Account # (Assigned by Dentist)";
        //    //string testC2 = "23 Patent ID/Account # (Assigned by Dentist)";

        //    //int simpleResult = Fuzz.Ratio(testA1, testA2);

        //    //int weightedResult = Fuzz.WeightedRatio(testA1, testA2);

        //    //Console.WriteLine($"{testA1}{Environment.NewLine}{testA2}");
        //    //Console.WriteLine($"SimpleRatio: {Fuzz.Ratio(testA1, testA2)}");
        //    //Console.WriteLine($"WeightedRatio: {Fuzz.WeightedRatio(testA1, testA2)}");
        //    //Console.WriteLine(Environment.NewLine);

        //    //Console.WriteLine($"{testB1}{Environment.NewLine}{testB2}");
        //    //Console.WriteLine($"SimpleRatio: {Fuzz.Ratio(testB1, testB2)}");
        //    //Console.WriteLine($"WeightedRatio: {Fuzz.WeightedRatio(testB1, testB2)}");
        //    //Console.WriteLine(Environment.NewLine);

        //    //Console.WriteLine($"{testC1}{Environment.NewLine}{testC2}");
        //    //Console.WriteLine($"SimpleRatio: {Fuzz.Ratio(testC1, testC2)}");
        //    //Console.WriteLine($"WeightedRatio: {Fuzz.WeightedRatio(testC1, testC2)}");
        //    //Console.WriteLine(Environment.NewLine);

        //    //return;

        //    string fn = @"C:\temp\dental\_Archive\Dental Claims 060820 1.zip";

        //    var nc = new ADA.ADANamedCoordinates(@".\NamedCoordinatesData\NamedCoordinates_ADA2019.xml");

        //    AWSTextractClaimCache<ADA.ADAClaim> cache
        //        = new AWSTextractClaimCache<ADA.ADAClaim>(fn);
        //    cache.BuildTextractClaim(nc);

        //    //// 21. & 22.
        //    //var query = from coords in nc
        //    //            join field in cache.TextractDocument.Pages[0].Form.Fields
        //    //                //on coords.HeaderText equals field.Key.HeaderText
        //    //                on coords.HeaderText equals field.Key.Text
        //    //            select new { KeyText = field.Key.Text, CoordHeader = coords.HeaderText };

        //    //foreach (var q in query)
        //    //    Console.WriteLine($"{q.KeyText} : {q.CoordHeader}");



        //    // ----------------------------------------------------
        //    //var query = from coords in nc
        //    //            join field in cache.TextractDocument.Pages[0].Form.Fields on coords.ExactTextMatch equals field.Key.Text
        //    //            select new { KeyText = field.Key.Text, Exact = coords.ExactTextMatch };

        //    //foreach (var q in query)
        //    //{
        //    //    Console.WriteLine($"{q.KeyText} : {q.Exact}");
        //    //}

        //    //var e = nc.Select(t => t.ExactTextMatch);

        //    //var r = cache.TextractDocument.Pages[0].Form.Fields.Select(f => new { key = f.Key.Text, header = f.Key.HeaderText });

        //    //r.ToList().ForEach(t => Console.WriteLine($"{t.key} : {t.header}"));
        //}

        static void RawS3Analysis(string s3FileName, string outputFn)
        {
            var a = new Textract.Services.AWSTextractClient();
            a.RawS3Analysis(s3FileName, outputFn, new string[] { "FORMS", "TABLES" });
        }

        static void TestRegExAddress()
        {
            string addr = "SUMMAR DRIVE JACKSONT, TN 383013930";

            AddressParseResult parsedAddr1 = Parser.ParseAddress("111 SUMMAR DRIVE JACKSONT, TN 383013930", false);

            AddressParseResult parsedAddr2 = Parser.ParseAddress("SUMMAR DRIVE JACKSON, TN 383013930", false);

            AddressParseResult parsedAddr3 = Parser.ParseAddress("SUMMAR DRIVE JACKSONT, TN 38301", false);

            AddressParseResult parsedAddr4 = Parser.ParseAddress("SUMMAR DRIVE JACKSON, TN 38301", false);
        }

        static void LinqTests(string fn)
        {

            //string fn = @"C:\BTS Folders\Dental\Working\Review\Dental Claims 0828201024_43.zip";

            var nc = new ADA.ADANamedCoordinates(@"C:\BTS Folders\Dental\NamedCoordinates\NamedCoordinates_ADA2012.xml");

            AWSTextractClaimCache<ADA.ADAClaim> cache
                = new AWSTextractClaimCache<ADA.ADAClaim>();

            string fnMeta = @"Dental Claims 0828201024_43_MetaData.xml";

            cache.AnalyseLocalDocument(fn, nc, true);

            cache.BuildTextractClaim(nc);
            //cache.BuildTextractClaim();
            //cache.OpenTextractClaim();

            //var r = cache.TextractDocument.GetBlockById("5074c2e0-bd89-4e80-9105-24e11de5ff32");
            //var r = cache.TextractDocument.GetBlockById("d1b2dbd4-f4e9-4f4f-b77e-b2dfaf59f78e");
            //var r = cache.TextractDocument.GetBlockById("08384650-8876-42fa-8c57-22b3e9766105");
            
            //var d = cache.TextractDocument.ResponsePages.SelectMany(b => b.Blocks)
            //    .Where(b => b.BlockType == "PAGE").ToList();

            //.ForEach(b =>
            //{
            //    Console.WriteLine($"{b.BlockType}");
            //    if (b.BlockType == "PAGE")
            //    {
            //        Console.WriteLine($"{b.Geometry.BoundingBox}");
            //    }
            //});

            //[0].Blocks
            //.ForEach(b =>
            //{
            //    Console.WriteLine($"{b.BlockType}");
            //    if (b.BlockType == "PAGE")
            //    {
            //        Console.WriteLine($"{b.Geometry.BoundingBox}");
            //    }
            //});


        }

        //static void GenerateNamedCoordinateFromField()
        //{
        //    string fn = @"C:\temp\dental\_Archive\Dental Claims 060820 1.zip";

        //    AWSTextractClaimCache<ADA.ADAClaim> cache
        //        = new AWSTextractClaimCache<ADA.ADAClaim>(fn);

        //    cache.OpenTextractClaim();

        //    Field field = cache.Page
        //        .Form.Fields.Where(f => f.Key.Text.StartsWith("15.")).SingleOrDefault();

        //    var n = new NamedCoordinate 
        //    { 
        //        ExactTextMatch = field.Key.Text, 
        //        FieldBoundingBox = field.Key.Geometry.BoundingBox,
        //        ValueBoundingBox = field.Value.Geometry.BoundingBox,
        //        IdealCenterKey = NewGeometry.IdealCenter(field.Key.Geometry.BoundingBox),
        //        IdealCenterValue = NewGeometry.IdealCenter(field.Value.Geometry.BoundingBox),
        //        GroupName = "groupName",
        //        Name = "name"
        //    };

        //    string x = Serializer.SerializeToXML<NamedCoordinate>(n);

        //}

        //static void DetermineADAversion()
        //{
        //    //string fn = @"C:\temp\dental\_Archive\Dental Claims 060820 15.zip";
        //    string fn = @"C:\temp\dental\_Archive\Dental Claims 060820 15.zip";

        //    // Load from compressed TextractClaimarchive
        //    AWSTextractClaimCache<ADA.ADAClaim> cache
        //        = new AWSTextractClaimCache<ADA.ADAClaim>(fn);

        //    //var nc = new TestOCRSerializer.ADA.ADANamedCoordinates(@".\NamedCoordinatesData\ADANamedCoordinates_2.xml");

        //    //cache.BuildTextractClaim();
        //    cache.OpenTextractClaim();

        //    //cache.BuildTextractClaim(nc);

        //    // ADA American Dental Association' Dental Claim Form
        //    // Dental Claim Form
        //    //bool f1 = cache.TextractDocument.ResponsePages.FirstOrDefault()
        //    //    .Blocks.Any(b => b.BlockType == "Line" & b.Text == "Dental Claim Form");

        //    //bool f2 = cache.TextractDocument.ResponsePages.FirstOrDefault()
        //    //   .Blocks.Any(b => b.BlockType == "Line" & b.Text == "ADA American Dental Association Dental Claim Form");


        //    //bool f3 = cache.TextractDocument.ResponsePages.FirstOrDefault()
        //    //    .Blocks.Any(b => b.BlockType.Value == "Line" 
        //    //    & (b.Text.IsNotNullOrWhiteSpace() && b.Text.StartsWith("ADA American Dental Association")));


        //    //var r = cache.Page.Lines.Where(l => l.Text == "").SingleOrDefault();


        //    //var x1 = cache.TextractDocument.Pages[0]
        //    //    .GetCellsByRowCoordinate(new System.Drawing.PointF() { X = 0.039F, Y = 0.43F });


        //    var fh = FormHeader(cache.TextractDocument);
        //    var t = ParseClaimFormHeader(fh);
        //    var n = NamedCoordinatesFileNameByFormType(t);
        //}

        static void RowOrder()
        {
            string fn = @"C:\temp\dental\_Archive\Dental Claims 060820 1.zip"; ;
            //var nc = new TestOCRSerializer.ADA.ADANamedCoordinates(@".\NamedCoordinatesData\ADANamedCoordinates_2.xml");

            AWSTextractClaimCache<ADA.ADAClaim> cache
                = new AWSTextractClaimCache<ADA.ADAClaim>(fn);
            //cache.BuildTextractClaim(nc);
            cache.BuildTextractClaim();

            var ttt = cache.Page.Form.Fields.Where(f => f.TabOrder != 0);

            var xx1 = cache.Page.Form.GetFieldById("cbaef3bc-bf00-4ebe-a443-9bebeb3a6b24");
            var xx2 = cache.Page.Form.GetFieldById("3357a935-7b20-492c-ab3f-0c10571163b8");
            var xx3 = cache.Page.Form.GetFieldById("ec1b9ca6-997e-4981-9868-2d72308a47c2");

            var xx = cache.Page.Form.GetFieldByKey("13. Date of Birth (MM/DD/CCYY)");

            var Headers2 = cache.Page.Form.Fields?.Where(m => m.TabOrder != 0).OrderBy(m => m.TabOrder).Select(m => m.Key);

            var fld = cache.Page.Form.Fields
                    .Where(f => f.Id == "b5dd35cf-12db-4553-912b-19fdd722c8ba")
                    ;

            //cache.Page.Form.Fields.ToList().ForEach(f => { Console.WriteLine($"{f.Key.Text}"); });

            //var Headers2 = cache.Page.Form.Fields?.Select(m => m.Key.HeaderText);

            //var Headers3 = cache.Page.Form.MatchedFields?.Select(m => m.Key.HeaderText);
        }
        //static string FormHeader(TextractDocument t)
        //{
        //    int lineIndex = Configuration.Instance.ADAForm_Header_LineIndex;
        //    string searchTerm = Configuration.Instance.ADAForm_Header_CommonSearchTerm;

        //    var lineArray = t.ResponsePages.FirstOrDefault().Blocks.Where(b => b.BlockType == "Line").ToArray();

        //    var headerText = lineArray[lineIndex]?.Text;

        //    return headerText;
        //}

        static string NamedCoordinatesFileNameByFormType(ClaimFormType type)
        {
            return Configuration.Instance.NamedCoordinates_FileName_Template.Replace("{FormType}", type.ToString());
        }

        static ClaimFormType ParseClaimFormHeader(string header)
        {
            //ToDo: configure or remove this... this is a bad way of doing this...
            ClaimFormType cft = ClaimFormType.Unknown;

            if (header == "Dental Claim Form")
                cft = ClaimFormType.ADA2006;

            else if (header == "ADA American Dental Association Dental Claim Form")
                cft = ClaimFormType.ADA2012;

            else if (header.StartsWith("ADA American Dental"))
                cft = ClaimFormType.ADA2012;

            //ClaimFormType cft;
            //bool f = Enum.TryParse(header, out cft);
            //if (f)
            //    return cft;
            //else
            //    return ClaimFormType.Unknown;

            return cft;
        }

        public static void LocalMappingTests() 
        {
            //ADA.ADAClaim claim = Serializer.DeserializeFromXmlFile<ADA.ADAClaim>(@"C:\temp\dental\dental_20200709_010348\Dental Claims 060820 1_Claim.xml");

            //Type typeType = typeof(ADA.ADAClaim);
            //Type typeInst = claim.GetType();

            //var f = typeType == typeInst;

            //GetPropertiesByCustomAttribute(typeType, typeof(CompositeValueAttribute), "Subscriber_NameAndAddr");
            
            TestSpeculativeLocal.OCRMapperPropertyInfo_Tests();
        }

        //static void GetPropertiesByCustomAttribute<A>(Type type, A attribute, string attributeName = null)
        //{
        //    Logger.TraceInfo(Environment.NewLine);

        //    var properties = from p in type.GetProperties()
        //                          .Select(p => GetCustomProperty(type, p.Name))
        //                          .Where(p => p != null & Attribute.IsDefined(p, typeof(A)))
        //                     from c in p.GetCustomAttributes(true)
        //                     where ((A)c).Name == attributeName
        //                     select p;

        //    foreach (var property in properties)
        //    {
        //        Logger.TraceInfo($"Select Attribute.....: {property.Name}");
        //    }
        //}


        static void OCRMapperPropertyInfo_Tests()
        {
            //// Working examples::
            //var m = Mapper.GetMappedProperties(typeof(ADA.ADAClaim), typeof(OCR.CompositeValueAttribute));

            //var pe = Mapper.GetMappedProperties(typeof(ADA.ADAClaim), typeof(OCR.CompositeValueAttribute))
            //               .Where(p => ((IMappedPropertyAttribute)p).Name == "Patient_NameAndAddr");

            //var cp = Mapper.FindPropertiesByCompositeAttributeName(typeof(ADA.ADAClaim)
            //    , "TreatingDentist_Addr");

            //var gp = Mapper.FindPropertiesByGroupFieldAttributeName(typeof(ADA.ADAClaim)
            //    , "SubscriberGender");

            //var pp = Mapper.FindRequiredProperties(typeof(ADA.ADAClaim));

            //var pxp2 = Mapper.FindCompositeProperties(typeof(ADA.ADAClaim), "TreatingDentist_Addr");
        }

        //static void tt()
        //{
        //    ContextData FieldContextData = ContextData.GetContext(this.TextractClaimImage, new PointF { X = identPos.X, Y = identPos.Y }, this.SelectedTextractElements);
        //}

        static IEnumerable<PropertyInfo> GetPropertiesByCustomAttribute(Type type, Type attribute, string attributeName = null)
        {
            //Logger.TraceInfo(Environment.NewLine);

            var properties = from p in type.GetProperties()
                                  .Select(p => GetCustomProperty(type, p.Name))
                                  .Where(p => p != null & Attribute.IsDefined(p, attribute))
                             from c in p.GetCustomAttributes(true)
                             where ((CompositeValueAttribute)c).Name == attributeName
                             select p;

            //foreach (var property in properties)
            //{
            //    Logger.TraceInfo($"Select Attribute.....: {property.Name}");
            //}

            return properties;
        }


        static IEnumerable<PropertyInfo> GetCustomProperties(Type type, Type attribute, string attributeName = null)
        {
            return type
              .GetProperties()
              .Select(p => GetCustomProperty(type, p.Name))
              //.Where(p => p != null & Attribute.IsDefined(p, typeof(MappedPropertyAttribute)));
              .Where(p => p != null & Attribute.IsDefined(p, attribute));
        }

        static PropertyInfo GetCustomProperty(Type type, string name)
        {
            if (type == null)
                return null;

            var prop = type.GetProperty(name);

            if (prop.DeclaringType == type)
                return prop;
            else
                return GetCustomProperty(type.BaseType, name);
        }


        public static IEnumerable<Tuple<string, string>> DehydrateObject(Type type)
        {

            IEnumerable<Tuple<string, string>> values =
                (from property in type.GetType().GetProperties()
                 where property.PropertyType == typeof(string)
                 //&&
                 //                                property.CanRead &&
                 //                                property.CanWrite
                 select new Tuple<string, string>(property.Name,
                                       (string)property.GetValue(type)));

            return values;

        }

        public static PropertyInfo FindProperty(this Type type, string propertyName)
        {

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                object[] mappedAttributes = propertyInfo.GetCustomAttributes(typeof(MappedPropertyAttribute), false);

                foreach (MappedPropertyAttribute attribute in mappedAttributes)
                {
                    if (attribute.Name == propertyName)
                    {
                        return propertyInfo;
                    }
                }

            }
            return null;
        }

    }
}
