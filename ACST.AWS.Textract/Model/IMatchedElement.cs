
namespace ACST.AWS.Textract.Model
{
    using System;

    public interface IMatchedElement
    {
        //ToDo: factor out some of this, MappedToClaim, Required, & Match are necessary 

        string Match { get; }

        //bool MappedToClaim { get; set; }
        string HeaderText { get; }

        string GroupName { get; }

        float MinConfidence { get; }

        // Required here only applies if mapped, so we need it at the claim type level
        //bool Required { get; }

    }
}
