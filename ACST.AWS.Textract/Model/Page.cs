
namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Amazon.Textract.Model;

    using ACST.AWS.Common;
    using System.Drawing;

    public class Page
    {
        #region Properties & Fields

        [JsonIgnore]
        [XmlIgnore]
        public List<dynamic> Content { get; } = new List<dynamic>();

        [JsonIgnore]
        [XmlIgnore]
        public List<Block> Blocks { get; set; }

        //[JsonIgnore]
        //[XmlIgnore]
        public Geometry Geometry { get; set; }

        public string Id { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string Text { get; set; }


        public List<MatchedLine> Lines { get; set; }

        public Form Form { get; set; }

        public List<Table> Tables { get; set; }

        #endregion

        public Page() { }

        public Page(List<Block> blocks, List<Block> blockMap, NamedCoordinates namedCoordinates)
        {

            #region Validation

            if (blocks == null)
                throw new ArgumentNullException(nameof(blocks));

            if (blockMap == null)
                throw new ArgumentNullException(nameof(blockMap));
            #endregion

            this.Blocks = blocks;
            this.Text = string.Empty;
            this.Lines = new List<MatchedLine>();
            this.Form = new Form();
            this.Tables = new List<Table>();
            this.Content = new List<dynamic>();

            blocks.ForEach(b => {
                if (b.BlockType == "PAGE")
                {
                    Console.WriteLine("Page: {0}", b.Id);
                    this.Geometry = new NewGeometry(b.Geometry);
                    this.Id = b.Id;
                }
                else if (b.BlockType == "LINE")
                {
                    var l = new MatchedLine(b, blockMap, namedCoordinates);
                    this.Lines.Add(l);
                    this.Content.Add(l);
                    this.Text = this.Text + l.Text + Environment.NewLine;
                }
                else if (b.BlockType == "TABLE")
                {
                    var t = new Table(b, blockMap, namedCoordinates);
                    this.Tables.Add(t);
                    this.Content.Add(t);
                }
                else if (b.BlockType == "KEY_VALUE_SET")
                {
                    if (b.EntityTypes.Contains("KEY"))
                    {
                        var f = new Field(b, blockMap, namedCoordinates);
                        
                        if (f.Key != null)
                        {
                            this.Form.AddField(f);
                            this.Content.Add(f);
                        }
                    }
                }
            });
        }

        //public List<Block> GetBlocksByCoordinate(PointF coordinate)
        //{
        //    //return this.Pages.Where(p => p.Blocks.)
        //}

        //public class IndexedText
        //{
        //    public int ColumnIndex { get; set; }
        //    public int RowIndex { get; set; }
        //    public string Text { get; set; }

        //    public override string ToString()
        //    {
        //        return string.Format("[{0},{1}] {2}", this.RowIndex, this.ColumnIndex, this.Text);
        //    }
        //}

        //public List<IndexedText> GetLines_ByRow_InReadingOrder(FieldValue fieldValue)
        //{
        //    var columns = 0;
        //    var rows = new List<Drawing.RectangleF>();
        //    var lines = new List<IndexedText>();

        //    fieldValue.Content.ForEach(word =>
        //    {
        //        var rowFound = false;
        //        for (var index = 0; index < rows.Count; index++)
        //        {
        //            var row = rows[index];
        //            var bb = word.Geometry.BoundingBox;
        //            var bbTop = bb.Top;
        //            var bbBottom = bb.Top + bb.Height;
        //            var bbCentre = bb.Top + (bb.Height / 2);
        //            var rowCentre = row.Top + (row.Height / 2);

        //            if ((bbCentre > row.Top && bbCentre < row.Bottom) || (rowCentre > bbBottom && rowCentre < bbTop))
        //            {
        //                lines.Add(new IndexedText { RowIndex = index, ColumnIndex = ++columns, Text = word.Text });
        //                rowFound = true;
        //                break;
        //            }
        //        }
        //        if (!rowFound)
        //        {
        //            var bb = word.Geometry.BoundingBox;
        //            columns = 0;
        //            rows.Add(new Drawing.RectangleF(bb.Left, bb.Top, bb.Width, bb.Height));
        //            lines.Add(new IndexedText { RowIndex = rows.Count - 1, ColumnIndex = columns, Text = word.Text });
        //        }
        //    });
        //    lines.FindAll(line => line.RowIndex == 0).ForEach(line => Console.WriteLine(line));
        //    return lines;
        //}

        //public List<IndexedText> GetLinesInReadingOrder()
        //{
        //    var lines = new List<IndexedText>();
        //    var columns = new List<Column>();

        //    this.Lines.ForEach(line => {
        //        var columnFound = false;
        //        for (var index = 0; index < columns.Count; index++)
        //        {
        //            var column = columns[index];
        //            var bb = line.Geometry.BoundingBox;
        //            var bbLeft = bb.Left;
        //            var bbRight = bb.Left + bb.Width;
        //            var bbCentre = bb.Left + (bb.Width / 2);
        //            var columnCentre = column.Left + (column.Right / 2);

        //            if ((bbCentre > column.Left && bbCentre < column.Right) || (columnCentre > bbLeft && columnCentre < bbRight))
        //            {
        //                lines.Add(new IndexedText { ColumnIndex = index, Text = line.Text });
        //                columnFound = true;
        //                break;
        //            }
        //        }
        //        if (!columnFound)
        //        {
        //            var bb = line.Geometry.BoundingBox;
        //            columns.Add(new Column { Left = bb.Left, Right = bb.Left + bb.Width });
        //            lines.Add(new IndexedText { ColumnIndex = columns.Count - 1, Text = line.Text });
        //        }
        //    });
        //    lines.FindAll(line => line.ColumnIndex == 0).ForEach(line => Console.WriteLine(line));
        //    return lines;
        //}

        //public string GetTextInReadingOrder()
        //{
        //    var lines = this.GetLinesInReadingOrder();
        //    var text = string.Empty;
        //    lines.ForEach(line => {
        //        text = text + line.Text + "\n";
        //    });
        //    return text;
        //}

        public IEnumerable<Block> GetBlocksByCoordinate(PointF coordinate)
        {
            if (this == null) return null;

            return this.Blocks.Where(b => b.BlockType != "Page" & b.ContainsPointF(coordinate)).ToList();
        }

        public IEnumerable<Field> GetFieldsByFieldValueCoordinate(PointF coordinate)
        {
            if (this == null) return null;

            return this.Form.Fields
                .Where(f => f.Value != null && f.Value.Geometry.ContainsPointF(coordinate))
                ;
        }

        public IEnumerable<Field> GetFieldsByConfidence(int maximun, int minimum = 0) 
        {
            if (this == null) return null;

            return this.Form.Fields
                .Where(f => f.Value != null && (f.Value.Confidence <= maximun & f.Value.Confidence >= minimum))
                ;
        }

        public IEnumerable<Row> GetCellsByRowCoordinate(PointF coordinate)
        {
            
            return this.Tables[0].Rows
                //.Where(r => r.Cells.Any(c => c.MappedToClaim & c.ColumnIndex > 1));
                .Where(r => r.Cells.Any(c => c.MappedToClaim 
                && c.Geometry.ContainsPointF(coordinate) ))
                ;
        }

        public IEnumerable<Line> GetLinesByCoordinate(PointF coordinate)
        {
            if (this == null) return null;

            return this.Lines
                .Where(l => l.Geometry.ContainsPointF(coordinate))
                ;
        }

        //public IQueryable<(Block, Relationship)> GetRelatedBlocksByCoordinate(PointF coordinate)
        //{
        //    if (this == null) return null;

        //    var nQuery = from b in this.Blocks
        //                 from r in b.Relationships
        //                 where b.BlockType != "Page" & b.ContainsPointF(coordinate)
        //                 select new {b, r};

        //    return nQuery.AsQueryable();
        //}

        //public List<T> GetElementAtCoordinate<T>(PointF coordinate)
        //{
        //    if (this == null) return null;

        //    switch (typeof(T).Name)
        //    {
        //        case "Field":
        //            return this.Form.Fields
        //                        .Where(f => f.Value != null && f.Value.Geometry.ContainsPointF(coordinate))
        //                        .ToList();
        //            break;

        //        default:
        //            return null;
        //            break;
        //    }
        //}

        public override string ToString()
        {
            var result = new List<string>();
            result.Add($"Page: {this.Id}{Environment.NewLine}");
            this.Content.ForEach(c => {
                result.Add($"{c}{Environment.NewLine}");
            });
            return string.Join("", result);
        }
    }
}
