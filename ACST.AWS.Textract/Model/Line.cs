
using Newtonsoft.Json;
using System;
using Amazon.Textract.Model;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACST.AWS.Textract.Model
{
	public class Line 
        : IAWSElement
    {

        #region Properties & Fields

        [JsonIgnore]
        [XmlIgnore]
        public Block Block { get; set; }

        public float Confidence { get; set; }

        public NewGeometry Geometry { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        public List<Word> Words { get; set; }
        #endregion

        public Line() { }

        public Line(Block block, List<Block> blocks, NamedCoordinates namedCoordinates = null) {
			this.Block = block;
			this.Confidence = block.Confidence;

            this.Geometry = new NewGeometry(block.Geometry, namedCoordinates?.PageGridRows, namedCoordinates?.PageGridColumns);

            this.Id = block.Id;
			this.Text = block == null ? string.Empty : block.Text;
			this.Words = new List<Word>();

			var relationships = block.Relationships;
			if(relationships != null && relationships.Count > 0) {
				relationships.ForEach(r => {
					if(r.Type == "CHILD") {
						r.Ids.ForEach(id => {
							this.Words.Add(new Word(blocks.Find(b => b.BlockType == "WORD" && b.Id == id), blocks));
						});
					}
				});
			}
		}

        public override string ToString()
        {
            return $"line: [{this.Geometry.ReadingOrder.Row},{this.Geometry.ReadingOrder.Column}] {this.Text}";
        }
    }
}