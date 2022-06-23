
namespace ACST.AWS.Textract.Model 
{

    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Drawing = System.Drawing;
    using Newtonsoft.Json;
    using Amazon.Textract.Model;
    using ACST.AWS.Common;


    public class FieldValue {

        #region Properties & Fields

        [JsonIgnore]
        [XmlIgnore()]
        public List<dynamic> Content { get; set; }

        [JsonIgnore]
        [XmlIgnore()]
        public Block Block { get; set; }

        public float Confidence { get; set; }

        public Geometry Geometry { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }
        #endregion

        public FieldValue() { }

        public FieldValue(Block block, List<string> children, List<Block> blocks)
        {
			this.Block = block;
			this.Confidence = block.Confidence;
            this.Geometry = new NewGeometry(block.Geometry);
            this.Id = block.Id;
			this.Text = string.Empty;
			this.Content = new List<dynamic>();

            int row, col;
            row = -1;
            col = 0;

            //ReadingOrder readingOrder = new ReadingOrder();
            Drawing.RectangleF previousLocation = new Drawing.RectangleF();

            var words = new List<string>();
			if(children != null && children.Count > 0) {

				children.ForEach(c => {
					var wordBlock = blocks.Find(b => b.Id == c);
					if(wordBlock.BlockType == "WORD") {
						var w = new Word(wordBlock, blocks);
                        var g = (NewGeometry)w.Geometry;

                        if (!previousLocation.VerticallyContains(g.Rectangle))
                        {
                            col = 0;
                            row++;
                        }

                        g.ReadingOrder = new ReadingOrder(row, col++);
						this.Content.Add(w);
						words.Add(w.Text);
                        
                        previousLocation = g.Rectangle;
                    }
                    else if (wordBlock.BlockType == "SELECTION_ELEMENT")
                    {
                        var selection = new SelectionElement(wordBlock, blocks);
                        this.Content.Add(selection);
                        words.Add(selection.SelectionStatus);
                    }
                });
			}

			if(words.Count > 0)
            {
				this.Text = string.Join(" ", words);
			}
		}

		public override string ToString() {
			return Text;
		}
	}
}