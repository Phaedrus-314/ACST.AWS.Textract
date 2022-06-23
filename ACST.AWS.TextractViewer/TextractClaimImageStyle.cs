
namespace ACST.AWS.TextractViewer
{
    using System;
    using System.Drawing;

    //struct TextractClaimImageStyle
    static class TextractClaimImageStyle
    {

        public const float DefaultGauge = 1f;
        public const float DefaultPaddingFactor = 1f;

        public const float LargeGauge = 2f;
        public const float LargePaddingFactor = 4f;

        //public static readonly Size DefaultPadding = new Size(10, 3);
        public const int DefaultOpacity = 25;

        public static int InfoPanelInitialHeight = 280;

        public static Font DefaultFont = new Font("Arial", 10);

        // Optical Character Recognition (OCR)
        // Specification Recognition 
        // Named Coordinate Mapping

        public static Color ocrConfidenceFail = Color.FromArgb(242, 44, 44);
        public static Color ocrConfidencePass = Color.FromArgb(66, 245, 78);
        public static Color ocrConfidenceBorderline = Color.FromArgb(242, 245, 66);
        public static int ocrConfidenceBorderlineRange = 5;

        public static Color ocrPageColor = Color.Purple;
        public static Color ocrFieldColor = Color.Purple;
        public static Color ocrFieldKeyColor = Color.Purple;
        public static Color ocrFieldValueColor = Color.Purple;
        public static Color ocrLineColor = Color.Purple;

        public static Color notMatchedFieldColor = Color.Red;

        public static Color matchedCellColor = Color.Blue;
        public static Color matchedFieldColor = Color.Blue;
        public static Color matchedFieldKeyColor = Color.Blue;
        public static Color matchedFieldValueColor = Color.Blue;

        public static Color mappedCellColor = Color.Green;
        public static Color mappedFieldColor = Color.Green;
        public static Color mappedFieldKeyColor = Color.Green;
        public static Color mappedFieldValueColor = Color.Green;

        public static PointF DefaultInsuredIsPatientNoticeLocation => new PointF { X = .8f, Y = .325f };
}
}
