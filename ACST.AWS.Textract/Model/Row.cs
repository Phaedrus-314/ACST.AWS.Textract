using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ACST.AWS.Common;

namespace ACST.AWS.Textract.Model
{
	public class Row {

		//public TextractClaimTableRowType RowType { get; set; }
		public TextractClaimTableRowType RowType { get { return GetRowType(); } }

		public List<Cell> Cells { get; set; }

		public Row() 
		{
			this.Cells = new List<Cell>();
		}

		TextractClaimTableRowType GetRowType()
        {
			TextractClaimTableRowType ret = TextractClaimTableRowType.Unknown;

			if ( Cells.Cast<MatchedCell>()
				.Any(c => c.RowType.HasFlag(TextractClaimTableRowType.HeaderRow) & c.GroupName == "TableHeader") )
				ret = TextractClaimTableRowType.HeaderRow | TextractClaimTableRowType.TableHeaderRow;
			else
			if (Cells.Cast<MatchedCell>()
				.Any(c => c.RowType.HasFlag(TextractClaimTableRowType.HeaderRow) & c.GroupName == "Table"))
				ret = TextractClaimTableRowType.HeaderRow | TextractClaimTableRowType.ColumnHeaderRow;
			else
			//if (Cells.Cast<MatchedCell>()
			//	.Any(c => c.GroupName == "TableHeader"))
			//	ret = TextractClaimTableRowType.HeaderRow | TextractClaimTableRowType.TableHeaderRow;
			//else
			//	ret = TextractClaimTableRowType.HeaderRow | TextractClaimTableRowType.ColumnHeaderRow;

			if (Cells.Cast<MatchedCell>()
                .Any(c =>  (c.RowType.HasFlag(TextractClaimTableRowType.DataRow))))
				ret = TextractClaimTableRowType.DataRow;

			return ret;
        }

		public override string ToString() {
			var result = new List<string>();
			this.Cells.ForEach(c => {
				result.Add(string.Format("[{0}]", c));
			});
			return $"{RowType}: {string.Join(" ", result)}";
		}
	}
}