
namespace ACST.AWS.Textract.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Amazon.Textract.Model;
    using ACST.AWS.Common;
    using System.Linq;

	public class MatchedCell
		: Cell
	{
		#region Properties & Fields

		public TextractClaimTableRowType RowType { get; set; }

		// ToDo: Move these up from Cell

		//public bool HasMatch { get { return this.Match.IsNotNullOrEmpty(); } }

		//public string Type { get; set; }

		//public string HeaderText { get; set; }

		//public bool MultiSelect { get; set; }

		//public string GroupName { get; set; }

		//public int TabOrder { get; set; }

		//public string Match { get; set; }
		#endregion

		public MatchedCell() { }

		public MatchedCell(Block block, List<Block> blocks, NamedCoordinates namedCoordinates)
			: base(block, blocks)
		{
			if (this.RowIndex == 1 | this.RowIndex == 2)
			{
				SearchForMatch(namedCoordinates);
			}
			else
			{
				string matchedPropertyAttributeName = null;

				// Assing claim property name from mapped header index
				if (Table.MatchedHeaders.TryGetValue(this.ColumnIndex, out matchedPropertyAttributeName))
				{
					if (this.Text.IsNotNullOrWhiteSpace())
						this.RowType = TextractClaimTableRowType.DataRow;
					this.Match = matchedPropertyAttributeName;
				}
			}
		}

		protected void SearchForMatch(NamedCoordinates namedCoordinates)
		{
			if (namedCoordinates == null || base.Text.IsNullOrWhiteSpace())
				return;

			NamedCoordinate namedCoordinate;

			var results = namedCoordinates.MatchTableByFuzzyText(this);
			if (results == null)
			{
				Logger.TraceVerbose($"MatchByColumnCenter search for: {this.Text}");
				Logger.TraceVerbose($"\tOCR.Header:\t'{this.Text}'");
				//results = namedCoordinates.MatchKeyByIdealCenter((NewGeometry)this.Geometry).FirstOrDefault();
				results = namedCoordinates.MatchByColumnCenter((NewGeometry)this.Geometry, "Table").FirstOrDefault();
			}

			namedCoordinate = results; //.FirstOrDefault();

			if (namedCoordinate != null)
			{
				this.Match = namedCoordinate.Name;
				this.HeaderText = namedCoordinate.ExactTextMatch;
				this.GroupName = namedCoordinate.GroupName;

				if (Table.MatchedHeaders.ContainsKey(this.ColumnIndex))
				{
					if (this.Text.IsNotNullOrWhiteSpace())
						this.RowType = TextractClaimTableRowType.DataRow;
					Table.MatchedHeaders[this.ColumnIndex] = namedCoordinate.Name;
				}
				else
				{
					if (this.Text.IsNotNullOrWhiteSpace())
						this.RowType = TextractClaimTableRowType.HeaderRow;
					Table.MatchedHeaders.Add(this.ColumnIndex, namedCoordinate.Name);
				}
			}
		}

		public override string ToString()
		{
			return $"{this.RowType}: {this.Text}";
		}

	}
}
