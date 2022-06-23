
namespace ACST.AWS.TextractClaimMapper.ADA
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using ACST.AWS.Common;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.TextractClaimMapper.OCR;

    [XmlRoot("ocrADAclaim", Namespace = "http://ACST.BizTalk.Edi.Schemas.a3500")]
    public class ADAClaim
        : OCR.BaseClaim 
    {

        #region HIPAA / CAMS Properties

        public static readonly TextractClaimType InstanceType = TextractClaimType.DENTAL;

        public List<ADAServiceLine> ServiceLines { get; } = new List<ADAServiceLine>();

        protected override int ServiceLinesCount => 10;

        public override string Specification { get => "005010X224A2"; }

        public override string Tracking { get => "NotTracked"; }

        public override string Status { get => "new"; }

        public override bool Priced { get => true; }

        public override string RelatedClaimNo { get => "none"; }

        public override string PayerLevelOfResponsibility { get => "P"; }

        public override string BenefitsAssignCertIndicator { get => "Y"; }

        public override string PricingMethodology { get => "02"; set => base.PricingMethodology = value; }

        public override string ClearinghouseRefNo { get => this.ClaimNo; set => base.ClearinghouseRefNo = value; }
        #endregion

        #region Property Overrides

        #region Treating Dentist

        [XmlIgnore]
        [PositionalValueAttribute("TreatingDentist_Name_Signature", true)]
        public Field TreatingDentistFullName { get; set; }

        [CompositeValueAttribute("TreatingDentist_Name_Signature", true)]
        public override string ProviderName { get; set; }

        //[CompositeValueAttribute("TreatingDentist_Name_Signature")]
        //public override string ProviderFirstName { get; set; }

        //[CompositeValueAttribute("TreatingDentist_Name_Signature")]
        //public override string ProviderMName { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("TreatingDentist_Addr", true)]
        public Field TreatingDentistAddr { get; set; }

        [CompositeValueAttribute("TreatingDentist_Addr", true)]
        public override string ProviderAddress1 { get; set; }

        [CompositeValueAttribute("TreatingDentist_Addr")]
        public override string ProviderCity { get; set; }

        [CompositeValueAttribute("TreatingDentist_Addr")]
        public override string ProviderState { get; set; }

        [CompositeValueAttribute("TreatingDentist_Addr")]
        public override string ProviderZip { get; set; }

        string _ProviderPhoneNo;
        [PositionalValueAttribute("TreatingDentist_PhoneNo")]
        public override string ProviderPhoneNo { get => _ProviderPhoneNo; set => _ProviderPhoneNo = value.Filter(char.IsDigit); }

        [PositionalValueAttribute("TreatingDentist_SpecialityCode")]
        public override string ProviderSpecialityCode { get; set; }

        [PositionalValueAttribute("TreatingDentist_LicenseNo")]
        public override string ProviderLicenseNo { get; set; }

        [PositionalValueAttribute("TreatingDentist_NPI")]
        public override string ProviderNPI { get; set; }

        // ToDo: choose which provider gets EIN or use addition id
        public override string ProviderEIN { get => _PayeeEIN; set => base.ProviderEIN = value; }

        [PositionalValueAttribute("TreatingDentist_AdditionalID")]
        public override string ProviderAdditionalID { get; set; }

        string _ProviderSignatureDate;
        [PositionalValueAttribute("TreatingDentist_SignatureDate")]
        public override string ProviderSignatureDate { get => _ProviderSignatureDate; set => _ProviderSignatureDate = value.ToDateFormat(); }
        #endregion

        #region Billing Dentist

        [XmlIgnore]
        [PositionalValueAttribute("BillingDentist_NameAndAddr", true)]
        public Field BillingDentistNameAndAddr { get; set; }

        [CompositeValueAttribute("BillingDentist_NameAndAddr", true)]
        public override string PayeeName { get; set; }

        //[CompositeValueAttribute("BillingDentist_NameAndAddr")]
        //public override string PayeeFirstName { get; set; }

        //[CompositeValueAttribute("BillingDentist_NameAndAddr")]
        //public override string PayeeMName { get; set; }

        [CompositeValueAttribute("BillingDentist_NameAndAddr")]
        public override string PayeeAddress1 { get; set; }

        [CompositeValueAttribute("BillingDentist_NameAndAddr")]
        public override string PayeeCity { get; set; }

        [CompositeValueAttribute("BillingDentist_NameAndAddr")]
        public override string PayeeState { get; set; }

        [CompositeValueAttribute("BillingDentist_NameAndAddr")]
        public override string PayeeZip { get; set; }

        string _PayeePhoneNo;
        [PositionalValueAttribute("BillingDentist_PhoneNo")]
        public override string PayeePhoneNo { get => _PayeePhoneNo; set => _PayeePhoneNo = value.Filter(char.IsDigit); }

        string _PayeeEIN;
        [PositionalValueAttribute("BillingDentist_EIN", true)]
        public override string PayeeEIN { get => _PayeeEIN; set => _PayeeEIN = value.Filter(char.IsDigit); }

        [PositionalValueAttribute("BillingDentist_LicenseNo")]
        public override string PayeeLicenseNo { get; set; }

        [PositionalValueAttribute("BillingDentist_NPI")]
        public override string PayeeNPI { get; set; }

        [PositionalValueAttribute("BillingDentist_AdditionalID")]
        public override string PayeeAdditionalID { get; set; }
        #endregion

        #region InsuranceCompany

        [XmlIgnore]
        [PositionalValueAttribute("InsuranceCompany_NameAndAddr")]
        public Field InsuranceCompanyNameAndAddr { get; set; }

        [CompositeValueAttribute("InsuranceCompany_NameAndAddr")]
        public override string ReceiverName { get; set; }

        //[XmlIgnore]
        //[CompositeValueAttribute("InsuranceCompany_NameAndAddr")]
        //public override string ReceiverAddress1 { get; set; }

        //[XmlIgnore]
        //[CompositeValueAttribute("InsuranceCompany_NameAndAddr")]
        //public override string ReceiverCity { get; set; }

        //[XmlIgnore]
        //[CompositeValueAttribute("InsuranceCompany_NameAndAddr")]
        //public override string ReceiverState { get; set; }

        //[XmlIgnore]
        //[CompositeValueAttribute("InsuranceCompany_NameAndAddr")]
        //public override string ReceiverZip { get; set; }
        #endregion

        #region OtherCoverage

        [MappedProperty("OtherCoverage_Dental")]
        public bool OtherCoverageDental { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherCoverage_Dental", "OtherCoverage_Type")]
        public Field OtherCoverage_Dental { get; set; }

        [MappedProperty("OtherCoverage_Medical")]
        public bool OtherCoverageMedical { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherCoverage_Medical", "OtherCoverage_Type")]
        public Field OtherCoverage_Medical { get; set; }


        [XmlIgnore]
        [PositionalValueAttribute("OtherSubscriber_Name")]
        public Field OtherSubscriberName { get; set; }

        [CompositeValueAttribute("OtherSubscriber_Name")]
        public string OtherSubscriberFirstName { get; set; }

        [CompositeValueAttribute("OtherSubscriber_Name")]
        public string OtherSubscriberLastName { get; set; }

        [CompositeValueAttribute("OtherSubscriber_Name")]
        public string OtherSubscriberMName { get; set; }


        string _OtherSubscriberDOB;
        [PositionalValueAttribute("OtherSubscriber_DOB")]
        public string OtherSubscriberDOB { get => _OtherSubscriberDOB; set => _OtherSubscriberDOB = value.ToDateFormat(); }

        string _OtherSubscriberID;
        [PositionalValueAttribute("OtherSubscriber_ID")]
        public string OtherSubscriberID { get => _OtherSubscriberID; set => _OtherSubscriberID = value.Filter(char.IsDigit); }


        [MappedProperty("OtherSubscriberGender")]
        public TextractClaimGender OtherSubscriberSex { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherSubscriber_Sex_F", "OtherSubscriberGender")]
        public Field OtherSubscriberSexIndicator_F { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherSubscriber_Sex_M", "OtherSubscriberGender")]
        public Field OtherSubscriberSexIndicator_M { get; set; }


        [PositionalValueAttribute("OtherSubscriber_PlanNo")]
        public string OtherSubscriberGroupNO { get; set; }


        [MappedProperty("OtherSubscriberRelationship")]
        public TextractClaimRelationship OtherSubscriberRelationshipCode { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherSubscriber_Self", "OtherSubscriberRelationship")]
        public Field OtherSubscriberRelationshipCode_Self { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherSubscriber_Spouse", "OtherSubscriberRelationship")]
        public Field OtherSubscriberRelationshipCode_Spouse { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherSubscriber_Dependent", "OtherSubscriberRelationship")]
        public Field OtherSubscriberRelationshipCode_Dependent { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("OtherSubscriber_Other", "OtherSubscriberRelationship")]
        public Field OtherSubscriberRelationshipCode_Other { get; set; }


        [XmlIgnore]
        [PositionalValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public Field OtherCoverageInsureNameAndAddr { get; set; }

        [CompositeValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public string OtherCoverageInsureName { get; set; }

        [CompositeValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public string OtherCoverageInsureAddress1 { get; set; }

        [CompositeValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public string OtherCoverageInsureAddress2 { get; set; }

        [CompositeValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public string OtherCoverageInsureCity { get; set; }

        [CompositeValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public string OtherCoverageInsureState { get; set; }

        [CompositeValueAttribute("OtherCoverageInsure_NameAndAddr")]
        public string OtherCoverageInsureZip { get; set; }
        #endregion

        #region Subscriber / Insured

        [XmlIgnore]
        [PositionalValueAttribute("Subscriber_NameAndAddr", true)]
        public Field InsuredNameAndAddr { get; set; }

        [CompositeValueAttribute("Subscriber_NameAndAddr", true)]
        public override string InsuredFirstName { get => base.InsuredFirstName; set => base.InsuredFirstName = value; }

        [CompositeValueAttribute("Subscriber_NameAndAddr", true)]
        public override string InsuredLastName { get => base.InsuredLastName; set => base.InsuredLastName = value; }

        [CompositeValueAttribute("Subscriber_NameAndAddr")]
        public override string InsuredMName { get => base.InsuredMName; set => base.InsuredMName = value; }

        [CompositeValueAttribute("Subscriber_NameAndAddr")]
        public override string InsuredAddress1 { get; set; }

        [CompositeValueAttribute("Subscriber_NameAndAddr")]
        public override string InsuredAddress2 { get; set; }

        [CompositeValueAttribute("Subscriber_NameAndAddr")]
        public override string InsuredCity { get; set; }

        [CompositeValueAttribute("Subscriber_NameAndAddr")]
        public override string InsuredState { get; set; }

        [CompositeValueAttribute("Subscriber_NameAndAddr")]
        public override string InsuredZip { get; set; }

        [PositionalValueAttribute("Subscriber_PlanNo")]
        public override string InsuredGroupNO { get; set; }

        string _InsuredDOB;
        [PositionalValueAttribute("Subscriber_DOB")]
        public override string InsuredDOB { get => _InsuredDOB; set => _InsuredDOB = value.ToDateFormat(); }

        string _InsuredID;
        [PositionalValueAttribute("Subscriber_ID", true)]
        public override string InsuredID { get => _InsuredID; set => _InsuredID = value; }
        //public override string InsuredID { get => _InsuredID; set => _InsuredID = value.Filter(char.IsDigit); }

        [PositionalValueAttribute("Subscriber_Employer")]
        public override string EmployerName { get; set; }

        [MappedProperty("SubscriberGender")]
        public override TextractClaimGender InsuredSex { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Subscriber_Sex_F", "SubscriberGender")]
        public Field InsuredSexIndicator_F { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Subscriber_Sex_M", "SubscriberGender")]
        public Field InsuredSexIndicator_M { get; set; }
        #endregion

        #region Patient Relationship

        [MappedProperty("PatientRelationship", true)]
        public override TextractClaimRelationship PatientRelationshipCode { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Patient_Self", "PatientRelationship", true)]
        public Field PatientRelationshipCode_Self { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Patient_Spouse", "PatientRelationship", true)]
        public Field PatientRelationshipCode_Spouse { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Patient_Dependent", "PatientRelationship", true)]
        public Field PatientRelationshipCode_Dependent { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Patient_Other", "PatientRelationship", true)]
        public Field PatientRelationshipCode_Other { get; set; }
        #endregion

        #region Patient

        [XmlIgnore]
        [PositionalValueAttribute("Patient_NameAndAddr")]
        public Field PatientNameAndAddr { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr", true)]
        public override string PatientLastName { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr", true)]
        public override string PatientFirstName { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientMName { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientNameSuffix { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientAddress1 { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientAddress2 { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientCity { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientState { get; set; }

        [CompositeValueAttribute("Patient_NameAndAddr")]
        public override string PatientZip { get; set; }

        string _PatientDOB;
        [PositionalValueAttribute("Patient_DOB")]
        public override string PatientDOB { get => _PatientDOB; set => _PatientDOB = value.ToDateFormat(); }
        //public override string PatientDOB { get => _PatientDOB; set => _PatientDOB = value.ToString("o"); }

        //string _PatientSSN;
        //[PositionalValueAttribute("Patient_SSN")]
        //public override string PatientSSN { get => _PatientSSN; set => _PatientSSN = value.Filter(char.IsDigit); }

        [MappedProperty("PatientGender")]
        public override TextractClaimGender PatientSex { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Patient_Sex_F", "PatientGender")]
        public Field PatientSexIndicator_F { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("Patient_Sex_M", "PatientGender")]
        public Field PatientSexIndicator_M { get; set; }

        string _PatientID;
        [PositionalValueAttribute("Patient_ID", true)]
        public override string PatientID { get => _PatientID; set => _PatientID = value.Filter(char.IsDigit); }

        //string _PatientAccountNO;
        [PositionalValueAttribute("PatientAccountNO")]
        public override string PatientAccountNO { get => _PatientID; }
        #endregion

        #region Insured/Patient Signatures
        
        string _InsuredSignatureDate;
        [PositionalValueAttribute("Authorizations_InsuredSignatureDate")]
        public override string InsuredSignatureDate { get => _InsuredSignatureDate; set => _InsuredSignatureDate = value.ToDateFormat(); }

        [PositionalValueAttribute("Authorizations_InsuredSignature")]
        public override string InsuredSignature { get; set; }

        string _PatientSignatureDate;
        [PositionalValueAttribute("Authorizations_PatientSignatureDate")]
        public override string PatientSignatureDate { get => _PatientSignatureDate; set => _PatientSignatureDate = value.ToDateFormat(); }

        [PositionalValueAttribute("Authorizations_PatientSignature")]
        public override string PatientSignature { get; set; }
        #endregion

        #region Ancillary Info

        string _Enclosures;
        [PositionalValueAttribute("AncillaryInfo_Enclosures")]
        public string Enclosures { get => _Enclosures; set => ParseYesNoBox(value); }

        [PositionalValueAttribute("AncillaryInfo_Location")]
        public string PlaceOfTreatment { get; set; }

        //AncillaryInfo_Orthadontics_No

        [MappedProperty("AncillaryInfo_Orthadontics")]
        public override TextractClaimYesNo AncillaryInfoOrthadontics { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("AncillaryInfo_Orthadontics_No", "AncillaryInfo_Orthadontics")]
        public Field AncillaryInfoOrthadontics_No { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("AncillaryInfo_Orthadontics_Yes", "AncillaryInfo_Orthadontics")]
        public Field AncillaryInfoOrthadontics_Yes { get; set; }

        //AncillaryInfo_Replacement
        [MappedProperty("AncillaryInfo_Replacement")]
        public override TextractClaimYesNo AncillaryInfoReplacement { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("AncillaryInfo_Replacement_No", "AncillaryInfo_Replacement")]
        public Field AncillaryInfoReplacement_No { get; set; }

        [XmlIgnore]
        [GroupFieldAttribute("AncillaryInfo_Replacement_Yes", "AncillaryInfo_Replacement")]
        public Field AncillaryInfoReplacement_Yes { get; set; }

        #endregion

        #region Table

        string _TotalChargeAmount;
        [MappedProperty("Services_TotalFee", true)]
        public override string TotalChargeAmount { get => _TotalChargeAmount; set => _TotalChargeAmount = value.CleanupForCurrency(); }
        //public override decimal TotalChargeAmount { get; set; }

        [XmlIgnore]
        [PositionalValueAttribute("Services_TotalFee", true)]
        public Field ServicesTotalFee { get; set; }

        [PositionalValueAttribute("Services_Remarks")]
        public override string Remarks { get; set; }
        #endregion
        #endregion

        #region Consructors
        
        public ADAClaim() 
            : base()
        {
            this.ClaimType = InstanceType;
            this.PatientRelationshipCode = TextractClaimRelationship.Self;
        }
        #endregion

        #region Methods

        public override void AssignClaimNumber(bool requery = false)
        {
            base.AssignClaimNumber(requery);

            CleanupClaim();
        }

        protected override void CleanupClaim()
        {
            // remove any empty service lines before exporing
            this.ServiceLines.Where(l => l.HasData == false | l.IsHeader)
                .ToList().ForEach(l => this.ServiceLines.Remove(l));
        }

        bool FieldConfidence(params Field[] fields)
        {
            bool result = false;

            if (fields.All(f => f == null))
                return result;

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i]?.Value != null)
                    result = result || (fields[i].UpdateToClaim | fields[i].Value.Confidence > fields[i].MinConfidence);
            }

            return result;
        }

        Table ConsoidateTable(Page page)
        {
            Table table = null;

            if (page.Tables.Count > 2)
            {
                Logger.TraceWarning($"Multiple tables present on claim page.  Total first page tables: {page.Tables.Count}.  Research potential missed data");

            }
            if (page.Tables.Count == 2)
            {
                try
                {
                    int tableOneRowCnt = page.Tables[0].Rows.Count;
                    int tableOneColumns = page.Tables[0].Rows.First().Cells.Count;

                    int tableTwoRowCnt = page.Tables[1].Rows.Count;
                    int tableTwoColumns = page.Tables[1].Rows.First().Cells.Count;
                    int r1Delta = 0;
                    int r2Delta = 0;

                    for (int r1 = 0; r1 < tableOneRowCnt; r1++)
                    {
                        var tableOneRow = page.Tables[0].Rows[r1 + r1Delta];
                        bool tableOneRowIsHeader = tableOneRow.RowType.HasFlag(TextractClaimTableRowType.HeaderRow);

                        var tableTwoRow = page.Tables[1].Rows[r1 + r2Delta];
                        bool tableTwoRowIsHeader = tableTwoRow.RowType.HasFlag(TextractClaimTableRowType.HeaderRow);

                        if (tableOneRowIsHeader & tableTwoRowIsHeader)
                            continue;

                        if (!tableOneRowIsHeader & tableTwoRowIsHeader)
                        {
                            r1Delta -= 1;
                            continue;
                        }

                        if (tableOneRowIsHeader & !tableTwoRowIsHeader)
                        {
                            r2Delta -= 1;
                            continue;
                        }

                        for (int c = 0; c < tableTwoColumns; c++)
                        {
                            // copy table two cells to table 1 to consolidate multi table claims
                            page.Tables[0].Rows[r1 + r1Delta].Cells
                                .Add(page.Tables[1].Rows[r1 + r2Delta].Cells[c]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceError(ex.ToString());
                    //throw;
                }
            }

            if (page.Tables.Count > 0)
                table = page.Tables[0];

            return table;
        }

        public override void Map(Page page)
        {
            try
            {
                base.Map(page);

                int r = 0;
                string priorStartDate = null;

                var compositeTable = ConsoidateTable(page); //page.Tables[0];

                compositeTable?.Rows
                    .ForEach(row =>
                    {
                        ADAServiceLine l = new ADAServiceLine();
                        bool hasData = l.Map(page, r++);
                        l.HasData = hasData & !l.IsHeader;

                        if (hasData && priorStartDate.IsNotNullOrWhiteSpace())
                            l.ServiceStartDate = l.ServiceStartDate.IsNullOrWhiteSpace() ? priorStartDate : l.ServiceStartDate;

                        this.ServiceLines.Add(l);

                        if (l.ServiceStartDate.IsNotNullOrWhiteSpace())
                            priorStartDate = l.ServiceStartDate;
                    });

                // check for missing decimal point
                if (this.TotalChargeAmount.IsNotNullOrWhiteSpace()
                    && (this.ServiceLinesTotalChargeAmount == decimal.Parse(this.TotalChargeAmount) / 10
                      | this.ServiceLinesTotalChargeAmount == decimal.Parse(this.TotalChargeAmount) / 100)
                    )
                    this.TotalChargeAmount = this.ServiceLinesTotalChargeAmount.ToString();

                //this.HasValidMap = true;
            }
            catch (System.Reflection.TargetInvocationException t)
            {
                Logger.TraceError(t);
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

        [CompositeValueParserAttribute]
        public override void ParseCompositeValues()
        {
            try
            {

                // InsuranceCompanNameAndAddr
                var nac = ParseNameAddr(this.InsuranceCompanyNameAndAddr?.Value, true);
                this.ReceiverName = nac.NameComponent.LastName;
                string receiverStreet = nac.AddressComponent.AddressLine1;


                // PatientNameAndAddr
                nac = ParseNameAddr(this.PatientNameAndAddr?.Value);
                this.PatientFirstName = nac.NameComponent.FirstName;
                this.PatientLastName = nac.NameComponent.LastName;
                this.PatientMName = nac.NameComponent.MiddleName;
                this.PatientAddress1 = nac.AddressComponent.AddressLine1;
                this.PatientAddress2 = nac.AddressComponent.AddressLine2;
                this.PatientCity = nac.AddressComponent.City;
                this.PatientState = nac.AddressComponent.State;
                this.PatientZip = nac.AddressComponent.Zip;

                // InsuredNameAndAddr
                nac = ParseNameAddr(this.InsuredNameAndAddr?.Value);

                this.InsuredFirstName = nac.NameComponent.FirstName;
                this.InsuredLastName = nac.NameComponent.LastName;
                this.InsuredMName = nac.NameComponent.MiddleName;
                this.InsuredAddress1 = nac.AddressComponent.AddressLine1;
                this.InsuredAddress2 = nac.AddressComponent.AddressLine2;
                this.InsuredCity = nac.AddressComponent.City;
                this.InsuredState = nac.AddressComponent.State;
                this.InsuredZip = nac.AddressComponent.Zip;

                // BillingDentistNameAndAddr
                nac = ParseNameAddr(this.BillingDentistNameAndAddr?.Value, true);
                this.PayeeName = nac.NameComponent.LastName;
                this.PayeeAddress1 = nac.AddressComponent.AddressLine1;
                this.PayeeCity = nac.AddressComponent.City;
                this.PayeeState = nac.AddressComponent.State;
                this.PayeeZip = nac.AddressComponent.Zip;

                // TreatingDentistFullName
                if (this.TreatingDentistFullName?.Value != null)
                {
                    var nc = ParseName(this.TreatingDentistFullName?.Value.Content, true);
                    //this.ProviderFirstName = nc.FirstName;
                    this.ProviderName = nc.LastName;
                }

                // TreatingDentistAddr
                if (this.TreatingDentistAddr?.Value != null) 
                { 
                    var ac = ParseAddress(this.TreatingDentistAddr?.Value.Content);
                    this.ProviderAddress1 = ac.AddressLine1;
                    this.ProviderCity = ac.City;
                    this.ProviderState = ac.State;
                    this.ProviderZip = ac.Zip;
                }

                if (this.ProviderName.IsNotNullOrWhiteSpace() 
                    && NamedCoordinates.FuzzyScore(this.ProviderName, SignatureOnFileLiteral) > 70 
                    && BillingDentistNameAndAddr.Key.HasMatch)
                    SetProviderDetailsToPayee();

                if(this.ProviderAddress1.IsNullOrWhiteSpace() & !this.PayeeAddress1.IsNullOrWhiteSpace())
                    SetProviderDetailsToPayee(false);

                // TotalChargeAmout
                this.TotalChargeAmount = this.ServicesTotalFee?.Text.CleanupForCurrency();


                this.InsuredSex = BuildSexValue(this.InsuredSexIndicator_F?.Text, this.InsuredSexIndicator_M?.Text);

                this.PatientSex = BuildSexValue(this.PatientSexIndicator_F?.Text, this.PatientSexIndicator_M?.Text);

                this.PatientRelationshipCode = BuildRelationshipValue(this.PatientRelationshipCode_Self?.Text,
                                                                        this.PatientRelationshipCode_Spouse?.Text,
                                                                        this.PatientRelationshipCode_Dependent?.Text,
                                                                        this.PatientRelationshipCode_Other?.Text);

                this.AncillaryInfoOrthadontics = BuildYesNoValue(this.AncillaryInfoOrthadontics_Yes?.Text, this.AncillaryInfoOrthadontics_No?.Text);
                this.AncillaryInfoReplacement = BuildYesNoValue(this.AncillaryInfoReplacement_Yes?.Text, this.AncillaryInfoReplacement_No?.Text);

                if (  this.PatientRelationshipCode == TextractClaimRelationship.Self
                    & this.PatientLastName.IsNullOrWhiteSpace() )
                    SetPatientToInsured();

                //if (this.ProviderName.IsNullOrWhiteSpace() & this.PayeeName.IsNotNullOrWhiteSpace())
                //    this.ProviderName = this.PayeeName;

                if (this.PatientID.IsNullOrWhiteSpace() & this.InsuredID.IsNotNullOrWhiteSpace())
                    this.PatientID = this.InsuredID;

                // ACST Address
                if (NamedCoordinates.FuzzyScore(receiverStreet?.ToUpper(), UnionStreetAddressLiteral.ToUpper()) > 70)
                {
                    ReceiverName = UnionNameLiteral;
                }

                // removed due to ValidatinResults Ienum
                //// This should be called at the end of overriden parse
                //base.ParseCompositeValues();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetProviderDetailsToPayee(bool includingName = true)
        {
            if(includingName)
                this.ProviderName = this.PayeeName;

            this.ProviderAddress1 = this.PayeeAddress1;
            this.ProviderCity = this.PayeeCity;
            this.ProviderState = this.PayeeState;
            this.ProviderZip = this.PayeeZip;
        }

        public bool SetPatientAsPayee()
        {
            return false;
        }

        public bool SetPatientToInsured()
        {
            this.PatientIsSetToInsured = false;

            if (  this.InsuredLastName.IsNotNullOrWhiteSpace()
                & this.InsuredID.IsNotNullOrWhiteSpace())
            {

                this.PatientRelationshipCode = TextractClaimRelationship.Self;
                this.PatientAddress1 = this.InsuredAddress1;
                this.PatientFirstName = this.InsuredFirstName;
                this.PatientLastName = this.InsuredLastName;
                this.PatientMName = this.InsuredMName;
                this.PatientAddress1 = this.InsuredAddress1;
                this.PatientAddress2 = this.InsuredAddress2;
                this.PatientCity = this.InsuredCity;
                this.PatientState = this.InsuredState;
                this.PatientZip = this.InsuredZip;

                this.PatientDOB = this.InsuredDOB;
                this.PatientSex = this.InsuredSex;

                if (this.InsuredID.IsNotNullOrWhiteSpace())
                    this.PatientID = this.InsuredID;

                this.PatientIsSetToInsured = true;
            }

            return this.PatientIsSetToInsured;
        }

        protected override decimal SumServiceLines()
        {
            return this.ServiceLines.Sum(a => a.ChargedAmount.ToDecimal());
        }

        public override IEnumerable<ValidationResult> ValidationResults()
        {
            DateTime dt;
            decimal amount;

            if (this.ServiceLines.Any(s => s.HasData && !s.IsValid))
            {
                yield return new ValidationResult("ServiceLine_ProcedureDate", "One or more Service Line errors", "Line");
            }

            if (this.ServiceLines.Where(s => s.HasData & s.IsValid).Count() == 0)
            {
                yield return new ValidationResult("ServiceLine_ProcedureDate", "At least one valid service line is required.", "Line");
            }

            if (this.ServiceLinesTotalChargeAmount != this.TotalChargeAmount.ToDecimal())
            {
                yield return new ValidationResult("ServiceLine_Fee", $"ServiceLine total: {this.ServiceLinesTotalChargeAmount} must equal Total Fee: {this.TotalChargeAmount}", "Line");
            }

            if (this.ManualApproval)
                yield break;

            if (this.PatientRelationshipCode == TextractClaimRelationship.Unknown)
                yield return new ValidationResult("PatientRelationship", "18. Patient relationship to Subscriber", "18");
            else if (!this.PatientIsSetToInsured && !FieldConfidence(PatientRelationshipCode_Dependent, PatientRelationshipCode_Other, PatientRelationshipCode_Self, PatientRelationshipCode_Spouse))
                yield return new ValidationResult("PatientRelationship", "Low Confidence - 18. Patient relationship to Subscriber", "18");

            // ToDo: Make these rules DataContract attribtes of the class properties collected during reflection

            if (!FieldConfidence(this.InsuredNameAndAddr))
                yield return new ValidationResult("Subscriber_NameAndAddr", "Low Confidence - 12.Subscriber Last Name is required.", "12");
            else if (this.InsuredLastName.IsNullOrWhiteSpace())
                yield return new ValidationResult("Subscriber_NameAndAddr", "12. Subscriber Last Name is required.", "12");

            if (!DateTime.TryParse(this.InsuredDOB, out dt) | dt > DateTime.Now)
                yield return new ValidationResult("Subscriber_DOB", "13. Subscriber DoB is required.  (Prior to today)", "13");

            if (this.InsuredID.IsNullOrWhiteSpace())
                yield return new ValidationResult("Subscriber_ID", "15. Subscriber SSN/ID is required.", "15");

            if (this.PatientLastName.IsNullOrWhiteSpace())
                yield return new ValidationResult("Patient_NameAndAddr", "20. Patient Last Name is required.", "20");

            if (!DateTime.TryParse(this.PatientDOB, out dt) | dt > DateTime.Now)
                yield return new ValidationResult("Patient_DOB", "21. Patient DoB is required.  (Prior to today)", "21");

            if (this.PatientID.IsNullOrWhiteSpace())
                yield return new ValidationResult("Patient_ID", "23. Patient SSN/ID is required.", "23");

            if (!FieldConfidence(this.ServicesTotalFee))
                yield return new ValidationResult("Services_TotalFee", "Low Confidence - 32/33 Total Fee is required.  (Positive non-zero decimal)", "32");
            else if (!decimal.TryParse(this.TotalChargeAmount, out amount) | amount <= 0)
                yield return new ValidationResult("Services_TotalFee", "32/33 Total Fee is required.  (Positive non-zero decimal)", "32");

            if (this.PayeeEIN.IsNullOrWhiteSpace())
                yield return new ValidationResult("BillingDentist_EIN", "51. Provider EIN is required.  (9-digit number)", "51");

            if (this.PayeeNPI.IsNullOrWhiteSpace())
                yield return new ValidationResult("BillingDentist_NPI", "49. NPI - Billing Dentist (10 digits)", "49");

            if (!FieldConfidence(this.TreatingDentistFullName))
                yield return new ValidationResult("TreatingDentist_Name_Signature", "Low Confidence - 53. Treating Dentist Last Name.", "53");
            else if (this.ProviderName.IsNullOrWhiteSpace())
                yield return new ValidationResult("TreatingDentist_Name_Signature", "53. Treating Dentist Last Name.", "53");

            if (!FieldConfidence(this.TreatingDentistAddr))
                yield return new ValidationResult("TreatingDentist_Addr", "Low Confidence - 56.Provider Address is required.", "56");
            else if (this.ProviderAddress1.IsNullOrWhiteSpace())
                yield return new ValidationResult("TreatingDentist_Addr", "56. Provider Address is required.", "56");

        //if (this.ProviderNPI.IsNullOrWhiteSpace())
        //    yield return new ValidationResult("TreatingDentist_NPI", "54. NPI - Treating Dentist (10 digits)", "54");
    }

    #endregion
}
}
