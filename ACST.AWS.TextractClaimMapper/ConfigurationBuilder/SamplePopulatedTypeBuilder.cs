
namespace ACST.AWS.TextractClaimMapper.ConfigurationBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Utility class usefule only for creating BTS Schema without XSD.  
    /// Not used by Textract Process
    /// </summary>
    internal class SamplePopulatedTypeBuilder
    {

        public static ADA.ADAClaim GenerateDental()
        {
            ADA.ADAClaim claim = new ADA.ADAClaim();

            claim.AmountPaid = "1234.56";
            claim.BenefitsAssignCertIndicator = "Y";
            //claim.BillingDentistNameAndAddr = "c";
            claim.ClaimFilingIndicatorCode = "15";
            claim.ClaimFrequencyCode = "1";
            claim.ClaimNo = "66677788";
            //claim.ClaimType = "c";
            claim.DelayReasonCode = "c"; // blank
            claim.DiagnosisCode1 = "D1234";
            claim.DiagnosisCode2 = "D1234";
            claim.DiagnosisCode3 = "D1234";
            claim.DiagnosisCode4 = "D1234";
            claim.DiagnosisCode5 = "D1234";
            //claim.InsuranceCompanNameAndAddr = "c";
            claim.InsuredAddress1 = "Line 1";
            claim.InsuredAddress2 = "Line 2";
            claim.InsuredCity = "city";
            claim.InsuredDOB = "2020-06-05";
            claim.InsuredFirstName = "fName";
            claim.InsuredGroupNO = "3336956";
            claim.InsuredID = "c12345678";
            claim.InsuredLastName = "iLast";
            claim.InsuredMName = "iMiddle";
            //claim.InsuredNameAndAddr = "c";
            //claim.InsuredSex = ACST.AWS.Common.TextractClaimGender.Unknown;
            //claim.InsuredSexIndicator_F = "c";
            //claim.InsuredSexIndicator_M = "c";
            claim.InsuredSignature = "iSignatu";
            claim.InsuredSignatureDate = "2020-06-05";
            claim.InsuredState = "GA";
            claim.InsuredZip = "12345-6543";
            claim.MedicareAssignmentCode = "A";
            claim.PatientAccountNO = "PDH1009422150";
            claim.PatientAddress1 = "line1";
            claim.PatientAddress2 = "line2";
            claim.PatientCity = "city";
            claim.PatientDOB = "2020-06-05";
            claim.PatientFirstName = "fName";
            claim.PatientLastName = "lName";
            claim.PatientMName = "mName";
            //claim.PatientNameAndAddr = "c";
            claim.PatientNameSuffix = "JSd";
            //claim.PatientSex = "c";
            //claim.PatientSexIndicator_F = "c";
            //claim.PatientSexIndicator_M = "c";
            claim.PatientSignature = "pat sig";
            claim.PatientSignatureDate = "2020-06-05";
            claim.PatientSSN = "c123456";
            claim.PatientState = "GA";
            claim.PatientZip = "12345-6789";
            claim.PayeeAdditionalID = "c123312";
            claim.PayeeAddress1 = "line1";
            claim.PayeeCity = "city";
            claim.PayeeEIN = "c123456";
            claim.PayeeFirstName = "fName";
            claim.PayeeName = "lName";
            claim.PayeeLicenseNo = "c123456";
            claim.PayeeMName = "mName";
            claim.PayeeNPI = "c12345678";
            claim.PayeePhoneNo = "6785551122";
            claim.PayeeState = "GA";
            claim.PayeeTypeQualifier = "o1";
            claim.PayeeZip = "12345-6789";
            claim.PayerID = "c123456";
            claim.PayerLevelOfResponsibility = "P";
            claim.PayerName = "Payer first and last name";
            claim.PrincipalDiagnosisCode = "cssss";
            claim.ProviderAdditionalID = "c123456";
            claim.ProviderAddress1 = "line1";
            claim.ProviderCity = "city";
            claim.ProviderFirstName = "fName";
            claim.ProviderName = "lName";
            claim.ProviderLicenseNo = "c123456";
            claim.ProviderMName = "mName";
            claim.ProviderNPI = "c12345678";
            claim.ProviderPhoneNo = "6785551122";
            claim.ProviderSignatureDate = "2020-06-05";
            claim.ProviderSignIndicator = true;
            claim.ProviderSpecialityCode = "c34423";
            claim.ProviderState = "GA";
            claim.ProviderTypeQualifier = "o2";
            claim.ProviderZip = "12345-6789";
            claim.ReceiverID = "c123456";
            claim.ReceiverName = "receiver first and last name";
            claim.RelatedClaimNo = "none";
            claim.PatientRelationshipCode = Common.TextractClaimRelationship.Self;

            claim.ReleaseOfInfoCode = "Y";
            claim.Remarks = "skljfsdkfjlkdfsas asf sdfa dc";
            claim.ClearinghouseRefNo = "ferwer23423";
            claim.SpecialProgramIndicator = true;
            //claim.Specification = "c"; 
            claim.Status = "new";
            claim.SubmitterContactName = "contact name";
            claim.SubmitterID = "c3342233";
            claim.SubmitterLastName = "lName";
            claim.SubmitterNO1 = "n232323223";
            claim.SubmitterNO2 = "c2332332";
            claim.SubmitterNOQualifier1 = "c2";
            claim.SubmitterNOQualifier2 = "c2";
            claim.SubmitterTypeQualifier = "c2";
            claim.TotalChargeAmount = "12345.67";
            claim.Tracking = "not used";
            //claim.TreatingDentistAddr = "c";
            //claim.TreatingDentistFullName = "c";


            ADA.ADAServiceLine s1 = new ADA.ADAServiceLine();
            s1.LineItemNO = "1";

            s1.ProductIDQualifier = "s1"; // len 2
            s1.ProcedureCode = "s123s"; // len 5

            //s1.ProcedureDate = "s1";
            s1.ProcedureModifier1 = "s1";
            s1.ProcedureModifier2 = "s2";
            s1.ProcedureModifier3 = "s3";
            s1.ProcedureModifier4 = "s4";

            s1.DiagnosisCodePointer1 = "1a"; // len 2
            s1.DiagnosisCodePointer2 = "1a";
            s1.DiagnosisCodePointer3 = "1a";
            s1.DiagnosisCodePointer4 = "1a";
            //s1.DiagnosisCodePointers = "s1";

            s1.ChargedAmount = "123423.55";

            s1.UnitsMeasurementCode = "UM"; //len2
            s1.ServiceUnitCount = "65"; // decimal

            s1.NoteReferenceCode = "s12"; //len 3
            s1.LineNoteText = "s1 sdf jsdklf sdlk";

            s1.ServiceEndDate = "2020-06-05";
            s1.ServiceStartDate = "2020-06-05";

            s1.CavityDesignationCode1 = "AAA"; // len 3
            s1.CavityDesignationCode2 = "AAA";
            s1.CavityDesignationCode3 = "AAA";
            s1.CavityDesignationCode4 = "AAA";
            //s1.CavityDesignationCodes = "s1";

            s1.OralCavityArea = "s1";

            s1.ToothCode = "s1sfsdfsdfs"; // len 30
            s1.ToothSurfaceCode1 = "s1"; // len 2
            s1.ToothSurfaceCode2 = "s1";
            s1.ToothSurfaceCode3 = "s1";
            s1.ToothSurfaceCode4 = "s1";
            //s1.ToothSurfaces = "s1";

            s1.LineItemControlNO = "sdfssd232332"; // len 30

            ADA.ADAServiceLine s2 = new ADA.ADAServiceLine();
            s2.LineItemNO = "2";
            s2.ProductIDQualifier = "s2"; // len 2
            s2.ProcedureCode = "s223s"; // len 5
            //s2.ProcedureDate = "s2";
            s2.ProcedureModifier1 = "s2";
            s2.ProcedureModifier2 = "s2";
            s2.ProcedureModifier3 = "s3";
            s2.ProcedureModifier4 = "s4";
            s2.DiagnosisCodePointer1 = "2a"; // len 2
            s2.DiagnosisCodePointer2 = "2a";
            s2.DiagnosisCodePointer3 = "2a";
            s2.DiagnosisCodePointer4 = "2a";
            //s2.DiagnosisCodePointers = "s2";
            s2.ChargedAmount = "123423.55";
            s2.UnitsMeasurementCode = "UM"; //len2
            s2.ServiceUnitCount = "65"; // decimal
            s2.NoteReferenceCode = "s22"; //len 3
            s2.LineNoteText = "s2 sdf jsdklf sdlk";
            s2.ServiceEndDate = "2020-06-05";
            s2.ServiceStartDate = "2020-06-05";
            s2.CavityDesignationCode1 = "AAA"; // len 3
            s2.CavityDesignationCode2 = "AAA";
            s2.CavityDesignationCode3 = "AAA";
            s2.CavityDesignationCode4 = "AAA";
            //s2.CavityDesignationCodes = "s2";
            s2.OralCavityArea = "s2";
            s2.ToothCode = "s2sfsdfsdfs"; // len 30
            s2.ToothSurfaceCode1 = "s2"; // len 2
            s2.ToothSurfaceCode2 = "s2";
            s2.ToothSurfaceCode3 = "s2";
            s2.ToothSurfaceCode4 = "s2";
            //s2.ToothSurfaces = "s2";
            s2.LineItemControlNO = "sdfssd232332"; // len 30


            claim.ServiceLines.Add(s1);
            claim.ServiceLines.Add(s2);


            return claim;
        }
    }
}
