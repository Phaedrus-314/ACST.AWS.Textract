
namespace ACST.AWS.TextractClaimMapper.CMS
{
    using System;
    using System.Linq;
    using ACST.AWS.Textract.Model;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;

    [XmlRoot("ocrCMSclaim", Namespace = "http://ACST.BizTalk.Edi.Schemas.a3500")]
    public class CMSClaim
        : OCR.BaseClaim 
    {
        public List<CMSServiceLine> ServiceLines { get; } = new List<CMSServiceLine>();

        public static readonly TextractClaimType InstanceType = TextractClaimType.MEDICALP;

        protected override int ServiceLinesCount => 6;

        #region Property Overrides

        //[PositionalValueAttribute("Provider_AccountNo", true)]
        [PositionalValueAttribute("Provider_AccountNo")]
        public override string PatientAccountNO { get; set; }

        [PositionalValueAttribute("Provider_EIN")]
        public override string PayeeEIN { get; set; }

        [PositionalValueAttribute("BillingProvider_ID")]
        public override string PayeeNPI { get; set; }

        [PositionalValueAttribute("BillingProvider_AdditionalID")]
        public override string PayeeAdditionalID { get; set; }

        [PositionalValueAttribute("Subscriber_Addr_Street")]
        public override string InsuredAddress1 { get; set; }

        [PositionalValueAttribute("Subscriber_Addr_City")]
        public override string InsuredCity { get; set; }

        [PositionalValueAttribute("Subscriber_Addr_State")]
        public override string InsuredState { get; set; }

        [PositionalValueAttribute("Subscriber_Addr_Zip")]
        public override string InsuredZip { get; set; }

        [PositionalValueAttribute("Subscriber_PlanNo")]
        public override string InsuredGroupNO { get; set; }

        [PositionalValueAttribute("Subscriber_ID")]
        public override string InsuredID { get; set; }

        [PositionalValueAttribute("Authorizations_InsuredSignatureDate")]
        public override string InsuredSignatureDate { get; set; }

        [PositionalValueAttribute("Authorizations_InsuredSignature")]
        public override string InsuredSignature { get; set; }

        [PositionalValueAttribute("Patient_Addr_Street")]
        public override string PatientAddress1 { get; set; }

        [PositionalValueAttribute("Patient_Addr_City")]
        public override string PatientCity { get; set; }

        [PositionalValueAttribute("Patient_Addr_State")]
        public override string PatientState { get; set; }

        [PositionalValueAttribute("Patient_Addr_Zip")]
        public override string PatientZip { get; set; }

        [PositionalValueAttribute("Patient_SSN")]
        public override string PatientSSN { get; set; }

        [PositionalValueAttribute("Authorizations_PatientSignatureDate")]
        public override string PatientSignatureDate { get; set; }

        [PositionalValueAttribute("Authorizations_PatientSignature")]
        public override string PatientSignature { get; set; }

        [PositionalValueAttribute("Provider_AmountPaid")]
        public override string AmountPaid { get; set; }

        //[PositionalValueAttribute("Provider_TotalCharge", true)]
        //[PositionalValueAttribute("Provider_TotalCharge")]
        //public override decimal TotalChargeAmount { get; set; }
        #endregion

        #region Composite Properties

        [XmlIgnore]
        [PositionalValueAttribute("InsuranceCompany_NameAndAddr")]
        public Field InsuranceCompanNameAndAddr { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_Name")]
        public Field InsuredName { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_Sex_F")]
        public Field InsuredSexIndicator_F { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_Sex_M")]
        public Field InsuredSexIndicator_M { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_DOB_MM")]
        public Field InsuredDOB_MM { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_DOB_DD")]
        public Field InsuredDOB_DD { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_DOB_YY")]
        public Field InsuredDOB_YY { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Patient_Name")]
        public Field PatientName { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Patient_Sex_F")]
        public Field PatientSexIndicator_F { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Patient_Sex_M")]
        public Field PatientSexIndicator_M { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Patient_DOB_MM")]
        public Field PatientDOB_MM { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Patient_DOB_DD")]
        public Field PatientDOB_DD { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Patient_DOB_YY")]
        public Field PatientDOB_YY { get; set; }


        [XmlIgnore]
        [PositionalValueAttribute("Authorizations_ProviderSignature")]
        public Field AuthorizationsProviderSignature { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("ServiceFacility_NameAndAddr")]
        public Field ServiceFacilityNameAndAddr { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("BillingProvider_NameAndAddr")]
        public Field BillingProviderNameAndAddr { get; set; }


        #endregion

        public CMSClaim()
        {
            //this.ClaimType = "MEDICALP";
            this.ClaimType = InstanceType;
        }

        [CompositeValueParserAttribute]
        public void ParseCompositeValues()
        {

            var nc = ParseName(this.InsuredName.Value.Content);

            if (nc.LastName.IsNullOrWhiteSpace())
                throw new RequiredPropertyException("InsuredLastName");

            this.InsuredFirstName = nc.FirstName;
            this.InsuredLastName = nc.LastName;
            this.InsuredMName = nc.MiddleName;

            nc = ParseName(this.PatientName.Value.Content);
            this.PatientFirstName = nc.FirstName;
            this.PatientLastName = nc.LastName;
            this.PatientMName = nc.MiddleName;

            Nullable<DateTime> dt = BuildDateValue(this.InsuredDOB_MM?.Text, this.InsuredDOB_DD?.Text, this.InsuredDOB_YY?.Text);
            if (dt.HasValue) this.InsuredDOB = dt.Value.ToShortDateString();

            dt = BuildDateValue(this.PatientDOB_MM?.Text, this.PatientDOB_DD?.Text, this.PatientDOB_YY?.Text);
            if (dt.HasValue) this.PatientDOB = dt.Value.ToShortDateString();

            this.InsuredSex = BuildSexValue(this.InsuredSexIndicator_F?.Text, this.InsuredSexIndicator_M?.Text);

            this.PatientSex = BuildSexValue(this.PatientSexIndicator_F?.Text, this.PatientSexIndicator_M?.Text);

            var nac = ParseNameAddr(this.BillingProviderNameAndAddr?.Value, true);
            this.PayeeFirstName = nac.NameComponent.FirstName;
            this.PayeeName = nac.NameComponent.LastName;
            this.PayeeAddress1 = nac.AddressComponent.AddressLine1;
            this.PayeeCity = nac.AddressComponent.City;
            this.PayeeState = nac.AddressComponent.State;
            this.PayeeZip = nac.AddressComponent.Zip;

            this.ProviderName = ParseName(this.AuthorizationsProviderSignature?.Value.Content, true).LastName;

            //if (this.ProviderLastName.IsNullOrWhiteSpace())
            //    throw new Common.RequiredPropertyException("ProviderLastName");
            //this.InsuredSex = 
            //var r = this.InsuredSexIndicator_F;

            // This should be called at the end of overriden parse
            base.ParseCompositeValues();
        }

        public override void Map(Page page)
        {
            try
            {
                base.Map(page);

                for (int i = 1; i <= this.ServiceLinesCount; i++)
                {
                    CMSServiceLine l = new CMSServiceLine();

                    var matchedLines = page.Lines.Where(e => e.HasMatch & e.MatchedTableRow == i);

                    l.Map(matchedLines.ToList(), i);

                    if (l.IsValid)
                    {
                        l.LineItemNO = i.ToString();
                        this.ServiceLines.Add(l);
                    }
                }
            }
            catch (System.Reflection.TargetInvocationException t)
            {
                Logger.TraceError(t.ToString());
            }
            catch (RequiredPropertyException rex)
            {
                Logger.TraceError(rex.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
