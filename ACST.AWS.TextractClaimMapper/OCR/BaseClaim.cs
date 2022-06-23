
namespace ACST.AWS.TextractClaimMapper.OCR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Common;
    using CAMS = ACST.AWS.Data.CAMS;
    using EDI = ACST.AWS.Data.EDI;

    using USAddress;
    using NameParser;
    using System.Runtime.CompilerServices;

    public class BaseClaim 
        : IClaim
    {
        private static readonly USAddress.AddressParser Parser = USAddress.AddressParser.Default;

        public const string SelectedElementIndicator = "SELECTED";
        public const string NotSelectedElementIndicator = "NOT_SELECTED";
        public const string SignatureOnFileLiteral = "Signature on File";
        public const string InsuredIsThePatientLiteral = "Insured is the Patient";
        public const string UnionNameLiteral = "UFCW LOCAL 1529";
        public const string UnionStreetAddressLiteral = "661 N Ericson Rd";

        public bool PatientIsSetToInsured { get; set; }

        public bool PatientIsPayee { get; set; }

        protected virtual int ServiceLinesCount { get; }

        protected virtual decimal ServiceLinesTotalChargeAmount { get { return SumServiceLines(); } }

        public bool ManualApproval { get; set; }

        public bool IsValid
        {
            get { return (ValidationResults().Count() == 0); }
        }

        protected virtual decimal SumServiceLines()
        {
            return 0;
        }

        #region Structures
        protected struct AddressComponents
        {
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
        }

        protected struct NameComponents
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Suffix { get; set; }
        }

        protected struct NameAddrComponents
        {
            public NameComponents NameComponent { get; set; }
            public AddressComponents AddressComponent { get; set; }
        }
        #endregion

        #region Claim Attribute Properties

        [XmlAttribute]
        public virtual string ClaimNo { get; set; }

        [XmlAttribute]
        public virtual TextractClaimType ClaimType { get; set; }

        [XmlAttribute]
        public virtual string Tracking { get; set; }

        [XmlAttribute]
        public virtual string Specification { get; set; }

        [XmlAttribute]
        public virtual string Status { get; set; }

        [XmlAttribute]
        public virtual bool Priced { get; set; }

        [XmlAttribute]
        public virtual string RelatedClaimNo { get; set; }
        #endregion

        #region Claim Properties

        public virtual string SubmitterTypeQualifier { get; set; } = "2";

        public virtual string SubmitterLastName { get; set; } = "ACST";

        public virtual string SubmitterID { get; set; } = "0000021850";

        public virtual string SubmitterContactName { get; set; } = "NEAL JARRETT";

        public virtual string SubmitterNOQualifier1 { get; set; } = "TE";

        public virtual string SubmitterNO1 { get; set; } = "9018468499";

        public virtual string SubmitterNOQualifier2 { get; set; }

        public virtual string SubmitterNO2 { get; set; }

        public virtual string ReceiverName { get; set; } = "UFCW1529";

        //public virtual string ReceiverAddress1 { get; set; }

        //public virtual string ReceiverCity { get; set; }

        //public virtual string ReceiverState { get; set; }

        //public virtual string ReceiverZip { get; set; }

        public virtual string ReceiverID { get; set; } = "000000001";

        public virtual string PayerName { get; set; }

        public virtual string PayerID { get; set; }

        public virtual string ClaimFilingIndicatorCode { get; set; }

        public virtual string ProviderTypeQualifier { get; set; }

        
        public virtual string ProviderName { get; set; }

        
        public virtual string ProviderFirstName { get; set; }

        
        public virtual string ProviderMName { get; set; }

        
        public virtual string ProviderAddress1 { get; set; }

        
        public virtual string ProviderCity { get; set; }

        
        public virtual string ProviderState { get; set; }

        
        public virtual string ProviderZip { get; set; }

        
        public virtual string ProviderPhoneNo { get; set; }

        
        public virtual string ProviderSpecialityCode { get; set; }

        public virtual string ProviderEIN { get; set; }

        public virtual string ProviderLicenseNo { get; set; }

        
        public virtual string ProviderNPI { get; set; }

        
        public virtual string ProviderAdditionalID { get; set; }

        public bool ProviderSignIndicator { get; set; }

        
        public virtual string ProviderSignatureDate { get; set; }

        public virtual string PayeeTypeQualifier { get; set; }

        
        public virtual string PayeeName { get; set; }

        
        public virtual string PayeeFirstName { get; set; }

        
        public virtual string PayeeMName { get; set; }

        
        public virtual string PayeeAddress1 { get; set; }

        
        public virtual string PayeeCity { get; set; }

        
        public virtual string PayeeState { get; set; }

        
        public virtual string PayeeZip { get; set; }

        
        public virtual string PayeePhoneNo { get; set; }

        public virtual string PayeeEIN { get; set; }

        public virtual string PayeeLicenseNo { get; set; }

        public virtual string PayeeNPI { get; set; }

        public virtual string PayeeAdditionalID { get; set; }

        public virtual string PayerLevelOfResponsibility { get; set; }


        public virtual string InsuredLastName { get; set; }

        public virtual string InsuredFirstName { get; set; }

        public virtual string InsuredMName { get; set; }

        public virtual string InsuredAddress1 { get; set; }

        public virtual string InsuredAddress2 { get; set; }

        public virtual string InsuredCity { get; set; }

        public virtual string InsuredState { get; set; }

        public virtual string InsuredZip { get; set; }



        public virtual string InsuredGroupNO { get; set; }

        public virtual string InsuredDOB { get; set; }

        public virtual TextractClaimGender InsuredSex { get; set; }

        public virtual string InsuredID { get; set; }

        public virtual string InsuredSignatureDate { get; set; }

        public virtual string InsuredSignature { get; set; }

        public virtual string EmployerName { get; set; }

        public virtual TextractClaimRelationship PatientRelationshipCode { get; set; }


        public virtual string PatientLastName { get; set; }


        public virtual string PatientFirstName { get; set; }


        public virtual string PatientMName { get; set; }


        public virtual string PatientNameSuffix { get; set; }

        public virtual string PatientAddress1 { get; set; }


        public virtual string PatientAddress2 { get; set; }

        public virtual string PatientCity { get; set; }

        public virtual string PatientState { get; set; }

        public virtual string PatientZip { get; set; }

        public virtual string PatientDOB { get; set; }

        public virtual TextractClaimGender PatientSex { get; set; }

        public virtual string PatientID { get; set; }

        public virtual string PatientSSN { get; set; }

        public virtual string PatientAccountNO { get; set; }

        public virtual string PatientSignatureDate { get; set; }

        public virtual string PatientSignature { get; set; }

        public virtual TextractClaimYesNo AncillaryInfoOrthadontics { get; set; }
        public virtual TextractClaimYesNo AncillaryInfoReplacement { get; set; }
        public virtual string AmountPaid { get; set; }

        // ToDo move many of these Dental specific properties to subtype
        public virtual string TotalChargeAmount { get; set; }
        //public virtual decimal TotalChargeAmount { get; set; }

        public virtual string Remarks { get; set; }

        public virtual string ClaimFrequencyCode { get; set; }

        public virtual string MedicareAssignmentCode { get; set; }

        public virtual string PricingMethodology { get; set; }
        
        public virtual string BenefitsAssignCertIndicator { get; set; }
        //public bool BenefitsAssignCertIndicator { get; set; }

        public virtual string ReleaseOfInfoCode { get; set; }

        public virtual string DelayReasonCode { get; set; }

        public bool SpecialProgramIndicator { get; set; }


        //public virtual string ClaimOriginalReferenceNO { get; set; }
        //public virtual string RepricedClaimRefNO { get; set; }
        public virtual string ClearinghouseRefNo { get; set; }

        public virtual string PrincipalDiagnosisCode { get; set; }

        public virtual string DiagnosisCode1 { get; set; }

        public virtual string DiagnosisCode2 { get; set; }

        public virtual string DiagnosisCode3 { get; set; }

        public virtual string DiagnosisCode4 { get; set; }

        public virtual string DiagnosisCode5 { get; set; }

        //public virtual string RenderingLastName { get; set; }

        //public virtual string RenderingFirstName { get; set; }

        //public virtual string RenderingIdentifier { get; set; }
        #endregion

        #region Constructor
        
        public BaseClaim() 
        {
            //this.IsBuilt = false;
            this.ManualApproval = false;
            this.PatientIsSetToInsured = false;
        }
        #endregion

        #region protected Methods

        protected virtual Nullable<DateTime> BuildDateValue(string mm, string dd, string yy)
        {
            Nullable<DateTime> ret = default(Nullable<DateTime>);

            if (DateTime.TryParse($"{mm}/{dd}/{yy}", out DateTime dt)) ret = dt;

            return ret;
        }

        protected virtual TextractClaimRelationship BuildRelationshipValue(string selfIndicator, string spouseIndicator, string dependentIndicator, string otherIndicator)
        {
            bool self = selfIndicator == SelectedElementIndicator;
            bool spouse = spouseIndicator == SelectedElementIndicator;
            bool dependent = dependentIndicator == SelectedElementIndicator;
            bool other = otherIndicator == SelectedElementIndicator;

            if ((!self & !spouse & !dependent & !other) || (self & spouse & dependent & other))
                return TextractClaimRelationship.Self;

            return self ? TextractClaimRelationship.Self
                    : spouse ? TextractClaimRelationship.Spouse
                    : dependent ? TextractClaimRelationship.Dependent
                    : other ? TextractClaimRelationship.Other
                    : TextractClaimRelationship.Unknown;
        }

        protected virtual TextractClaimGender BuildSexValue(string fIndicator, string mIndicator)
        {
            //bool f = fIndicator.IsNotNullOrWhiteSpace();
            //bool m = mIndicator.IsNotNullOrWhiteSpace();
            bool f = fIndicator == SelectedElementIndicator;
            bool m = mIndicator == SelectedElementIndicator;

            if ((f & m) | (!f & !m)) return TextractClaimGender.Unknown;

            return f ? TextractClaimGender.Female : TextractClaimGender.Male;
        }

        protected virtual TextractClaimYesNo BuildYesNoValue(string yesIndicator, string noIndicator)
        {
            bool yes = yesIndicator == SelectedElementIndicator;
            bool no = noIndicator == SelectedElementIndicator;

            if ((yes & no) | (!yes & !no)) return TextractClaimYesNo.No;

            return yes ? TextractClaimYesNo.Yes : TextractClaimYesNo.No;
        }

        protected virtual string ParseYesNoBox(string value)
        {
            string ret;

            switch (value?.ToUpper())
            {
                case "Y":
                case SelectedElementIndicator:
                    ret = "Y";
                    break;
                case "N":
                case NotSelectedElementIndicator:
                    ret = "N";
                    break;
                default:
                    ret = "N";
                    break;
            }

            return ret;
        }

        protected virtual NameComponents ParseName(IEnumerable<dynamic> content, bool businessName = false)
        {
            if (content == null || content.OfType<SelectionElement>().Any())
                return new NameComponents();

            NameComponents nc = new NameComponents();

            var name = content.Select(c => c.Text).ToArray();
            var aggrate = name.Aggregate((i, j) => i + " " + j);

            if (businessName)
            {
                nc.LastName = aggrate;
                return nc;
            }

            // Use NameParserSharp Nuget for human names
            var n = new HumanName(aggrate);
            nc.FirstName = n.First;
            nc.LastName = n.Last;
            nc.MiddleName = n.Middle;
            nc.Suffix = n.Suffix;

            //int parts = name.Length;

            //if (businessName)
            //{
            //    nc.LastName = name.Aggregate((i, j) => i + " " + j);
            //    return nc;
            //}

            //if (name.First().Contains(","))
            //{
            //    nc.LastName = name.First().Replace(",", string.Empty);

            //    if (name.Length > 2)
            //    {
            //        nc.MiddleName = name.Last().Replace(".", string.Empty);
            //        nc.FirstName = name[1].Replace(",", string.Empty);
            //    }
            //    else
            //    {
            //        if (name.Length > 1) nc.FirstName = name.Last();
            //    }
            //}
            //else
            //{
            //    nc.LastName = name.Last();

            //    if (name.Length > 2) nc.MiddleName = name[1];

            //    if (name.Length > 1) nc.FirstName = name.First();
            //}

            return nc;
        }

        protected virtual AddressComponents ParseAddress(IEnumerable<dynamic> content)
        {
            bool falseNo = false;

            if (content == null || content?.Count() == 0)
                return new AddressComponents();

            AddressComponents ac = new AddressComponents();

            var addr = content.Aggregate((i, j) => i + " " + j);

            AddressParseResult parsedAddr = Parser.ParseAddress(addr, false);
            
            // todo: hack until we expand regex, this allows street addresses without leading number
            if (parsedAddr == null)
            {
                falseNo = true;
                parsedAddr = Parser.ParseAddress($"1 {addr}", false);
            }

            if (parsedAddr == null)
                return new AddressComponents();

            ac.AddressLine1 = falseNo ? parsedAddr.StreetLine.TrimStart('1').TrimStart(' ') : parsedAddr.StreetLine;
            ac.City = parsedAddr.City;
            ac.State = parsedAddr.State;
            ac.Zip = parsedAddr.Zip;

            return ac;
        }

        protected virtual NameAddrComponents ParseNameAddr(FieldValue fieldValue, bool businessName = false)
        {
            if (fieldValue == null)
                return new NameAddrComponents();

            NameAddrComponents nac = new NameAddrComponents();

            var Name = fieldValue.Content.Where(c => c.Geometry.ReadingOrder?.Row == 0);
            var Addr = fieldValue.Content.Where(c => c.Geometry.ReadingOrder?.Row > 0);

            nac.NameComponent = ParseName(Name, businessName);
            nac.AddressComponent = ParseAddress(Addr);

            return nac;
        }
        #endregion

        #region Public Methods
        public virtual void AssignClaimNumber(bool requery)
        {
            if (!requery & this.ClaimNo.IsNotNullOrEmpty())
                return;

            try
            {
                // Get next CAMS ClaimNo
                CAMS.Context cams = new CAMS.Context();

                var nextClaimNo = cams.GetNextClaimNumber();

                if (nextClaimNo == 0)
                    throw new ArgumentOutOfRangeException("GetNextClaimNo returned an invalid value: '{nextClaimNo}'.");

                this.ClaimNo = nextClaimNo.ToString();
            }
            catch (Exception ex)
            {
                Logger.TraceError(ex);
            }
        }

        protected virtual void CleanupClaim()
        {
            throw new NotImplementedException();
        }

        public virtual void Map(Page page)
        {
            Map(page?.Form.MatchedFields);
        }

        public virtual void Map(List<Field> matchedFields)
        {
            //Mapper.SuppressUnMatchedFieldExceptions = false;
            Mapper.MatchedElementMapping<BaseClaim, Field>(this, matchedFields);
        }

        public virtual void ParseCompositeValues()
        {
            throw new NotImplementedException();
        }

        public void StoreClaimDetails(string imageName)
        {
            //ToDo: Move this to data layer

            if (imageName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(imageName));

            if (InsuredID.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(InsuredID));
            try
            {
                EDI.Context edi = new EDI.Context();

                var result = edi.SetImageTransmission(DateTime.Now, null, null, null, null, null, this.InsuredID, null, imageName);

                if (!result)
                    throw new ArgumentOutOfRangeException("SetImageTransmission did not insert a record.");
            }
            catch (Exception ex)
            {
                Logger.TraceError(ex);
            }
        }

        public virtual IEnumerable<ValidationResult> ValidationResults()
        {
            yield break;
        }
        #endregion
    }
}