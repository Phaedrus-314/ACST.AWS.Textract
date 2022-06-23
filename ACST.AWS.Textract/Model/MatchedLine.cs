using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Textract.Model;
using ACST.AWS.Common;

namespace ACST.AWS.Textract.Model
{
    public class MatchedLine
        : Line, IMatchedElement, IMappedElement
    {

        #region Properties & Fields

        public bool HasMatch { get { return this.Match.IsNotNullOrEmpty(); } }

        //public string Type { get; set; }

        //public bool MultiSelect { get; set; }
        public float MinConfidence { get; }

        public string HeaderText { get; }

        public string GroupName { get; }

        //public int TabOrder { get; }

        public string Match { get; set; }

        public bool Required { get; set; }

        public bool UpdateToClaim { get; set; }

        public int MatchedTableColumn { get; set; }

        public int MatchedTableRow { get; set; }

        NamedCoordinates NamedCoordinates { get; }

        public bool MappedToClaim { get; set; }
        #endregion

        public MatchedLine() { }

        public MatchedLine(Block block, List<Block> blocks, NamedCoordinates namedCoordinates) 
            :base(block, blocks, namedCoordinates)
        {
            this.NamedCoordinates = namedCoordinates;

            SearchForMatch();
        }

        protected void SearchForMatch()
        {
            if (this.NamedCoordinates == null || base.Text.IsNullOrWhiteSpace())
                return;

            var pk1 = this.NamedCoordinates.MatchTableCellByGridCoordinate(this.Geometry);
            if (pk1 != null && pk1.Any())
            {
                var m = pk1.First();
                this.Match = m.Name;
                //this.MatchedTableColumn = m.Column;
                //this.MatchedTableRow = m.Row - this.NamedCoordinates.TableHeaderGridPosition;
                this.MatchedTableColumn = this.Geometry.ReadingOrder.Column;
                this.MatchedTableRow = 1 + (int)Math.Round(Convert.ToDecimal((this.Geometry.ReadingOrder.Row - this.NamedCoordinates.TableHeaderGridPosition) / this.NamedCoordinates.TableSubRows), MidpointRounding.AwayFromZero);
            }
        }
        
        public override string ToString()
        {
            string s = HasMatch ? " " + this.Match : "";

            return base.ToString() + s;
        }
    }
}
