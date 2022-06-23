
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{

    public class FieldKey {

        #region Properties & Fields

        [JsonIgnore]
        [XmlIgnore]
        public List<dynamic> Content { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Block Block { get; set; }

        public float Confidence { get; set; }

        public Geometry Geometry { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }
        #endregion

        public FieldKey() { }

        public FieldKey(Block block, List<string> children, List<Block> blocks) {
			this.Block = block;
			this.Confidence = block.Confidence;
            //this.Geometry = block.Geometry;
            this.Geometry = new NewGeometry(block.Geometry);

            this.Id = block.Id;
			this.Text = string.Empty;
			this.Content = new List<dynamic>();

			var words = new List<string>();

			if(children != null && children.Count > 0) {
				children.ForEach(c => {
					var wordBlock = blocks.Find(b => b.Id == c);
					if(wordBlock.BlockType == "WORD") {
						var w = new Word(wordBlock, blocks);
						this.Content.Add(w);
						words.Add(w.Text);
					}
				});
			}

			if(words.Count > 0) {
				this.Text = string.Join(" ", words);
			}
		}

		public override string ToString() {
			return Text;
		}
	}
}