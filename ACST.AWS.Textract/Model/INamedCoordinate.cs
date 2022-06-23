
namespace ACST.AWS.Textract.Model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Amazon.Textract.Model;

    using ACST.AWS.Common;

    public interface INamedCoordinate
    {
        string Name { get; set; }

        string ExactTextMatch { get; set; }

        string GroupName { get; set; }

        //bool TableStructure { get; set; }

        byte Column { get; set; }

        byte Width { get; set; }

        byte Row { get; set; }

        PointF IdealCenterValue { get; set; }

        PointF IdealCenterKey { get; set; }

        BoundingBox FieldBoundingBox { get; set; }

        BoundingBox ValueBoundingBox { get; set; }

        NamedCoordinatesSource Source { get; set; }

        string NamedCoordinatesOverrideFileName { get; }
    }
}
