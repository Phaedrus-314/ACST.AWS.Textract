
namespace ACST.AWS.Common
{
    using System;

    public enum ArchiveType
    {
        Error,
        Review,
        Skip,
        Success
    }

    public enum TextractClaimYesNo
    {
        Unknown,
        Yes,
        No
    }

    public enum TextractClaimGender
    {
        Unknown,
        Female,
        Male
    }

    public enum TextractClaimRelationship
    {
        Unknown,
        Self,
        Spouse,
        Dependent,
        Other
    }

    public enum TextractImageType
    {
        Unknown,
        JPG,
        PDF
    }

    public enum TextractClaimType
    {
        Unknown,
        DENTAL,
        MEDICALI,
        MEDICALP
    }

    //public enum TextractFieldType
    //{
    //    String,
    //    Bool,
    //    Int,
    //    Long,
    //    DateTime
    //}

    public enum ClaimFormType
    {
        Unknown,
        ADA2006,
        ADA2012,
        //ADA2019,
        CMS
    }

    public enum NamedCoordinatesSource
    {
        InternalDefault,
        ExternalXML,
        ExternalJSON
    }

    [Flags]
    public enum TextractElements
    {
        None = 0,
        Page = 1 << 0,
        Line = 1 << 1,
        Form = 1 << 2,
        Field = 1 << 3,
        FieldKey = 1 << 4,
        FieldValue = 1 << 5,
        Table = 1 << 6,
        Row = 1 << 7,
        Cell = 1 << 8,
        SelectionElement = 1 << 9
    }

    [Flags]
    public enum TextractViewerAdornments
    {
        None = 0,
        CoordinateBox = 1 << 0,
        CoordinateCenter = 1 << 1
    }

    [Flags]
    public enum TextractClaimMatchStrategy
    {
        None = 0,
        HeaderFuzzyText = 1 << 0,
        HeaderExactText = 1 << 1,
        KeyIdealCenter = 1 << 2,
        ValueIdealCenter = 1 << 3,
        FieldIdealCenter = 1 << 4
    }

    [Flags]
    public enum TextractClaimTableRowType
    {
        Unknown = 0,
        HeaderRow = 1 << 0,
        TableHeaderRow = 1 << 1,
        ColumnHeaderRow = 1 << 2,
        DataRow = 1 << 3,
        FirstDataRow = 1 << 4,
        LastDataRow = 1 << 5
    }
}
