
namespace ACST.AWS.TextractViewer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;

    using ACST.AWS.Common;
    using Model = Amazon.Textract.Model;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.TransferUtility;

    using ACST.AWS.TextractClaimMapper;
    using ACST.AWS.Common.OCR;
    using ADA=ACST.AWS.TextractClaimMapper.ADA;
    using ACST.AWS.TextractClaimMapper.OCR;
    using System.IO;
    using System.Drawing.Drawing2D;

    public class TextractClaimImage
    {

        #region Properties & Fields

        public Image SourceImage { get; private set; }

        public AWSTextractClaimCache<ADA.ADAClaim> Cache { get; private set; }

        public OCRResultMetaData MetaData { get { return Cache.MetaData; } }

        public TextractDocument TextractDocument { get { return Cache.TextractDocument; } }

        public Page Page { get { return Cache.TextractDocument.Pages[0]; } }

        public ADA.ADAClaim Claim { get { return Cache?.Claim; } }

        public IEnumerable<ValidationResult> ValidationResults { get; private set; }

        public int ImageHeight { get { return SourceImage.Height; } }

        public int ImageWidth { get { return SourceImage.Width; } }

        static int ImageConfiguredOffsetX { get; set; } = 0;

        static int ImageConfiguredOffsetY { get; set; } = 0;
        #endregion

        #region Constructors
        public TextractClaimImage() { }

        #endregion

        #region Methods
        
        public void BuildADA(string compressedOCRFileName, ADA.ADANamedCoordinates namedCoordinates = null)
        {
            // Load from compressed TextractClaimarchive
            AWSTextractClaimCache<ADA.ADAClaim> cache
                = new AWSTextractClaimCache<ADA.ADAClaim>(compressedOCRFileName);

            if (namedCoordinates == null)
                cache.BuildTextractClaim();
            else
                cache.BuildTextractClaim(namedCoordinates);

            this.Cache = cache;
            this.SourceImage = Image.FromFile(this.MetaData.RootedPath(this.MetaData.ImageFileName));
        }

        public void OpenADA(string compressedOCRFileName)
        {
            // Load from compressed TextractClaimarchive
            AWSTextractClaimCache<ADA.ADAClaim> cache
                = new AWSTextractClaimCache<ADA.ADAClaim>(compressedOCRFileName);

            this.Cache = cache;
            string fn = this.MetaData.RootedPath(this.MetaData.ImageFileName);
            
            if (!File.Exists(fn))
                System.Threading.Thread.Sleep(5000);

            this.SourceImage = Image.FromFile(fn);

            // ToDo: change storage of claim so that fields are serialised, until then rebuild on open
            cache.OpenTextractClaim(true);
        }

        public void Save(bool export = false)
        {
            TextractClaim<ADA.ADAClaim> textractClaim = new TextractClaim<ADA.ADAClaim>(this.TextractDocument);
            textractClaim.Claim = this.Claim;
            bool isValid = textractClaim.Claim.IsValid;
            this.ValidationResults = textractClaim.Claim.ValidationResults();

            if (export & isValid)
            {
                textractClaim.Claim.AssignClaimNumber();
                this.MetaData.ClaimNo = textractClaim.Claim.ClaimNo;
            }

            AWSTextractClaimCache<ADA.ADAClaim>.SaveTextractClaim(textractClaim, this.MetaData);
            FileTransfer.CompressOCRResults(this.MetaData);

            if (export & isValid)
            {
                // Moved to BaseClaim, in assignclaimno
                //// remove any empty service lines before exporing
                //textractClaim.Claim.ServiceLines.Where(l => l.HasData == false | l.IsHeader)
                //    .ToList().ForEach(l => textractClaim.Claim.ServiceLines.Remove(l));

                AWSTextractClaimCache<ADA.ADAClaim>.ExportTextractClaim(textractClaim, this.MetaData);

                textractClaim.Claim.StoreClaimDetails(this.MetaData.ImageFileName);
            }
        }

        public void Archive()
        {
            //this.MetaData

            string fn = this.MetaData.ClaimNo.IsNotNullOrWhiteSpace()
                                                ? $"{this.MetaData.ClaimNo}.zip"
                                                : Path.GetFileName(this.MetaData.SourceFileName);

            string destinationArchiveFileName = Path.Combine(Configuration.Instance.Textract_ArchiveFolder, fn);

            if (File.Exists(destinationArchiveFileName))
                File.Delete(destinationArchiveFileName);

            File.Move(this.MetaData.SourceFileName, destinationArchiveFileName);
        }
        #endregion

        #region Utility Methods

        public Model.BoundingBox ImageIdentityPosition(Model.BoundingBox boundingBox)
        {
            var left = boundingBox.Left / (this.ImageWidth + ImageConfiguredOffsetX);
            var top = boundingBox.Top / (this.ImageHeight + ImageConfiguredOffsetY);
            var width = boundingBox.Width / this.ImageWidth;
            var height = boundingBox.Height / this.ImageHeight;

            return new Model.BoundingBox { Left = left, Top = top, Width = width, Height = height };
        }

        public Model.BoundingBox ImagePosition(Model.BoundingBox boundingBox)
        {
            //var left = (this.ImageWidth + ImagetOffsetX) * boundingBox.Left;
            //var top = (this.ImageHeight + ImageOffsetY) * boundingBox.Top;
            //var width = this.ImageWidth * boundingBox.Width;
            //var height = this.ImageHeight * boundingBox.Height;

            //return new Model.BoundingBox { Left = left, Top = top, Width = width, Height = height };
            return ImagePosition(this.SourceImage, boundingBox);
        }

        public PointF AbsoluteImageCenter(Image image)
        {
            return new PointF(image.Width / 2, image.Height / 2);
        }

        public static Model.BoundingBox ImagePosition(Image image,  Model.BoundingBox boundingBox)
        {
            var left = (image.Width + ImageConfiguredOffsetX) * boundingBox.Left;
            var top = (image.Height + ImageConfiguredOffsetY) * boundingBox.Top;
            var width = image.Width * boundingBox.Width;
            var height = image.Height * boundingBox.Height;

            return new Model.BoundingBox { Left = left, Top = top, Width = width, Height = height };
        }

        public PointF ImagePosition(PointF point)
        {
            return ImagePosition(this.SourceImage, point);
        }

        public static PointF ImagePosition(Image image, PointF point)
        {
            var x = (image.Width + ImageConfiguredOffsetX) * point.X;
            var y = (image.Height + ImageConfiguredOffsetY) * point.Y;

            return new PointF { X = x, Y = y };
        }

        public static PointF ImageIdentityPosition(Image image, PointF point)
        {
            var left = point.X / (image.Width + ImageConfiguredOffsetX);
            var top = point.Y / (image.Height + ImageConfiguredOffsetY);

            return new PointF(left, top);
        }

        // turn this into cast
        public static RectangleF BoundingBoxToRectangle(Model.BoundingBox boundingBox)
        {
            return new RectangleF(boundingBox.Left, boundingBox.Top, boundingBox.Width, boundingBox.Height);
        }

        public static Matrix RotationMatrix(float angle, PointF centerOfRotation)
        {
            Matrix result = new Matrix();
            result.RotateAt(angle, centerOfRotation);
            return result;
        }

        public static PointF RotatePoint(PointF point, PointF centerOfRotation, float angle)
        {
            PointF[] array = { point };

            Matrix matrix = new Matrix();
            if (centerOfRotation == PointF.Empty)
                matrix.Rotate(angle, MatrixOrder.Append);
            else
                matrix.RotateAt(angle, centerOfRotation, MatrixOrder.Append);

            matrix.TransformPoints(array);

            return array[0];
        }


        #endregion
    }
}
