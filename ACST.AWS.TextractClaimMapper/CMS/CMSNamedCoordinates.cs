
namespace ACST.AWS.TextractClaimMapper.CMS
{
    using System;
    using ACST.AWS.Common;
    using ACST.AWS.Textract.Model ;

    public class CMSNamedCoordinates
        : NamedCoordinates
    {
        public override string ConfigurationFileName => Configuration.Instance.CMSNamedCoordinates_FileName;

        public override int PageGridColumns => 86;

        public override int PageGridRows => 70; //67

        public override int TableHeaderGridPosition => 45;

        public override int TableRows => 6;

        public override int TableSubRows => 2;

        public CMSNamedCoordinates()
        {
            this.Source = NamedCoordinatesSource.ExternalXML;
            //DeserializeFromXML(this.ConfigurationFileName);
            DeserializeFromXML();
        }

        public void VerifyHeaderGridPosition()
        {

        }
    }

}
