

namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using FuzzySharp;
    using ACST.AWS.Common;
    using System.Xml.Linq;

    public class NamedCoordinates
         : List<NamedCoordinate>
        //, INamedCoordinates
    {
        #region Fields and Properties
        
        const int MinumumScore = 25;
        const float MaximumDistance = .05f;  

        public NamedCoordinatesSource Source { get; protected set; }

        public virtual string ConfigurationFileName { get; protected set; }

        public virtual int PageGridRows { get; }

        public virtual int PageGridColumns { get; }

        int _maxTabOrder;
        public int MaxTabOrder
        {
            get
            {
                if (_maxTabOrder == -1)
                    _maxTabOrder = this.Max(m => m.TabOrder);
                return _maxTabOrder;
            }
        }

        int _minTabOrder;
        public int MinTabOrder
        {
            get
            {
                if (_minTabOrder == -1)
                    _minTabOrder = this.Where(w => !w.DoNotMap).Min(m => m.TabOrder);
                return _minTabOrder;
            }
        }

        public virtual int TableHeaderGridPosition { get; protected set; }

        public virtual int TableRows { get; }

        public virtual int TableSubRows { get; }
        #endregion

        #region Constructors
        
        public NamedCoordinates() 
        {
            _maxTabOrder = -1;
            _minTabOrder = -1;
        }

        public NamedCoordinates(string configurationFileName) 
            : this()
        {
            DeserializeFromXML(configurationFileName);
        }
        #endregion

        #region Methods
        
        #region Match Methods
        
        public IEnumerable<NamedCoordinate> MatchTableCellByGridCoordinate(NewGeometry geometry)
        {
            return this.Where(pk => pk.Column <= geometry.ReadingOrder.Column
                                & (pk.Column + pk.Width - 1) >= geometry.ReadingOrder.Column
                                & pk.Row <= geometry.ReadingOrder.Row
                                & (pk.Row + (this.TableRows * this.TableSubRows) - 1) >= geometry.ReadingOrder.Row);
        }

        public NamedCoordinate MatchTableByFuzzyText(Cell cell)
        {
            //Todo: consolidate this with FieldKeyMatch
            string log = "";

            var results = this.Where(nc => (nc?.GroupName == "Table" | nc?.GroupName == "TableHeader") && nc?.ExactTextMatch != null )
                    .Select(nc => new {
                        NC = nc, 
                        Score = FuzzyScore(nc?.ExactTextMatch, cell?.Text) 
                    })
                    .Where(x => x?.Score >= x?.NC.MinConfidence
                              & x?.Score > MinumumScore)
                    //.Where(x => x?.Score >= Configuration.Instance.FuzzyMatch_MinimumScore)
                    //.Select(x => x.NC);
                    ;

            foreach (var r in results)
            {
                var n = r.NC?.ExactTextMatch;
                if (n != null)
                    log += $"Fuzzy Score = {r.Score}:\n\tOCR.Field:\t'{cell?.Text}'\n\tNamedCoord:\t'{n}'\n";
            }
            if (log.IsNotNullOrWhiteSpace())
                Logger.TraceVerbose(log.TrimEnd('\n'));

            //return results.Select(s => s.NC);

            if (results.Count() == 0) return null;

            // return NC with max score
            return results.Where(r => r != null).Aggregate((i1, i2) => i1.Score > i2.Score ? i1 : i2).NC;
        }

        public NamedCoordinate MatchByFuzzyHeaderText(FieldKey fieldKey)
        {
            string log = "";

            var results = this.Where(nc => nc?.HeaderText != null)
                                .Select(nc => new {
                                    NC = nc,
                                    Score = FuzzyScore(nc?.HeaderText, fieldKey?.Text),
                                    Distance = DistanceSquared(nc.IdealCenterKey, ((NewGeometry)fieldKey?.Geometry).Center)
                                })
                                //.Where(x => x?.Score >= Configuration.Instance.FuzzyMatch_MinimumScore)
                                .Where(x => x?.Score >= x?.NC.MinConfidence 
                                        & x?.Score > MinumumScore
                                        & x?.Distance < MaximumDistance)
                                .OrderBy(x => x.Distance).Take(5)
                                ;

            if (results.Count() == 0) return null;

            foreach (var r in results)
            {
                var n = r.NC?.HeaderText;
                if (n != null)
                {
                    log += $"Fuzzy Score/Min Conf = {r.Score}/{r.NC.MinConfidence} Distance = {r.Distance}\n\tOCR.Field:\t'{fieldKey?.Text}'\n\tNamedCoord:\t'{n}'\n";
                }
            }

            if (log.IsNotNullOrWhiteSpace())
                Logger.TraceVerbose(log.TrimEnd('\n'));

            // return closest confident NC
            return results.Where(r => r != null)
                //.Aggregate((i1, i2) => (i1.Score >= i2.Score & i1.Distance <= i2.Distance) ? i1 : i2).NC;
                .Aggregate((i1, i2) => (i1.Distance <= i2.Distance) ? i1 : i2).NC;
        }

        public NamedCoordinate MatchByFuzzyExactText(FieldKey fieldKey)
        {
            string log = "";

            var results = this.Where(nc => nc?.HeaderText != null && nc?.ExactTextMatch != null 
                                && nc?.HeaderText != nc?.ExactTextMatch)
                                .Select(nc => new { 
                                    NC = nc, 
                                    Score = Fuzz.Ratio(nc?.ExactTextMatch, fieldKey?.Text),
                                    Distance = DistanceSquared(nc.IdealCenterKey, ((NewGeometry)fieldKey?.Geometry).Center)
                                })
                                //.Where(x => x?.Score >= Configuration.Instance.FuzzyMatch_MinimumScore)
                                .Where(x => x?.Score >= x?.NC.MinConfidence
                                        & x?.Score > MinumumScore
                                        & x?.Distance < MaximumDistance)
                                .OrderBy(x => x.Distance).Take(5)
                                ;

            if (results.Count() == 0) return null;

            foreach (var r in results)
            {
                var n = r.NC?.ExactTextMatch;
                if (n != null)
                    log += $"Fuzzy Score/Min Conf = {r.Score}/{r.NC.MinConfidence} Distance = {r.Distance}\n\tOCR.Field:\t'{fieldKey?.Text}'\n\tNamedCoord:\t'{n}'\n";
            }

            if (log.IsNotNullOrWhiteSpace())
                Logger.TraceVerbose(log.TrimEnd('\n'));

            // return closest confident NC
            return results.Where(r => r != null)
                .Aggregate((i1, i2) => (i1.Distance <= i2.Distance) ? i1 : i2).NC;
        }

        public IEnumerable<NamedCoordinate> MatchByColumnCenter(NewGeometry geometry, string groupName)
        {
            //var nc = this.Where(pk => geometry.Rectangle.Contains(pk.IdealCenterKey));
            var nc = this.Where(pk => pk.GroupName == groupName 
                                        && pk.FieldBoundingBox.SharesColumnWith(geometry.Center));

            var n = nc.FirstOrDefault();

            //string log = $"\tNamedCoord: '{n?.ExactTextMatch}' mapped to:'{n?.Name}'";

            Logger.TraceVerbose($"\tNamedCoord:\t'{n?.ExactTextMatch}'");

            return nc;
        }

        public IEnumerable<NamedCoordinate> MatchCoordinateBoundryAndExactText(MatchedFieldKey matchedFieldKey)
        {
            // Trys to locate Textract FieldValue bounding box actual center within Named Coord Key bounding box
            var nc = this.Where(pk => pk.ValueBoundingBox.ContainsPoint(((NewGeometry)matchedFieldKey.Geometry).Center)
                                & pk.ExactTextMatch == matchedFieldKey.Text
            );

            //OffsetValueBoundingBox
            var n = nc.FirstOrDefault()?.HeaderText;

            string log = $"FieldValue actual center within NamedCoord FieldKey boundry:'{nc.FirstOrDefault()?.HeaderText}'";

            Logger.TraceVerbose(log);

            return nc;
        }

        //public IEnumerable<NamedCoordinate> MatchCoordinateBoundry(Field field)
        //{
        //    if (field.Key == null | field.Value == null)
        //        return null;

        //    // Trys to locate Textract FieldValue bounding box actual center within Named Coord Key bounding box
        //    var nc = this.Where(pk => pk.OffsetValueBoundingBox(field.Key.Geometry.BoundingBox)
        //                                .ContainsPoint(((NewGeometry)field.Value.Geometry).Center)
        //    );

        //    //OffsetValueBoundingBox
        //    var n = nc.FirstOrDefault()?.HeaderText;

        //    string log = $"FieldValue actual center within NamedCoord OffsetFieldValue boundry:'{nc.FirstOrDefault()?.HeaderText}'";

        //    Logger.TraceVerbose(log);

        //    return nc;
        //}

        public IEnumerable<NamedCoordinate> MatchKeyByIdealCenter(NewGeometry geometry)
        {
            // trys to locate Named Coord Key Ideal Center within Textract FieldKey bounding box
            var nc = this.Where(pk => geometry.Rectangle.Contains(pk.IdealCenterKey));

            var n = nc.FirstOrDefault()?.HeaderText;

            string log = $"IdealCenter Match for #NamedCoord:'{n}'";

            Logger.TraceVerbose(log);

            return nc;
        }
        #endregion

        ///// <summary>
        ///// File.ReadAllBytes alternative to avoid read and/or write locking
        ///// </summary>
        //private byte[] ReadAllBytes2(string filePath, FileAccess fileAccess = FileAccess.Read, FileShare shareMode = FileShare.ReadWrite)
        //{
        //    using (var fs = new FileStream(filePath, FileMode.Open, fileAccess, shareMode))
        //    {
        //        using (var ms = new MemoryStream())
        //        {
        //            fs.CopyTo(ms);
        //            return ms.ToArray();
        //        }
        //    }
        //}

        public void DeserializeFromXML(string configurationFileName)
        {
            this.ConfigurationFileName = configurationFileName;
            this.Source = NamedCoordinatesSource.ExternalXML;
            DeserializeFromXML();
        }

        protected void DeserializeFromXML()
        {
            if (this.ConfigurationFileName == null) throw new ArgumentNullException(nameof(this.ConfigurationFileName));
            if (!File.Exists(this.ConfigurationFileName)) throw new FileNotFoundException(this.ConfigurationFileName);
            //if (sourceFile == null) throw new ArgumentNullException(sourceFile);

            var serializer = new XmlSerializer(typeof(List<NamedCoordinate>));

            using (FileStream fileStream = new FileStream(this.ConfigurationFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.AddRange((List<NamedCoordinate>)serializer.Deserialize(fileStream));
            }

            ////ToDo test coordinates to add to XML after testing:
            //this.Add(new NamedCoordinate { Column = 1, Name = "ServiceLine_ProcedureDate",      ExactTextMatch = "24. Procedure Date (MM/DD/CCYY)", GroupName = "Table", IdealCenterKey = new PointF(0.09259243F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.02768238F, Top = 0.0F, Width = 0.129820108F } });
            //this.Add(new NamedCoordinate { Column = 2, Name = "ServiceLine_OralCavityArea",     ExactTextMatch = "25. Area of Oral Cavity", GroupName = "Table", IdealCenterKey = new PointF(0.17587325F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.157502487F, Top = 0.0F, Width = 0.03674153F } });
            //this.Add(new NamedCoordinate { Column = 3, Name = "ServiceLine_ToothSystem",        ExactTextMatch = "26. Tooth System", GroupName = "Table", IdealCenterKey = new PointF(0.03674153F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.194244012F, Top = 0.0F, Width = 0.03674155F } });
            //this.Add(new NamedCoordinate { Column = 4, Name = "ServiceLine_ToothNumbers",       ExactTextMatch = "27. Tooth Number(s) or Letter(s)", GroupName = "Table", IdealCenterKey = new PointF(0.3032439F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.230985567F, Top = 0.0F, Width = 0.144516692F } });
            //this.Add(new NamedCoordinate { Column = 5, Name = "ServiceLine_ToothSurfaces",      ExactTextMatch = "28. Tooth Surface", GroupName = "Table", IdealCenterKey = new PointF(0.4110191F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.375502259F, Top = 0.0F, Width = 0.07103363F } });
            //this.Add(new NamedCoordinate { Column = 6, Name = "ServiceLine_ProcedureCode",      ExactTextMatch = "29. Procedure Code", GroupName = "Table", IdealCenterKey = new PointF(0.4820527F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.4465359F, Top = 0.0F, Width = 0.07103366F } });
            //this.Add(new NamedCoordinate { Column = 7, Name = "ServiceLine_Description",        ExactTextMatch = "30. Description", GroupName = "Table", IdealCenterKey = new PointF(0.707400858F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.517569542F, Top = 0.0F, Width = 0.379662633F } });
            //this.Add(new NamedCoordinate { Column = 8, Name = "ServiceLine_Qty",                ExactTextMatch = "", GroupName = "Table", IdealCenterKey = new PointF(0.9107041F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.8972322F, Top = 0.0F, Width = 0.02694376F } });
            //this.Add(new NamedCoordinate { Column = 9, Name = "ServiceLine_Fee",                ExactTextMatch = "31. Fee", GroupName = "Table", IdealCenterKey = new PointF(0.949895F, .0F), FieldBoundingBox = new Amazon.Textract.Model.BoundingBox { Height = 0.999F, Left = 0.9241759F, Top = 0.0F, Width = 0.05143819F } });
        }

        public void SerializeToXMLFile(string destFile)
        {
            //ToDo: Change to common Serializer & test
            //Serializer.SerializeToXML<NamedCoordinates>(this);
            XmlSerializer mySerializer = new XmlSerializer(typeof(NamedCoordinates));

            using (StreamWriter writer = new StreamWriter(destFile))
            {
                mySerializer.Serialize(writer, this);
            }
        }

        public static string GetFileNameFromFormType(ClaimFormType type)
        {
            return Configuration.Instance.NamedCoordinates_FileName_Template.Replace("{FormType}", type.ToString());
        }

        static float DistanceSquared(PointF point1, PointF point2)
        {
            return ((point1.X - point2.X) * (point1.X - point2.X)
                    + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }

        public static int FuzzyScore(string value, string comparisonValue)
        {
            if (value == null | comparisonValue == null)
                return 0;

            return Fuzz.Ratio(value, comparisonValue);
        }


        public static void UpdateConfidence(ClaimFormType formType, string coordinateName, float minimumConfidence)
        {
            UpdateConfidence(GetFileNameFromFormType(formType), coordinateName, minimumConfidence);
        }

        public static void UpdateConfidence(string configurationFileName, string coordinateName, float minimumConfidence)
        {
            XDocument xmldoc = XDocument.Load(configurationFileName);
            XElement root = xmldoc.Root;

            IEnumerable<XElement> vls = from e in root.Elements("NamedCoordinate") where e.Element("Name").Value.Equals(coordinateName) select e;

            if (vls.Count() == 1)
            {
                vls.ElementAt(0).Element("MinConfidence").Value = minimumConfidence.ToString();
            }

            xmldoc.Save(configurationFileName);
        }
        #endregion

    }
}
