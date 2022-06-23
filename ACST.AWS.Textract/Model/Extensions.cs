using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{
    public static class Extensions
    {
        public static bool ContainsPoint(this BoundingBox bb, PointF actualCenter)
        {
            if (bb == null) return false;

            // does this rectangle contain point
            RectangleF r = new RectangleF(bb.Left, bb.Top, bb.Width, bb.Height);

            return r.Contains(actualCenter);
        }

        public static bool SharesColumnWith(this BoundingBox bb, PointF actualCenter)
        {
            if (bb == null) return false;

            // are this rectangle and point in the same column
            RectangleF r = new RectangleF(bb.Left, 0.0F, bb.Width, 1.0F);
            
            return r.Contains(actualCenter);
        }
    }
}
