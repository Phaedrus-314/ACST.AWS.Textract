using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{
    public class NewBoundingBox : BoundingBox {

        public NewBoundingBox() { }

        public NewBoundingBox(float width, float height, float left, float top) : base() {
			this.Width = width;
			this.Height = height;
			this.Left = left;
			this.Top = top;
		}

		public override string ToString() {
			return $"width: {this.Width}, height: {this.Height}, left: {this.Left}, top: {this.Left}";
		}
	}
}