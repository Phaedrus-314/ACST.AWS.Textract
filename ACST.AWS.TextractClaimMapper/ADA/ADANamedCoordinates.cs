
namespace ACST.AWS.TextractClaimMapper.ADA
{
    using System;

    using ACST.AWS.Common;
    using ACST.AWS.Textract.Model;

    public class ADANamedCoordinates
        : NamedCoordinates
    {
        //public override string ConfigurationFileName => Configuration.Instance.ADANamedCoordinates_FileName;

        public ADANamedCoordinates()
        {
            base.Source = NamedCoordinatesSource.ExternalXML;
            this.ConfigurationFileName = Configuration.Instance.ADANamedCoordinates_FileName;
            DeserializeFromXML();
        }

        public ADANamedCoordinates(string configurationFileName) 
            : base(configurationFileName)
        { }
    }


    //[Obsolete]
    //public sealed class XADANamedCoordinatesSingleton<T>
    //    : List<T> where T : INamedCoordinate, new()
    //{
    //    public NamedCoordinatesSource Source { get; private set; }
    //    public string SourceFileName { get; private set; }

    //    private XADANamedCoordinatesSingleton()
    //    {
    //        T T = new T();

    //        this.Source = T.Source;
    //        this.SourceFileName = T.NamedCoordinatesOverrideFileName;

    //        //ADANamedCoordinatesDefaults();
    //        DeserializeFromXML();
    //    }

    //    //private void ADANamedCoordinatesDefaults()
    //    //{
    //    //    //=CONCATENATE("new ADANamedCoordinate ('name','",D10,"',","1",",","2",", new PointF { X=",C10,"F,","Y=",B10,"F } ),")

    //    //    //(string groupName, string name, string exactTextMatch, PointF idealCenterValue, PointF idealCenterKey)
    //    //    namedCoordinates.Add(new T() { GroupName = "HeaderInfo", Name = "Header_Preauthorization", ExactTextMatch = "Request for Predetermination/Preauthorization", IdealCenterKey= new PointF { X = 0.3642209F, Y = 0.0871736854F }, IdealCenterValue = new PointF { X = 0.248578712F, Y = 0.08771375F } });
    //    //    namedCoordinates.Add(new T() { Name = "test2" });
    //    //    //namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_Preauthorization", "Request for Predetermination/Preauthorization", new PointF { X = 0.248578712F, Y = 0.08771375F }, new PointF { X = 0.3642209F, Y = 0.0871736854F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_ActualServices", "Statement of Actual Services", new PointF { X = 0.06297525F, Y = 0.08657699F }, new PointF { X = 0.138912886F, Y = 0.08758103F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_EPSDT", "EPSDT XIX", new PointF { X = 0.06251692F, Y = 0.101918526F }, new PointF { X = 0.113488108F, Y = 0.101589344F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("HeaderInfo","Header_PreauthorizationNo","2. Predetermination/Preauthorization Number", new PointF { X=F,Y=F }, new PointF { X=0.145023376F,Y=0.1172243F } ));

    //    //    //namedCoordinates.Add(new NamedCoordinate("InsuranceCompany", "InsuranceCompany_NameAndAddr", "3. Company/Plan Name. Address City State, Zip Code", new PointF { X = 0.13358438F, Y = 0.185889557F }, new PointF { X = 0.167338222F, Y = 0.160027787F }));

