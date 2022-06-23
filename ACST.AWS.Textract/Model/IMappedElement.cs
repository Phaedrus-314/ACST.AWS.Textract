
namespace ACST.AWS.Textract.Model
{
    using System;

    public interface IMappedElement
    {
        bool MappedToClaim { get; set; }

        bool UpdateToClaim { get; set; }

        bool Required { get; set; }
        //string Type { get; }

        //bool MultiSelect { get; }

        //string HeaderText { get; }

        //string GroupName { get; }

        //int TabOrder { get; }

    }
}
