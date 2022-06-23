
namespace ACST.AWS.Textract.Model
{
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Newtonsoft.Json;
	using Amazon.Textract.Model;
    using ACST.AWS.Common;
    using System.Linq;

	public class Cell
		: IAWSElement, IMatchedElement, IMappedElement
	{

		#region Properties & Fields

		[JsonIgnore]
		[XmlIgnore]
		public List<dynamic> Content { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public Block Block { get; set; }

		public float Confidence { get; set; }

		//[JsonIgnore]
		//[XmlIgnore]
		//public Geometry Geometry { get; set; }
		public Geometry Geometry { get; set; }

		public string Id { get; set; }

		public string Text { get; set; }

		public int RowIndex { get; set; }
		public int RowSpan { get; set; }
		public int ColumnIndex { get; set; }
		public int ColumnSpan { get; set; }

		//public string Type { get; set; }

		//public bool MultiSelect { get; set; }

		public string HeaderText { get; set; }

		public string GroupName { get; set; }

		//public int TabOrder { get; }

		public string Match { get; set; }

		public bool MappedToClaim { get; set; }

		public float MinConfidence { get; set; }

		public bool Required { get; set; }
		
		public bool UpdateToClaim { get; set; }
		#endregion

		public Cell() { }

        public Cell(Block block, List<Block> blocks) {
			this.Block = block;
			this.ColumnIndex = block.ColumnIndex;
			this.ColumnSpan = block.ColumnSpan;
			this.Confidence = block.Confidence;
			this.Content = new List<dynamic>();
			//this.Geometry = block.Geometry;
			this.Geometry = new NewGeometry(block.Geometry);
			this.Id = block.Id;
			this.RowIndex = block.RowIndex;
			this.RowSpan = block.RowSpan;
			this.Text = string.Empty;

			var relationships = block.Relationships;
			if(relationships != null && relationships.Count > 0) {
				relationships.ForEach(r => {
					if(r.Type == "CHILD") {
						r.Ids.ForEach(id => {
							var rb = blocks.Find(b => b.Id == id);
							if(rb.BlockType == "WORD") {
								var w = new Word(rb, blocks);
								this.Content.Add(w);
								this.Text = this.Text + w.Text + " ";
							} else if(rb.BlockType == "SELECTION_ELEMENT") {
								var se = new SelectionElement(rb, blocks);
								this.Content.Add(se);
								this.Text = this.Text + se.SelectionStatus + ", ";
							}
						});
					}
				});
			}
		}

		public override string ToString() {
			return $"{this.Text}";
		}
	}
}