    //    //    //namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_NameAndAddr", "12 Policyholder/Subscriber Name (Last, First, Middle Initial, Suffix)); Address City State, Zip Code", new PointF { X = 0.601931F, Y = 0.158007056F }, new PointF { X = 0.7316091F, Y = 0.127945259F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_ID", "15. Policyholder/Subscriber ID (SSN or ID#)", new PointF { X = 0.8143239F, Y = 0.216305524F }, new PointF { X = 0.863415241F, Y = 0.201100081F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB", "13 Date of Birth (MM/DD/CCYY)", new PointF { X = 0.56922394F, Y = 0.2161932F }, new PointF { X = 0.5874541F, Y = 0.201570138F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Sex_M", "M", new PointF { X = 0.6976763F, Y = 0.215500608F }, new PointF { X = 0.7132686F, Y = 0.213894725F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("SubscriberInfo","Subscriber_Employer","17 Employer Name", new PointF { X=F,Y=F }, new PointF { X=0.7124295F,Y=0.231463F } ));
    //    //    //namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PlanNo", "16. Plan/Group Number", new PointF { X = 0.559370458F, Y = 0.244800255F }, new PointF { X = 0.5663628F, Y = 0.2315019F }));

    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Medical","Medical?", new PointF { X=0.202067211F,Y=0.245446354F }, new PointF { X=0.165942237F,Y=0.245377451F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Dental","4. Dental?", new PointF { X=0.110398151F,Y=0.245542943F }, new PointF { X=0.06719726F,Y=0.245583877F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Ignore1","(If both. complete 5-11 for dental only.)", new PointF { X=F,Y=F }, new PointF { X=0.331373632F,Y=0.2460042F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Name","5. Name of Policyholder/Subscriber in #4 (Last, First, Middle Initial, Suffix)", new PointF { X=F,Y=F }, new PointF { X=0.209576041F,Y=0.26062274F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_DOB","6. Date of Birth (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.114805743F,Y=0.289402574F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_ID","8 Policyholder/Subscriber ID (SSN or ID#)", new PointF { X=F,Y=F }, new PointF { X=0.391588032F,Y=0.290944785F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Sex_M","M", new PointF { X=0.223726645F,Y=0.3025531F }, new PointF { X=0.239640325F,Y=0.302259773F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Sex_F","F", new PointF { X=0.259282321F,Y=0.302528441F }, new PointF { X=0.2752207F,Y=0.3024253F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_PlanNo","9 Plan/Group Number", new PointF { X=F,Y=F }, new PointF { X=0.09432092F,Y=0.317573369F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Self","Self", new PointF { X=0.224116921F,Y=0.330135047F }, new PointF { X=0.244684562F,Y=0.330209255F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Spouse","Spouse", new PointF { X=0.28237316F,Y=0.3302574F }, new PointF { X=0.3109225F,Y=0.330694318F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Dependent","Dependent", new PointF { X=0.35097754F,Y=0.3301795F }, new PointF { X=0.386818618F,Y=0.3305339F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Other","Other", new PointF { X=0.431791246F,Y=0.330295026F }, new PointF { X=0.455605328F,Y=0.330015957F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_","11, Other Insurance Company/Dental Benefit Plan Name Address City, State, Zip Code", new PointF { X=F,Y=F }, new PointF { X=0.239019454F,Y=0.346023679F } ));

    //    //    //namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_NameAndAddr", "20 Name (Last, First Middle Initial, Suffix) Address City. State, Zip Code", new PointF { X = 0.600933F, Y = 0.330510736F }, new PointF { X = 0.6777182F, Y = 0.3034424F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Self","Self", new PointF { X=0.7673692F,Y=0.28753987F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Spouse","Spouse", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Dependent","Dependent Child", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Other","Other", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //    //    //namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Reserved", "Reserved For Future", new PointF { X = 0.870980263F, Y = 0.282466084F }, new PointF { X = 0.9176366F, Y = 0.277240753F }));


    //    //    //namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_ID", "23 Patient ID/Account # (Assigned by Dentist)", new PointF { X = 0.865340769F, Y = 0.388285F }, new PointF { X = 0.8659477F, Y = 0.3738706F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB", "21. Date of Birth (MM/DD/CCYY)", new PointF { X = 0.567835331F, Y = 0.388117135F }, new PointF { X = 0.586663365F, Y = 0.374154538F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Sex_M", "M", new PointF { X = 0.697230756F, Y = 0.386646748F }, new PointF { X = 0.712651968F, Y = 0.3867614F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Sex_F","F", new PointF { X=F,Y=F }, new PointF { X=0.7463434F,Y=0.386944264F } ));

    //    //    //namedCoordinates.Add(new NamedCoordinate("Services", "Services_MissingTeath", "33 Missing Teeth Information (Place an X on each missing tooth.)", new PointF { X = 0.240448266F, Y = 0.6092858F }, new PointF { X = 0.191075563F, Y = 0.5885275F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("Services","Services_","34. Diagnosis Code List Qualifier", new PointF { X=F,Y=F }, new PointF { X=0.505822659F,Y=0.5889741F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("Services","Services_","31a Other Fee(s)", new PointF { X=F,Y=F }, new PointF { X=0.8553593F,Y=0.5928383F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("Services","Services_","C", new PointF { X=F,Y=F }, new PointF { X=0.713207364F,Y=0.603561938F } ));
    //    //    //namedCoordinates.Add(new NamedCoordinate("Services", "Services_TotalFee", "32. Total Fee", new PointF { X = 0.9377053F, Y = 0.6184867F }, new PointF { X = 0.856220663F, Y = 0.615528762F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("Services","Services_","D", new PointF { X=F,Y=F }, new PointF { X=0.7133121F,Y=0.618725538F } ));
    //    //    //namedCoordinates.Add(new NamedCoordinate("Services", "Services_Remarks", "35 Remarks", new PointF { X = 0.143217087F, Y = 0.63222307F }, new PointF { X = 0.072160475F, Y = 0.6306957F }));

    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Enclosures", "39. Enclosures (Y or N)", new PointF { X = 0.8583816F, Y = 0.687379062F }, new PointF { X = 0.839093F, Y = 0.672511339F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Location", "(e-g 11office; 22O/P Hospital)", new PointF { X = 0.624753237F, Y = 0.6753882F }, new PointF { X = 0.707914233F, Y = 0.673129439F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_ApplianceDate","41. Date Appliance Placed (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.873371542F,Y=0.701274455F } ));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_Yes", "Yes (Complete 41-42)", new PointF { X = 0.638570547F, Y = 0.715175569F }, new PointF { X = 0.6996396F, Y = 0.7156286F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_No", "No (Skip 41-42)", new PointF { X = 0.5335592F, Y = 0.7178163F }, new PointF { X = 0.5814929F, Y = 0.7163427F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_PriorPlacementDate","44. Date of Prior Placement (MM/DD/CCYY)", new PointF { X=F,Y=F }, new PointF { X=0.8740308F,Y=0.729764163F } ));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_","42 Months of Treatment Remaining", new PointF { X=F,Y=F }, new PointF { X=0.553362846F,Y=0.7344524F } ));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_No", "No", new PointF { X = 0.6383015F, Y = 0.7445523F }, new PointF { X = 0.6555384F, Y = 0.7439394F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_Yes", "Yes (Complete 44)", new PointF { X = 0.673276365F, Y = 0.744835556F }, new PointF { X = 0.7265575F, Y = 0.744526863F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Other", "Other accident", new PointF { X = 0.8229921F, Y = 0.7732178F }, new PointF { X = 0.867154241F, Y = 0.77169F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Auto", "Auto accident", new PointF { X = 0.70783174F, Y = 0.773049951F }, new PointF { X = 0.7495544F, Y = 0.771929264F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Iliness", "Occupational iliness/injury", new PointF { X = 0.5351579F, Y = 0.7732332F }, new PointF { X = 0.6047592F, Y = 0.7722903F }));
    //    //    ////namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_","46 Date of Accident (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.581149042F,Y=0.788172F } ));

    //    //    //namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignatureDate", "Date", new PointF { X = 0.403814018F, Y = 0.718785644F }, new PointF { X = 0.379310071F, Y = 0.730112255F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignature", "Patient/Guardian Signature", new PointF { X = 0.13895604F, Y = 0.718584359F }, new PointF { X = 0.118631512F, Y = 0.730610669F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignatureDate", "Date", new PointF { X = 0.4035308F, Y = 0.775803447F }, new PointF { X = 0.379176944F, Y = 0.7868028F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignature", "Subscriber Signature", new PointF { X = 0.13857469F, Y = 0.775170565F }, new PointF { X = 0.104461655F, Y = 0.787308335F }));


    //    //    //namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NameAndAddr", "48 Name. Address, City, State Zip Code", new PointF { X = 0.168977022F, Y = 0.860715449F }, new PointF { X = 0.135210782F, Y = 0.830174148F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_EIN", "51 SSN or TIN", new PointF { X = 0.3890075F, Y = 0.9118356F }, new PointF { X = 0.378261F, Y = 0.901209235F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NPI", "49 NP|", new PointF { X = 0.09441187F, Y = 0.9121314F }, new PointF { X = 0.0607231781F, Y = 0.9012483F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_LicenseNo", "50. License Number", new PointF { X = 0.219463423F, Y = 0.9118469F }, new PointF { X = 0.240718365F, Y = 0.901891649F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_AdditionalID", "52a Additional", new PointF { X = 0.383819759F, Y = 0.9332478F }, new PointF { X = 0.322182655F, Y = 0.929167151F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_PhoneNo", "52 Phone Number", new PointF { X = 0.190277934F, Y = 0.932663739F }, new PointF { X = 0.07045753F, Y = 0.9301442F }));


    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_AdditionalID", "58 Additional Provider ID", new PointF { X = 0.83524996F, Y = 0.933086336F }, new PointF { X = 0.766909242F, Y = 0.929156661F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SignatureDate", "Date", new PointF { X = 0.884167254F, Y = 0.8474523F }, new PointF { X = 0.8512771F, Y = 0.857867062F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Name_Signature", "Signed (Treating Dentist)", new PointF { X = 0.585925043F, Y = 0.8475695F }, new PointF { X = 0.5814807F, Y = 0.8591455F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_NPI", "54 NPI", new PointF { X = 0.6137268F, Y = 0.8760797F }, new PointF { X = 0.5161233F, Y = 0.8715982F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_LicenseNo", "55 License Number", new PointF { X = 0.844708264F, Y = 0.8762593F }, new PointF { X = 0.7800867F, Y = 0.871617F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Addr", "56 Address, City State, Zip Code", new PointF { X = 0.6592705F, Y = 0.9076931F }, new PointF { X = 0.574821055F, Y = 0.8868786F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SpecialityCode", "56a Provider Specialty Code", new PointF { X = 0.883666635F, Y = 0.890574455F }, new PointF { X = 0.767791033F, Y = 0.8869426F }));
    //    //    //namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_PhoneNo", "Phone Number", new PointF { X = 0.642414451F, Y = 0.9310866F }, new PointF { X = 0.5320557F, Y = 0.929851651F }));

    //    //}

    //    private static readonly Lazy<XADANamedCoordinatesSingleton<T>> instance = new Lazy<XADANamedCoordinatesSingleton<T>>(() => new XADANamedCoordinatesSingleton<T>());

    //    public static List<T> Instance { get { return instance.Value; } }

    //    private void DeserializeFromXML()
    //    {
    //        if (this.SourceFileName == null) throw new ArgumentNullException("SourceFileName");
    //        if (!File.Exists(this.SourceFileName)) throw new FileNotFoundException(this.SourceFileName);

    //        var serializer = new XmlSerializer(typeof(List<T>));

    //        using (FileStream fileStream = new FileStream(this.SourceFileName, FileMode.Open))
    //        {
    //            this.AddRange((List<T>)serializer.Deserialize(fileStream));
    //        }
    //    }

    //}

    //public interface IDependency { }


    //[Obsolete]
    //public sealed class XADANamedCoordinates
    //    : List<NamedCoordinate>
    //{
    //    public NamedCoordinatesSource Source { get; private set; }
    //    public string SourceFileName { get; private set; }


    //    private static readonly Lazy<XADANamedCoordinates>
    //        lazy =
    //        new Lazy<XADANamedCoordinates>
    //            (() => new XADANamedCoordinates());

    //    public static XADANamedCoordinates Instance { get { return lazy.Value; } }

    //    private XADANamedCoordinates()
    //    {
    //        this.SourceFileName = Configuration.Instance.ADANamedCoordinates_FileName;

    //        if (this.SourceFileName != null)
    //        {
    //            this.Source = NamedCoordinatesSource.ExternalXML;
    //            DeserializeFromXML();
    //        }
    //        else
    //        {
    //            this.SourceFileName = null;
    //            this.Source = NamedCoordinatesSource.InternalDefault;
    //            ADANamedCoordinatesDefaults();
    //        }
    //    }

    //    private void ADANamedCoordinatesDefaults()
    //    {
    //        //=CONCATENATE("new NamedCoordinate ('name','",D10,"',","1",",","2",", new PointF { X=",C10,"F,","Y=",B10,"F } ),")

    //        namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_Preauthorization", "Request for Predetermination/Preauthorization", new PointF { X = 0.248578712F, Y = 0.08771375F }, new PointF { X = 0.3642209F, Y = 0.0871736854F }));
    //        namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_ActualServices", "Statement of Actual Services", new PointF { X = 0.06297525F, Y = 0.08657699F }, new PointF { X = 0.138912886F, Y = 0.08758103F }));
    //        namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_EPSDT", "EPSDT XIX", new PointF { X = 0.06251692F, Y = 0.101918526F }, new PointF { X = 0.113488108F, Y = 0.101589344F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("HeaderInfo","Header_PreauthorizationNo","2. Predetermination/Preauthorization Number", new PointF { X=F,Y=F }, new PointF { X=0.145023376F,Y=0.1172243F } ));

    //        namedCoordinates.Add(new NamedCoordinate("InsuranceCompany", "InsuranceCompany_NameAndAddr", "3. Company/Plan Name. Address City State, Zip Code", new PointF { X = 0.13358438F, Y = 0.185889557F }, new PointF { X = 0.167338222F, Y = 0.160027787F }));

    //        namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_NameAndAddr", "12 Policyholder/Subscriber Name (Last, First, Middle Initial, Suffix)); Address City State, Zip Code", new PointF { X = 0.601931F, Y = 0.158007056F }, new PointF { X = 0.7316091F, Y = 0.127945259F }));
    //        namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_ID", "15. Policyholder/Subscriber ID (SSN or ID#)", new PointF { X = 0.8143239F, Y = 0.216305524F }, new PointF { X = 0.863415241F, Y = 0.201100081F }));
    //        namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB", "13 Date of Birth (MM/DD/CCYY)", new PointF { X = 0.56922394F, Y = 0.2161932F }, new PointF { X = 0.5874541F, Y = 0.201570138F }));
    //        namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Sex_M", "M", new PointF { X = 0.6976763F, Y = 0.215500608F }, new PointF { X = 0.7132686F, Y = 0.213894725F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("SubscriberInfo","Subscriber_Employer","17 Employer Name", new PointF { X=F,Y=F }, new PointF { X=0.7124295F,Y=0.231463F } ));
    //        namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PlanNo", "16. Plan/Group Number", new PointF { X = 0.559370458F, Y = 0.244800255F }, new PointF { X = 0.5663628F, Y = 0.2315019F }));

    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Medical","Medical?", new PointF { X=0.202067211F,Y=0.245446354F }, new PointF { X=0.165942237F,Y=0.245377451F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Dental","4. Dental?", new PointF { X=0.110398151F,Y=0.245542943F }, new PointF { X=0.06719726F,Y=0.245583877F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Ignore1","(If both. complete 5-11 for dental only.)", new PointF { X=F,Y=F }, new PointF { X=0.331373632F,Y=0.2460042F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Name","5. Name of Policyholder/Subscriber in #4 (Last, First, Middle Initial, Suffix)", new PointF { X=F,Y=F }, new PointF { X=0.209576041F,Y=0.26062274F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_DOB","6. Date of Birth (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.114805743F,Y=0.289402574F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_ID","8 Policyholder/Subscriber ID (SSN or ID#)", new PointF { X=F,Y=F }, new PointF { X=0.391588032F,Y=0.290944785F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Sex_M","M", new PointF { X=0.223726645F,Y=0.3025531F }, new PointF { X=0.239640325F,Y=0.302259773F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Sex_F","F", new PointF { X=0.259282321F,Y=0.302528441F }, new PointF { X=0.2752207F,Y=0.3024253F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_PlanNo","9 Plan/Group Number", new PointF { X=F,Y=F }, new PointF { X=0.09432092F,Y=0.317573369F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Self","Self", new PointF { X=0.224116921F,Y=0.330135047F }, new PointF { X=0.244684562F,Y=0.330209255F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Spouse","Spouse", new PointF { X=0.28237316F,Y=0.3302574F }, new PointF { X=0.3109225F,Y=0.330694318F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Dependent","Dependent", new PointF { X=0.35097754F,Y=0.3301795F }, new PointF { X=0.386818618F,Y=0.3305339F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Other","Other", new PointF { X=0.431791246F,Y=0.330295026F }, new PointF { X=0.455605328F,Y=0.330015957F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_","11, Other Insurance Company/Dental Benefit Plan Name Address City, State, Zip Code", new PointF { X=F,Y=F }, new PointF { X=0.239019454F,Y=0.346023679F } ));

    //        namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_NameAndAddr", "20 Name (Last, First Middle Initial, Suffix) Address City. State, Zip Code", new PointF { X = 0.600933F, Y = 0.330510736F }, new PointF { X = 0.6777182F, Y = 0.3034424F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Self","Self", new PointF { X=0.7673692F,Y=0.28753987F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Spouse","Spouse", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Dependent","Dependent Child", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Other","Other", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
    //        namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Reserved", "Reserved For Future", new PointF { X = 0.870980263F, Y = 0.282466084F }, new PointF { X = 0.9176366F, Y = 0.277240753F }));


    //        namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_ID", "23 Patient ID/Account # (Assigned by Dentist)", new PointF { X = 0.865340769F, Y = 0.388285F }, new PointF { X = 0.8659477F, Y = 0.3738706F }));
    //        namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB", "21. Date of Birth (MM/DD/CCYY)", new PointF { X = 0.567835331F, Y = 0.388117135F }, new PointF { X = 0.586663365F, Y = 0.374154538F }));
    //        namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Sex_M", "M", new PointF { X = 0.697230756F, Y = 0.386646748F }, new PointF { X = 0.712651968F, Y = 0.3867614F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Sex_F","F", new PointF { X=F,Y=F }, new PointF { X=0.7463434F,Y=0.386944264F } ));

    //        namedCoordinates.Add(new NamedCoordinate("Services", "Services_MissingTeath", "33 Missing Teeth Information (Place an X on each missing tooth.)", new PointF { X = 0.240448266F, Y = 0.6092858F }, new PointF { X = 0.191075563F, Y = 0.5885275F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","34. Diagnosis Code List Qualifier", new PointF { X=F,Y=F }, new PointF { X=0.505822659F,Y=0.5889741F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","31a Other Fee(s)", new PointF { X=F,Y=F }, new PointF { X=0.8553593F,Y=0.5928383F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","C", new PointF { X=F,Y=F }, new PointF { X=0.713207364F,Y=0.603561938F } ));
    //        namedCoordinates.Add(new NamedCoordinate("Services", "Services_TotalFee", "32. Total Fee", new PointF { X = 0.9377053F, Y = 0.6184867F }, new PointF { X = 0.856220663F, Y = 0.615528762F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","D", new PointF { X=F,Y=F }, new PointF { X=0.7133121F,Y=0.618725538F } ));
    //        namedCoordinates.Add(new NamedCoordinate("Services", "Services_Remarks", "35 Remarks", new PointF { X = 0.143217087F, Y = 0.63222307F }, new PointF { X = 0.072160475F, Y = 0.6306957F }));

    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Enclosures", "39. Enclosures (Y or N)", new PointF { X = 0.8583816F, Y = 0.687379062F }, new PointF { X = 0.839093F, Y = 0.672511339F }));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Location", "(e-g 11office; 22O/P Hospital)", new PointF { X = 0.624753237F, Y = 0.6753882F }, new PointF { X = 0.707914233F, Y = 0.673129439F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_ApplianceDate","41. Date Appliance Placed (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.873371542F,Y=0.701274455F } ));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_Yes", "Yes (Complete 41-42)", new PointF { X = 0.638570547F, Y = 0.715175569F }, new PointF { X = 0.6996396F, Y = 0.7156286F }));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_No", "No (Skip 41-42)", new PointF { X = 0.5335592F, Y = 0.7178163F }, new PointF { X = 0.5814929F, Y = 0.7163427F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_PriorPlacementDate","44. Date of Prior Placement (MM/DD/CCYY)", new PointF { X=F,Y=F }, new PointF { X=0.8740308F,Y=0.729764163F } ));
    //        //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_","42 Months of Treatment Remaining", new PointF { X=F,Y=F }, new PointF { X=0.553362846F,Y=0.7344524F } ));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_No", "No", new PointF { X = 0.6383015F, Y = 0.7445523F }, new PointF { X = 0.6555384F, Y = 0.7439394F }));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_Yes", "Yes (Complete 44)", new PointF { X = 0.673276365F, Y = 0.744835556F }, new PointF { X = 0.7265575F, Y = 0.744526863F }));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Other", "Other accident", new PointF { X = 0.8229921F, Y = 0.7732178F }, new PointF { X = 0.867154241F, Y = 0.77169F }));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Auto", "Auto accident", new PointF { X = 0.70783174F, Y = 0.773049951F }, new PointF { X = 0.7495544F, Y = 0.771929264F }));
    //        namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Iliness", "Occupational iliness/injury", new PointF { X = 0.5351579F, Y = 0.7732332F }, new PointF { X = 0.6047592F, Y = 0.7722903F }));
    //        //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_","46 Date of Accident (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.581149042F,Y=0.788172F } ));

    //        namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignatureDate", "Date", new PointF { X = 0.403814018F, Y = 0.718785644F }, new PointF { X = 0.379310071F, Y = 0.730112255F }));
    //        namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignature", "Patient/Guardian Signature", new PointF { X = 0.13895604F, Y = 0.718584359F }, new PointF { X = 0.118631512F, Y = 0.730610669F }));
    //        namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignatureDate", "Date", new PointF { X = 0.4035308F, Y = 0.775803447F }, new PointF { X = 0.379176944F, Y = 0.7868028F }));
    //        namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignature", "Subscriber Signature", new PointF { X = 0.13857469F, Y = 0.775170565F }, new PointF { X = 0.104461655F, Y = 0.787308335F }));


    //        namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NameAndAddr", "48 Name. Address, City, State Zip Code", new PointF { X = 0.168977022F, Y = 0.860715449F }, new PointF { X = 0.135210782F, Y = 0.830174148F }));
    //        namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_EIN", "51 SSN or TIN", new PointF { X = 0.3890075F, Y = 0.9118356F }, new PointF { X = 0.378261F, Y = 0.901209235F }));
    //        namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NPI", "49 NP|", new PointF { X = 0.09441187F, Y = 0.9121314F }, new PointF { X = 0.0607231781F, Y = 0.9012483F }));
    //        namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_LicenseNo", "50. License Number", new PointF { X = 0.219463423F, Y = 0.9118469F }, new PointF { X = 0.240718365F, Y = 0.901891649F }));
    //        namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_AdditionalID", "52a Additional", new PointF { X = 0.383819759F, Y = 0.9332478F }, new PointF { X = 0.322182655F, Y = 0.929167151F }));
    //        namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_PhoneNo", "52 Phone Number", new PointF { X = 0.190277934F, Y = 0.932663739F }, new PointF { X = 0.07045753F, Y = 0.9301442F }));


    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_AdditionalID", "58 Additional Provider ID", new PointF { X = 0.83524996F, Y = 0.933086336F }, new PointF { X = 0.766909242F, Y = 0.929156661F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SignatureDate", "Date", new PointF { X = 0.884167254F, Y = 0.8474523F }, new PointF { X = 0.8512771F, Y = 0.857867062F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Name_Signature", "Signed (Treating Dentist)", new PointF { X = 0.585925043F, Y = 0.8475695F }, new PointF { X = 0.5814807F, Y = 0.8591455F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_NPI", "54 NPI", new PointF { X = 0.6137268F, Y = 0.8760797F }, new PointF { X = 0.5161233F, Y = 0.8715982F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_LicenseNo", "55 License Number", new PointF { X = 0.844708264F, Y = 0.8762593F }, new PointF { X = 0.7800867F, Y = 0.871617F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Addr", "56 Address, City State, Zip Code", new PointF { X = 0.6592705F, Y = 0.9076931F }, new PointF { X = 0.574821055F, Y = 0.8868786F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SpecialityCode", "56a Provider Specialty Code", new PointF { X = 0.883666635F, Y = 0.890574455F }, new PointF { X = 0.767791033F, Y = 0.8869426F }));
    //        namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_PhoneNo", "Phone Number", new PointF { X = 0.642414451F, Y = 0.9310866F }, new PointF { X = 0.5320557F, Y = 0.929851651F }));

    //    }

    //    public void SerializeXML(string fn)
    //    {
    //        XMLTests.SerializeToXML<XADANamedCoordinates>(fn, this);
    //    }

    //    private void DeserializeFromXML()
    //    {
    //        if (this.SourceFileName == null) throw new ArgumentNullException("SourceFileName");
    //        if (!File.Exists(this.SourceFileName)) throw new FileNotFoundException(this.SourceFileName);

    //        var serializer = new XmlSerializer(typeof(List<NamedCoordinate>));

    //        using (FileStream fileStream = new FileStream(this.SourceFileName, FileMode.Open))
    //        {
    //            this.AddRange((List<NamedCoordinate>)serializer.Deserialize(fileStream));
    //        }
    //    }

    //    //private void DeserializeFromJSON(string sourceFile, bool OverrideIgnoreAttribute = false)
    //    //{
    //    //    JsonSerializerSettings settings = new JsonSerializerSettings();
    //    //    if (OverrideIgnoreAttribute)
    //    //        settings.ContractResolver = new IgnoreJsonAttributesResolver();

    //    //    //settings.Formatting = Newtonsoft.Json.Formatting.Indented;

    //    //    return JsonConvert.DeserializeObject<List<Page>>(File.ReadAllText(sourceFile), settings);

    //    //}
    //}
}
