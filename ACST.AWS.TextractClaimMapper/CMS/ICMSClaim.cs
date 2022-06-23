
namespace ACST.AWS.TextractClaimMapper.CMS
{
    using System.Collections.Generic;
    using ACST.AWS.Textract.Model;

    public interface ICMSClaim
    {
        //string AmountPaid { get; set; }
        //Field AuthorizationsProviderSignature { get; set; }
        //Field BillingProviderNameAndAddr { get; set; }
        Field InsuranceCompanNameAndAddr { get; set; }
        string InsuredAddress1 { get; set; }
        string InsuredAddress2 { get; set; }
        string InsuredCity { get; set; }
        string InsuredDOB { get; set; }
        //Field InsuredDOB_DD { get; set; }
        //Field InsuredDOB_MM { get; set; }
        //Field InsuredDOB_YY { get; set; }
        string InsuredGroupNO { get; set; }
        string InsuredID { get; set; }
        //Field InsuredName { get; set; }
        Field InsuredSexIndicator_F { get; set; }
        Field InsuredSexIndicator_M { get; set; }
        string InsuredSignature { get; set; }
        string InsuredSignatureDate { get; set; }
        string InsuredState { get; set; }
        string InsuredZip { get; set; }
        string PatientAccountNO { get; set; }
        string PatientAddress1 { get; set; }
        string PatientAddress2 { get; set; }
        string PatientCity { get; set; }
        string PatientDOB { get; set; }
        //Field PatientDOB_DD { get; set; }
        //Field PatientDOB_MM { get; set; }
        //Field PatientDOB_YY { get; set; }
        //Field PatientName { get; set; }
        Field PatientSexIndicator_F { get; set; }
        Field PatientSexIndicator_M { get; set; }
        string PatientSignature { get; set; }
        string PatientSignatureDate { get; set; }
        string PatientSSN { get; set; }
        string PatientState { get; set; }
        string PatientZip { get; set; }
        string PayeeAdditionalID { get; set; }
        string PayeeEIN { get; set; }
        string PayeeNPI { get; set; }
        Field ServiceFacilityNameAndAddr { get; set; }
        List<OCR.IServiceLine> ServiceLines { get; }
        string TotalChargeAmount { get; set; }

        void ParseCompositeValues();
        void Serialize(Page page);
    }
}