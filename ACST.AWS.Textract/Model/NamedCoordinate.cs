
namespace ACST.AWS.Textract.Model
{
    using System.Drawing;
    using System.Xml.Serialization;

    using Amazon.Textract.Model;
    using ACST.AWS.Common;
    using System.Runtime.CompilerServices;

    public class NamedCoordinate 
        : INamedCoordinate
    {
        #region Propertied and Fields

        [XmlIgnore]
        public string NamedCoordinatesOverrideFileName { get { return Configuration.Instance.ADANamedCoordinates_FileName; } }

        public NamedCoordinatesSource Source { get; set; }

        [XmlAttribute(AttributeName = "donotmap")]
        public bool DoNotMap { get; set; }

        public string Name { get; set; }

        public string ExactTextMatch { get; set; }

        public string HeaderText { get; set; }

        public string GroupName { get; set; }

        public string Type { get; set; }

        public string MultiSelect { get; set; }

        public float MinConfidence { get; set; }

        //public bool Required { get; set; }

        public int TabOrder { get; set; }

        public byte Column { get; set; }

        public byte Row { get; set; }

        public byte Width { get; set; }

        public PointF IdealCenterValue { get; set; }

        public PointF IdealCenterKey { get; set; }

        public BoundingBox FieldBoundingBox { get; set; }

        public BoundingBox ValueBoundingBox { get; set; }

        public BoundingBox ValueOffset { get; set; }
        #endregion

        #region Constructors

        public NamedCoordinate() { }

        public NamedCoordinate(string name, string exactTextMatch, byte row, byte column)
            : this(name, exactTextMatch, row, column, new PointF()) { }

        public NamedCoordinate(string name, string exactTextMatch, byte row, byte column, PointF idealCenterValue)
        {
            this.Name = name;
            this.ExactTextMatch = exactTextMatch;
            this.Row = row;
            this.Column = column;
            this.IdealCenterValue = idealCenterValue;
        }

        public NamedCoordinate(string groupName, string name, string exactTextMatch, PointF idealCenterKey, PointF idealCenterValue)
        {
            this.GroupName = groupName;
            this.Name = name ?? exactTextMatch.SafeSubstring(0, 20);
            this.ExactTextMatch = exactTextMatch;
            //this.Row = row;
            //this.Column = column;
            this.IdealCenterKey = idealCenterKey;
            this.IdealCenterValue = idealCenterValue;
        }

        public NamedCoordinate(string groupName, string name, string exactTextMatch, string value, PointF idealCenterKey, PointF idealCenterValue)
            : this(groupName, name, exactTextMatch,idealCenterKey, idealCenterValue)
        {
            var ignoreVaue = value;
        }

        public NamedCoordinate(string groupName, string name, string exactTextMatch, string value, PointF idealCenterKey, PointF idealCenterValue, BoundingBox fieldBoundingBox, BoundingBox valueBoundingBox)
            : this(groupName, name, exactTextMatch, value, idealCenterKey, idealCenterValue)
        {
            this.FieldBoundingBox = fieldBoundingBox;
            this.ValueBoundingBox = valueBoundingBox;
        }
        #endregion

        public BoundingBox OffsetValueBoundingBox(BoundingBox actualKeyBoundingBox)
        {
            if (ValueOffset == null)
                return ValueBoundingBox;

            return new BoundingBox
            {
                Height = ValueOffset.Height,
                Left = actualKeyBoundingBox.Left + ValueOffset.Left,
                Top = actualKeyBoundingBox.Top + actualKeyBoundingBox.Height + ValueOffset.Top,
                Width = ValueOffset.Width
            };
        }
    }
}
