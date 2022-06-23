
namespace ACST.AWS.Textract.Model
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Amazon.Textract.Model;
    using System;

    public class Word {

        #region Properties & Fields

        [JsonIgnore]
        [XmlIgnore]
        public Block Block { get; set; }

        public float Confidence { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public NewGeometry Geometry { get; set; }

        public string Id { get; set; }

        public string Text { get; set; }

        #endregion

        public Word() { }

        public Word(Block block, List<Block> blocks) {

            if (block == null)
            {
                this.Text = string.Empty;
                return;
            }

            this.Block = block;
            this.Confidence = block.Confidence;
            this.Geometry = new NewGeometry(block.Geometry);
            //this.Geometry = block.Geometry;
            this.Id = block.Id;
            this.Text = block == null ? string.Empty : block.Text;
        }

		public override string ToString() {
			return this.Text;
		}
	}
}