
namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Drawing;
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    public class ReadingOrder
    {
        #region Properties & Fields

        [JsonIgnore]
        [XmlIgnore]
        public int TotalGridRows { get; set; } = 1;

        [JsonIgnore]
        [XmlIgnore]
        public int TotalGridColumns { get; set; } = 1;

        public int Row { get; set; }
        public int Column { get; set; }
        #endregion

        public ReadingOrder() { }

        public ReadingOrder(PointF center)
        {
            this.Column = (int)(center.X * this.TotalGridColumns);
            this.Row = (int)(center.Y * this.TotalGridRows);
        }

        public ReadingOrder(PointF center, Nullable<int> totalGridRows = null, Nullable<int> totalGridColumns = null)
        {
            this.TotalGridColumns = totalGridColumns.HasValue ? totalGridColumns.Value : this.TotalGridColumns;
            this.TotalGridRows = totalGridRows.HasValue ? totalGridRows.Value : this.TotalGridRows;

            this.Column = (int)(center.X * this.TotalGridColumns);
            this.Row = (int)(center.Y * this.TotalGridRows);
        }

        public ReadingOrder(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public override string ToString()
        {
            return $"[{this.Row}, {this.Column}]";
        }
    }

}
