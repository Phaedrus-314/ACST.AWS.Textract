
namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Linq;
    using Amazon.Textract.Model;
    using Point = Amazon.Textract.Model.Point;
    using Newtonsoft.Json;
    using System.Xml.Serialization;


    public class NewGeometry 
        : Geometry
    {
        #region Properties & Fields

        //[JsonIgnore]
        //[XmlIgnore]
        public PointF Center { get { return this.BoundingBox == null ? default : new PointF(this.BoundingBox.Left + (this.BoundingBox.Width / 2), this.BoundingBox.Top + (this.BoundingBox.Height / 2)); } }

        //[JsonIgnore]
        //[XmlIgnore]
        public RectangleF Rectangle { get; set;}

        public ReadingOrder ReadingOrder { get; set; }
        #endregion

        // dynamic JSON property ignore
        public bool ShouldSerializeRectangle()
        { 
            // local example, use contractresolver otherwise
            return (false);
        }

        public NewGeometry() { }

        public NewGeometry(Geometry geometry, Nullable<int> maxRows = null, Nullable<int> maxColumns = null) : base() {

			this.BoundingBox = geometry.BoundingBox;
			this.Polygon = geometry.Polygon;
            this.Rectangle = new RectangleF(this.BoundingBox.Left, this.BoundingBox.Top, this.BoundingBox.Width, this.BoundingBox.Height);
            
            //this.Center = new PointF(this.BoundingBox.Left + (this.BoundingBox.Width / 2), this.BoundingBox.Top + (this.BoundingBox.Height / 2));
            this.ReadingOrder = new ReadingOrder(this.Center, maxRows, maxColumns);

            var bb = new NewBoundingBox(this.BoundingBox.Width, this.BoundingBox.Height, this.BoundingBox.Left, this.BoundingBox.Top);
			var pgs = new List<Point>();
			Polygon.ForEach(pg => pgs.Add(new Point {
				X = pg.X,
				Y = pg.Y
			}));

			BoundingBox = bb;
			Polygon = pgs;
		}

        public override string ToString() {
            return $"BoundingBox: {this.BoundingBox}{Environment.NewLine}";

		}

        public static PointF IdealCenter(BoundingBox boundingBox)
        {
            return new PointF(boundingBox.Left + (boundingBox.Width / 2), boundingBox.Top + (boundingBox.Height / 2));
        }

        public static BoundingBox EnclosingBoundingBox(IEnumerable<Geometry> geometries)
        {
            return EnclosingBoundingBox(geometries.Select(g => g.BoundingBox));
        }

        public static BoundingBox EnclosingBoundingBox(IEnumerable<BoundingBox> boundingBoxs)
        {
            if (boundingBoxs == null || !boundingBoxs.Any())
                return null;
            
            float minTop = boundingBoxs.Min(g => g.Top);
            float minLeft = boundingBoxs.Min(g => g.Left);
            float maxBottom = boundingBoxs.Max(g => g.Top + g.Height);
            float maxRight = boundingBoxs.Max(g => g.Left + g.Width);

            BoundingBox bb = new BoundingBox();
            bb.Top = minTop;
            bb.Left = minLeft;
            bb.Height = Math.Abs(maxBottom - minTop);
            bb.Width = Math.Abs(maxRight - minLeft);

            //ACST.AWS.Common.Logger.TraceVerbose($"Outside: ({bb.Top},{bb.Left}) x ({bb.Height},{bb.Width})");

            return bb;
        }

    }


}