
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;
using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{
	public class SelectionElement {

        #region Properties & Fields

        public float Confidence { get; set; }

        //[JsonIgnore]
        //[XmlIgnore]
        public Geometry Geometry { get; set; }

        public string Id { get; set; }

        public string SelectionStatus { get; set; }
        #endregion

        public SelectionElement() { }

        public SelectionElement(Block block, List<Block> blocks) {
			this.Confidence = block.Confidence;
            //this.Geometry = block.Geometry;
            this.Geometry = new NewGeometry(block.Geometry);
            this.Id = block.Id;
			this.SelectionStatus = block.SelectionStatus;
            
		}

	}
}