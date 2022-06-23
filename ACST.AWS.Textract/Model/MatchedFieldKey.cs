using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Textract.Model;
using ACST.AWS.Common;

namespace ACST.AWS.Textract.Model
{
    public class MatchedFieldKey
        : FieldKey, IMatchedElement
    {
        #region Properties & Fields

        public bool HasMatch { get { return this.Match.IsNotNullOrEmpty(); } }

        public string Type { get; set; }

        public string HeaderText { get; set; }

        public bool MultiSelect { get; set; }

        public string GroupName { get; set; }

        public int TabOrder { get; set; }

        public float MinConfidence { get; set; }

        public string Match { get; set; }
        #endregion

        public MatchedFieldKey() { }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Use this construction when Textract Analysis Form Key-Value's do not contain a required element specified by in NamedCoordinates.
        /// This allows backfilling the document with a dummy Field avaliable for user audit.</remarks>
        /// <param name="namedCoordinate"></param>
        public MatchedFieldKey(NamedCoordinate namedCoordinate) 
        {
            this.Type = namedCoordinate.Type;
            this.GroupName = namedCoordinate.GroupName;
            this.HeaderText = namedCoordinate.HeaderText;
            this.Match = namedCoordinate.Name;
            this.MultiSelect = namedCoordinate.MultiSelect?.ToLower() == "true";
            this.TabOrder = namedCoordinate.TabOrder;
        }

        public MatchedFieldKey(Block block, List<string> children, List<Block> blocks, NamedCoordinates namedCoordinates) :
            base(block, children, blocks)
        {
            SearchForMatch(namedCoordinates);
        }

        protected void SearchForMatch(NamedCoordinates namedCoordinates)
        {
            if (namedCoordinates == null || base.Text.IsNullOrWhiteSpace())
                return;

            //////DebugStrageties(namedCoordinates);
            //if (NamedCoordinates.FuzzyScore(this.Text, "31a. Other Fee(s)") > 50 
            //    | NamedCoordinates.FuzzyScore(this.Text, "32. Total Fee") > 70)
            //    Debugger.Break();

            //if (this.Text.Contains("Spouse")
            //    | this.Text.Contains("Dependent")
            //    )
            //    Debugger.Break();

            NamedCoordinate namedCoordinate;

            var results = namedCoordinates.MatchByFuzzyHeaderText(this);

            if (results == null)
            {
                Logger.TraceVerbose($"CoordinateBoundryAndExactTextSearch for: {this.HeaderText} - {this.Text}");
                results = namedCoordinates.MatchCoordinateBoundryAndExactText(this).FirstOrDefault();
            }

            if (results == null)
            {
                Logger.TraceVerbose($"ExactTextSearch for: {this.HeaderText} - {this.Text}");
                results = namedCoordinates.MatchByFuzzyExactText(this);
            }

            if (results == null)
            {
                Logger.TraceVerbose($"Position search for: {this.HeaderText} - {this.Text}");
                results = namedCoordinates.MatchKeyByIdealCenter((NewGeometry)this.Geometry).FirstOrDefault();
            }
                

            namedCoordinate = results; //.FirstOrDefault();

            if (namedCoordinate != null)
            {
                this.Match = namedCoordinate.Name;
                this.GroupName = namedCoordinate.GroupName;
                this.HeaderText = namedCoordinate.HeaderText;
                this.Type = namedCoordinate.Type ?? "string";
                this.TabOrder = namedCoordinate.TabOrder;
                // Required here only applies if mapped, so we need it at the claim type level
                //this.Required = namedCoordinate.Required;
                this.MinConfidence = namedCoordinate.MinConfidence;

                bool boolOut = true;
                bool f = bool.TryParse(namedCoordinate.MultiSelect, out boolOut);
                this.MultiSelect = boolOut;

                //f = bool.TryParse(namedCoordinate.Required, out boolOut);
                //this.MultiSelect = boolOut;
            }

            // Try exact match first
            //PositionedKey pk2 = PositionedKey.TestKeys.Where(n => n.ExactNameMatch == base.Text).FirstOrDefault();
            //if (pk2 != null )
            //   this.Match = pk2.Name;
        }

        [Conditional("DEBUG")]
        void DebugStrageties(NamedCoordinates namedCoordinates)
        {
            NamedCoordinate namedCoordinate;
            TextractClaimMatchStrategy strategies = Configuration.Instance.NamedCoordinates_ClaimMatch_Strategy;

            if (strategies.HasFlag(TextractClaimMatchStrategy.HeaderFuzzyText))
            {
                namedCoordinate = namedCoordinates.MatchByFuzzyHeaderText(this); //.FirstOrDefault();
            }

            if (strategies.HasFlag(TextractClaimMatchStrategy.KeyIdealCenter))
            {
                namedCoordinate = namedCoordinates.MatchKeyByIdealCenter((NewGeometry)this.Geometry).FirstOrDefault();
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

}
