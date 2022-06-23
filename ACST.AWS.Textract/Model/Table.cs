using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{
	public class Table {

		public static Dictionary<int, string> MatchedHeaders;

		#region Properties & Fields

		[JsonIgnore]
		[XmlIgnore]
		public Block Block { get; set; }
		public float Confidence { get; set; }

		//[JsonIgnore]
		//[XmlIgnore]
		public Geometry Geometry { get; set; }

		public string Id { get; set; }

		public List<Row> Rows { get; set; }
		#endregion

		public Table() { }

        public Table(Block block, List<Block> blocks, NamedCoordinates namedCoordinates) {

			MatchedHeaders = new Dictionary<int, string>();

			this.Block = block;
			this.Confidence = block.Confidence;
			this.Geometry = block.Geometry;
            this.Id = block.Id;
			this.Rows = new List<Row>();
			var ri = 1;
			var row = new Row();

			var relationships = block.Relationships;

			if(relationships != null && relationships.Count > 0) {
				relationships.ForEach(r => {
					if(r.Type == "CHILD") {
						r.Ids.ForEach(id => {
							var cell = new MatchedCell(blocks.Find(b => b.Id == id), blocks, namedCoordinates);
							if (cell.RowIndex > ri) {

								var x = row.RowType;
								this.Rows.Add(row);

								row = new Row();
								ri = cell.RowIndex;
							}
							row.Cells.Add(cell);
						});
						if (row != null && row.Cells.Count > 0)
						{
							//row.RowType = row.Cells.Cast<MatchedCell>().First().RowType;
							this.Rows.Add(row);
						}
					}
				});
			}
		}

		public IEnumerable<Cell> GetCellsByCoordinate(PointF coordinate)
		{
			if (this == null) return null;

			return this.Rows.SelectMany(m => m.Cells.Where(c => c.Geometry.ContainsPointF(coordinate)));

			//return this.Form.Fields
			//	.Where(f => f.Value != null && f.Value.Geometry.ContainsPointF(coordinate))
			//	;
		}


		public override string ToString() {
			var result = new List<string>();
			result.Add(string.Format("Table{0}===={0}", Environment.NewLine));
			this.Rows.ForEach(r => {
				result.Add(string.Format("Row{0}===={0}{1}{0}", Environment.NewLine, r));
			});
			return string.Join("", result);
		}
	}
}