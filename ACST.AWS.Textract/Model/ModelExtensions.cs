
namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Amazon.Textract.Model;

    public static class ModelExtensions
    {
        // aparently this is in Core but not Std
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            => dict.TryGetValue(key, out var value) ? value : default(TValue);

        public static bool ContainsPointF(this Geometry self, PointF coordinate)
        {
            if (self == null) return false;

            return self.BoundingBox.Left <= coordinate.X
                    & self.BoundingBox.Left + self.BoundingBox.Width >= coordinate.X
                    & self.BoundingBox.Top <= coordinate.Y
                    & self.BoundingBox.Top + self.BoundingBox.Height >= coordinate.Y
                ;
        }

        public static bool ContainsPointF(this Block self, PointF coordinate)
        {
            if (self == null) return false;
            return self.Geometry.ContainsPointF(coordinate);
        }
    }

}
