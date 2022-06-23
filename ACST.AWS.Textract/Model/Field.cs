using System;
using System.Collections.Generic;
using System.Linq;
using ACST.AWS.Common;
using Amazon.Textract.Model;

namespace ACST.AWS.Textract.Model
{
    public class Field 
		: IAWSElement, IMatchedElement, IMappedElement
	{
		#region Properties & Fields

		public MatchedFieldKey Key { get; set; }

		public FieldValue Value { get; set; }

		public NewGeometry Geometry { get; set; }

		//public string HeaderText => Key.HeaderText;

		public bool MappedToClaim { get; set; }

		public bool Required { get; set; }
		
		public bool UpdateToClaim { get; set; }

		public string Id => Key.Id;

		public float MinConfidence => Key.MinConfidence;

		//public bool Required => Key.Required;

		public string Match => Key.Match;

		public int TabOrder => Key.TabOrder;

		public string Text => Value?.Text;

		public string HeaderText { get; }

		public string GroupName { get; }
		#endregion

		public Field() { }

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Use this construction when Textract Analysis Form Key-Value's do not contain a required element specified by in NamedCoordinates.
		/// This allows backfilling the document with a dummy Field avaliable for user audit.</remarks>
		/// <param name="namedCoordinate"></param>
		public Field(NamedCoordinate namedCoordinate)
		{
			this.Key = new MatchedFieldKey(namedCoordinate);
			this.Value = new FieldValue();
			this.Geometry = new NewGeometry() { BoundingBox = namedCoordinate.ValueBoundingBox };
		}

		public Field(Block block, List<Block> blocks, NamedCoordinates namedCoordinates)
		{
			List<Geometry> componentRectangles = new List<Geometry>();

			var relationships = block.Relationships;
			if (relationships != null && relationships.Count > 0)
			{

				relationships.ForEach(r => {
					if (r.Type == "CHILD")
					{
						this.Key = new MatchedFieldKey(block, r.Ids, blocks, namedCoordinates);

						componentRectangles.Add(block.Geometry);

      //                  //if (!this.Key.HasMatch)
						//if(this.Key.Text.StartsWith("12."))
      //                  {
      //                      var results = namedCoordinates.MatchCoordinateBoundry(this);
      //                      var result = results?.FirstOrDefault();
      //                      Logger.TraceVerbose($"Field CoordinateBoundrySearch for: {this.HeaderText} - {this.Text} Results: {results?.Count()}");
      //                  }
                    }
					else if (r.Type == "VALUE")
					{
						r.Ids.ForEach(id => {
							var v = blocks.Find(b => b.Id == id);
							if (v.EntityTypes.Contains("VALUE"))
							{
								var vr = v.Relationships;
								if (vr != null && vr.Count > 0)
								{
									vr.ForEach(vc => {
										if (vc.Type == "CHILD")
										{
											this.Value = new FieldValue(v, vc.Ids, blocks);
											componentRectangles.Add(v.Geometry);
										}
									});
								}
							}
						});
					}
				});

				//componentRectangles.ForEach(r => ACST.AWS.Common.Logger.TraceVerbose($"Fields: ({r.BoundingBox.Top},{r.BoundingBox.Left}) x ({r.BoundingBox.Height},{r.BoundingBox.Width})"));

				if (componentRectangles.Count > 0)
				{
					this.Geometry = new NewGeometry() { BoundingBox = NewGeometry.EnclosingBoundingBox(componentRectangles) };
					//this.Geometry = EnclosingBoundingBox(componentRectangles.Select(g => g.BoundingBox));
				}

			}
		}

		Geometry EnclosingBoundingBox(IEnumerable<BoundingBox> boundingBoxs)
		{
			if (boundingBoxs == null || !boundingBoxs.Any())
				return null;

			float minTop = boundingBoxs.Min(g => g.Top);
			float minLeft = boundingBoxs.Min(g => g.Left);
			float maxBottom = boundingBoxs.Max(g => g.Top + g.Height);
			float maxRight = boundingBoxs.Max(g => g.Left + g.Width);

			BoundingBox bb = new BoundingBox();
			bb.Top = minTop;
			bb.Left = minLeft;
			bb.Height = Math.Abs(maxBottom - minTop);
			bb.Width = Math.Abs(maxRight - minLeft);

			return new Geometry { BoundingBox = bb };
		}

		//public static BoundingBox EnclosingBoundingBox(IEnumerable<BoundingBox> boundingBoxs)
		//{
		//	if (boundingBoxs == null || !boundingBoxs.Any())
		//		return null;

		//	float minTop = boundingBoxs.Min(g => g.Top);
		//	float minLeft = boundingBoxs.Min(g => g.Left);
		//	float maxBottom = boundingBoxs.Max(g => g.Top + g.Height);
		//	float maxRight = boundingBoxs.Max(g => g.Left + g.Width);

		//	BoundingBox bb = new BoundingBox();
		//	bb.Top = minTop;
		//	bb.Left = minLeft;
		//	bb.Height = Math.Abs(maxBottom - minTop);
		//	bb.Width = Math.Abs(maxRight - minLeft);

		//	//ACST.AWS.Common.Logger.TraceVerbose($"Outside: ({bb.Top},{bb.Left}) x ({bb.Height},{bb.Width})");

		//	return bb;
		//}

		public override string ToString()
		{
			return $"FieldKey: {this.Key?.ToString()}" + Environment.NewLine
				+ $"Value: {this.Value?.ToString()}";
		}
	}
}