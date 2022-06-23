
namespace ACST.AWS.Common
{
    using System;
    using System.Drawing;

    public static class GeometryExtensions
    {
        public static bool HorizontallyContains(this RectangleF self, RectangleF rectangle)
        {
            var bbLeft = rectangle.Left;
            var bbRight = rectangle.Left + rectangle.Width;
            var bbCentre = rectangle.Left + (rectangle.Width / 2);
            var rowCentre = self.Left + (self.Width / 2);

            return ((bbCentre > self.Left && bbCentre < self.Right) || (rowCentre > bbRight && rowCentre < bbLeft));
        }

        public static bool VerticallyContains(this RectangleF self, RectangleF rectangle)
        {
            var bbTop = rectangle.Top;
            var bbBottom = rectangle.Top + rectangle.Height;
            var bbCentre = rectangle.Top + (rectangle.Height / 2);
            var rowCentre = self.Top + (self.Height / 2);

            return ((bbCentre > self.Top && bbCentre < self.Bottom) || (rowCentre > bbBottom && rowCentre < bbTop));
        }
    }
}
