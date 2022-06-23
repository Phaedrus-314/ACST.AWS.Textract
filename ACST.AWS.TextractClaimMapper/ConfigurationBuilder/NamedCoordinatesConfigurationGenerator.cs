
namespace ACST.AWS.TextractClaimMapper.ConfigurationBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using ACST.AWS.Textract.Model;
    using Amazon.Textract.Model;
    using ACST.AWS.Common;

    /// <summary>
    /// Utility class usefule only creating base coordinate data
    /// Not used by Textract Process
    /// </summary>
    internal class NamedCoordinatesConfigurationGenerator
    {

        string DefaultInstanceMethodCallFormatXstl = @".\ConfigurationBuilder\DefaultInstance_MethodCallFormat.xslt";
        string HtmlXstl = @".\ConfigurationBuilder\KeyValue_HTMLFormat.xslt";

        TextractDocument AWSSourceDocument { get; set; }

        public XmlDocument HtmlFormat { get; set; }
        public XmlDocument MethodCallFormat { get; set; }

        public NamedCoordinatesConfigurationGenerator() { }

        public NamedCoordinatesConfigurationGenerator(TextractDocument textractDocument)
        {
            this.AWSSourceDocument = textractDocument;
        }

        public void Build()
        {
            // ToDo: make this more generic, not just for intermediate file generation
            XmlDocument xd = SerializeToXmlDocument(this.AWSSourceDocument);
            this.HtmlFormat = ApplyXsltTransform(xd, HtmlXstl);
            this.MethodCallFormat = ApplyXsltTransform(xd, DefaultInstanceMethodCallFormatXstl);
        }

        public XmlDocument SerializeToXmlDocument(TextractDocument textractDocument)
        {
            Type[] extraTypes = new Type[] { typeof(NewGeometry), typeof(NewBoundingBox), typeof(MatchedFieldKey) };

            //Type tt = textractDocument.Pages.GetType();
            XmlSerializer ser = new XmlSerializer(textractDocument.Pages.GetType(), extraTypes);
            //XmlSerializer ser = new XmlSerializer(textractDocument.Pages[0].GetType(), extraTypes);
            XmlDocument xd = null;

            using (MemoryStream memStm = new MemoryStream())
            {
                //ser.Serialize(memStm, textractDocument.Pages[0]);
                ser.Serialize(memStm, textractDocument.Pages);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }

            return xd;
        }

        public static XmlDocument ApplyXsltTransform(XmlDocument xd, string xsltFile)
        {
            XmlDocument resultDocument = new XmlDocument();
            using (XmlWriter xw = resultDocument.CreateNavigator().AppendChild())
            {
                XslCompiledTransform proc = new XslCompiledTransform();
                proc.Load(xsltFile);
                proc.Transform(xd, null, xw);
                xw.Close();
            }
            return resultDocument;
        }

        public static void ApplyXsltTransform(string sourceFile, string destFile, string xsltFile)
        {
            XPathDocument xpath = new XPathDocument(sourceFile);
            XslCompiledTransform proc = new XslCompiledTransform();
            proc.Load(xsltFile);
            XmlTextWriter xtw = new XmlTextWriter(destFile, null);
            proc.Transform(xpath, null, xtw);
        }

        public static void Save(XmlDocument doc, string fn)
        {
            doc.PreserveWhitespace = true;
            doc.Save(fn);
        }

        public static NamedCoordinates ADADefaultNamedCoordinateInstance()
        {
            List<NamedCoordinate> namedCoordinates = new List<NamedCoordinate>();

            //=CONCATENATE("new NamedCoordinate ('name','",D10,"',","1",",","2",", new PointF { X=",C10,"F,","Y=",B10,"F } ),")

            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_Preauthorization", "Request for Predetermination/Preauthorization", new PointF { X = 0.248578712F, Y = 0.08771375F }, new PointF { X = 0.3642209F, Y = 0.0871736854F }));
            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_ActualServices", "Statement of Actual Services", new PointF { X = 0.06297525F, Y = 0.08657699F }, new PointF { X = 0.138912886F, Y = 0.08758103F }));
            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_EPSDT", "EPSDT XIX", new PointF { X = 0.06251692F, Y = 0.101918526F }, new PointF { X = 0.113488108F, Y = 0.101589344F }));
            //namedCoordinates.Add(new NamedCoordinate ("HeaderInfo","Header_PreauthorizationNo","2. Predetermination/Preauthorization Number", new PointF { X=F,Y=F }, new PointF { X=0.145023376F,Y=0.1172243F } ));

            namedCoordinates.Add(new NamedCoordinate("InsuranceCompany", "InsuranceCompany_NameAndAddr", "3. Company/Plan Name. Address City State, Zip Code", new PointF { X = 0.13358438F, Y = 0.185889557F }, new PointF { X = 0.167338222F, Y = 0.160027787F }));

            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_NameAndAddr", "12 Policyholder/Subscriber Name (Last, First, Middle Initial, Suffix)); Address City State, Zip Code", new PointF { X = 0.601931F, Y = 0.158007056F }, new PointF { X = 0.7316091F, Y = 0.127945259F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_ID", "15. Policyholder/Subscriber ID (SSN or ID#)", new PointF { X = 0.8143239F, Y = 0.216305524F }, new PointF { X = 0.863415241F, Y = 0.201100081F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB", "13 Date of Birth (MM/DD/CCYY)", new PointF { X = 0.56922394F, Y = 0.2161932F }, new PointF { X = 0.5874541F, Y = 0.201570138F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Sex_M", "M", new PointF { X = 0.6976763F, Y = 0.215500608F }, new PointF { X = 0.7132686F, Y = 0.213894725F }));
            //namedCoordinates.Add(new NamedCoordinate ("SubscriberInfo","Subscriber_Employer","17 Employer Name", new PointF { X=F,Y=F }, new PointF { X=0.7124295F,Y=0.231463F } ));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PlanNo", "16. Plan/Group Number", new PointF { X = 0.559370458F, Y = 0.244800255F }, new PointF { X = 0.5663628F, Y = 0.2315019F }));

            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Medical","Medical?", new PointF { X=0.202067211F,Y=0.245446354F }, new PointF { X=0.165942237F,Y=0.245377451F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Dental","4. Dental?", new PointF { X=0.110398151F,Y=0.245542943F }, new PointF { X=0.06719726F,Y=0.245583877F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Ignore1","(If both. complete 5-11 for dental only.)", new PointF { X=F,Y=F }, new PointF { X=0.331373632F,Y=0.2460042F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Name","5. Name of Policyholder/Subscriber in #4 (Last, First, Middle Initial, Suffix)", new PointF { X=F,Y=F }, new PointF { X=0.209576041F,Y=0.26062274F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_DOB","6. Date of Birth (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.114805743F,Y=0.289402574F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_ID","8 Policyholder/Subscriber ID (SSN or ID#)", new PointF { X=F,Y=F }, new PointF { X=0.391588032F,Y=0.290944785F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Sex_M","M", new PointF { X=0.223726645F,Y=0.3025531F }, new PointF { X=0.239640325F,Y=0.302259773F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Sex_F","F", new PointF { X=0.259282321F,Y=0.302528441F }, new PointF { X=0.2752207F,Y=0.3024253F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_PlanNo","9 Plan/Group Number", new PointF { X=F,Y=F }, new PointF { X=0.09432092F,Y=0.317573369F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Self","Self", new PointF { X=0.224116921F,Y=0.330135047F }, new PointF { X=0.244684562F,Y=0.330209255F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Spouse","Spouse", new PointF { X=0.28237316F,Y=0.3302574F }, new PointF { X=0.3109225F,Y=0.330694318F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Dependent","Dependent", new PointF { X=0.35097754F,Y=0.3301795F }, new PointF { X=0.386818618F,Y=0.3305339F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_Other","Other", new PointF { X=0.431791246F,Y=0.330295026F }, new PointF { X=0.455605328F,Y=0.330015957F } ));
            //namedCoordinates.Add(new NamedCoordinate ("OtherCoverage","OtherCoverage_","11, Other Insurance Company/Dental Benefit Plan Name Address City, State, Zip Code", new PointF { X=F,Y=F }, new PointF { X=0.239019454F,Y=0.346023679F } ));

            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_NameAndAddr", "20 Name (Last, First Middle Initial, Suffix) Address City. State, Zip Code", new PointF { X = 0.600933F, Y = 0.330510736F }, new PointF { X = 0.6777182F, Y = 0.3034424F }));
            //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Self","Self", new PointF { X=0.7673692F,Y=0.28753987F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
            //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Spouse","Spouse", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
            //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Dependent","Dependent Child", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
            //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Other","Other", new PointF { X=F,Y=F }, new PointF { X=0.557220638F,Y=0.288476557F } ));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Reserved", "Reserved For Future", new PointF { X = 0.870980263F, Y = 0.282466084F }, new PointF { X = 0.9176366F, Y = 0.277240753F }));


            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_ID", "23 Patient ID/Account # (Assigned by Dentist)", new PointF { X = 0.865340769F, Y = 0.388285F }, new PointF { X = 0.8659477F, Y = 0.3738706F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB", "21. Date of Birth (MM/DD/CCYY)", new PointF { X = 0.567835331F, Y = 0.388117135F }, new PointF { X = 0.586663365F, Y = 0.374154538F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Sex_M", "M", new PointF { X = 0.697230756F, Y = 0.386646748F }, new PointF { X = 0.712651968F, Y = 0.3867614F }));
            //namedCoordinates.Add(new NamedCoordinate ("PatientInfo","Patient_Sex_F","F", new PointF { X=F,Y=F }, new PointF { X=0.7463434F,Y=0.386944264F } ));

            namedCoordinates.Add(new NamedCoordinate("Services", "Services_MissingTeath", "33 Missing Teeth Information (Place an X on each missing tooth.)", new PointF { X = 0.240448266F, Y = 0.6092858F }, new PointF { X = 0.191075563F, Y = 0.5885275F }));
            //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","34. Diagnosis Code List Qualifier", new PointF { X=F,Y=F }, new PointF { X=0.505822659F,Y=0.5889741F } ));
            //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","31a Other Fee(s)", new PointF { X=F,Y=F }, new PointF { X=0.8553593F,Y=0.5928383F } ));
            //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","C", new PointF { X=F,Y=F }, new PointF { X=0.713207364F,Y=0.603561938F } ));
            namedCoordinates.Add(new NamedCoordinate("Services", "Services_TotalFee", "32. Total Fee", new PointF { X = 0.9377053F, Y = 0.6184867F }, new PointF { X = 0.856220663F, Y = 0.615528762F }));
            //namedCoordinates.Add(new NamedCoordinate ("Services","Services_","D", new PointF { X=F,Y=F }, new PointF { X=0.7133121F,Y=0.618725538F } ));
            namedCoordinates.Add(new NamedCoordinate("Services", "Services_Remarks", "35 Remarks", new PointF { X = 0.143217087F, Y = 0.63222307F }, new PointF { X = 0.072160475F, Y = 0.6306957F }));

            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Enclosures", "39. Enclosures (Y or N)", new PointF { X = 0.8583816F, Y = 0.687379062F }, new PointF { X = 0.839093F, Y = 0.672511339F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Location", "(e-g 11office; 22O/P Hospital)", new PointF { X = 0.624753237F, Y = 0.6753882F }, new PointF { X = 0.707914233F, Y = 0.673129439F }));
            //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_ApplianceDate","41. Date Appliance Placed (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.873371542F,Y=0.701274455F } ));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_Yes", "Yes (Complete 41-42)", new PointF { X = 0.638570547F, Y = 0.715175569F }, new PointF { X = 0.6996396F, Y = 0.7156286F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_No", "No (Skip 41-42)", new PointF { X = 0.5335592F, Y = 0.7178163F }, new PointF { X = 0.5814929F, Y = 0.7163427F }));
            //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_PriorPlacementDate","44. Date of Prior Placement (MM/DD/CCYY)", new PointF { X=F,Y=F }, new PointF { X=0.8740308F,Y=0.729764163F } ));
            //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_","42 Months of Treatment Remaining", new PointF { X=F,Y=F }, new PointF { X=0.553362846F,Y=0.7344524F } ));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_No", "No", new PointF { X = 0.6383015F, Y = 0.7445523F }, new PointF { X = 0.6555384F, Y = 0.7439394F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_Yes", "Yes (Complete 44)", new PointF { X = 0.673276365F, Y = 0.744835556F }, new PointF { X = 0.7265575F, Y = 0.744526863F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Other", "Other accident", new PointF { X = 0.8229921F, Y = 0.7732178F }, new PointF { X = 0.867154241F, Y = 0.77169F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Auto", "Auto accident", new PointF { X = 0.70783174F, Y = 0.773049951F }, new PointF { X = 0.7495544F, Y = 0.771929264F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Iliness", "Occupational iliness/injury", new PointF { X = 0.5351579F, Y = 0.7732332F }, new PointF { X = 0.6047592F, Y = 0.7722903F }));
            //namedCoordinates.Add(new NamedCoordinate ("AncillaryInfo","AncillaryInfo_","46 Date of Accident (MM/DDICCYY)", new PointF { X=F,Y=F }, new PointF { X=0.581149042F,Y=0.788172F } ));

            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignatureDate", "Date", new PointF { X = 0.403814018F, Y = 0.718785644F }, new PointF { X = 0.379310071F, Y = 0.730112255F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignature", "Patient/Guardian Signature", new PointF { X = 0.13895604F, Y = 0.718584359F }, new PointF { X = 0.118631512F, Y = 0.730610669F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignatureDate", "Date", new PointF { X = 0.4035308F, Y = 0.775803447F }, new PointF { X = 0.379176944F, Y = 0.7868028F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignature", "Subscriber Signature", new PointF { X = 0.13857469F, Y = 0.775170565F }, new PointF { X = 0.104461655F, Y = 0.787308335F }));

            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NameAndAddr", "48 Name. Address, City, State Zip Code", new PointF { X = 0.168977022F, Y = 0.860715449F }, new PointF { X = 0.135210782F, Y = 0.830174148F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_EIN", "51 SSN or TIN", new PointF { X = 0.3890075F, Y = 0.9118356F }, new PointF { X = 0.378261F, Y = 0.901209235F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NPI", "49 NP|", new PointF { X = 0.09441187F, Y = 0.9121314F }, new PointF { X = 0.0607231781F, Y = 0.9012483F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_LicenseNo", "50. License Number", new PointF { X = 0.219463423F, Y = 0.9118469F }, new PointF { X = 0.240718365F, Y = 0.901891649F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_AdditionalID", "52a Additional", new PointF { X = 0.383819759F, Y = 0.9332478F }, new PointF { X = 0.322182655F, Y = 0.929167151F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_PhoneNo", "52 Phone Number", new PointF { X = 0.190277934F, Y = 0.932663739F }, new PointF { X = 0.07045753F, Y = 0.9301442F }));

            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_AdditionalID", "58 Additional Provider ID", new PointF { X = 0.83524996F, Y = 0.933086336F }, new PointF { X = 0.766909242F, Y = 0.929156661F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SignatureDate", "Date", new PointF { X = 0.884167254F, Y = 0.8474523F }, new PointF { X = 0.8512771F, Y = 0.857867062F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Name_Signature", "Signed (Treating Dentist)", new PointF { X = 0.585925043F, Y = 0.8475695F }, new PointF { X = 0.5814807F, Y = 0.8591455F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_NPI", "54 NPI", new PointF { X = 0.6137268F, Y = 0.8760797F }, new PointF { X = 0.5161233F, Y = 0.8715982F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_LicenseNo", "55 License Number", new PointF { X = 0.844708264F, Y = 0.8762593F }, new PointF { X = 0.7800867F, Y = 0.871617F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Addr", "56 Address, City State, Zip Code", new PointF { X = 0.6592705F, Y = 0.9076931F }, new PointF { X = 0.574821055F, Y = 0.8868786F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SpecialityCode", "56a Provider Specialty Code", new PointF { X = 0.883666635F, Y = 0.890574455F }, new PointF { X = 0.767791033F, Y = 0.8869426F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_PhoneNo", "Phone Number", new PointF { X = 0.642414451F, Y = 0.9310866F }, new PointF { X = 0.5320557F, Y = 0.929851651F }));

            var n = new NamedCoordinates();
            n.AddRange(namedCoordinates);

            return n;

        }

        public static NamedCoordinates ADADefaultNamedCoordinateInstance2()
        {
            List<NamedCoordinate> namedCoordinates = new List<NamedCoordinate>();

            //=CONCATENATE("new NamedCoordinate ('name','",D10,"',","1",",","2",", new PointF { X=",C10,"F,","Y=",B10,"F } ),")
            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_Preauthorization", "Requast for Predetemminallon/Preeuthorizatior", "NOT_SELECTED", new PointF { X = 0.3515363340F, Y = 0.064805680F }, new PointF { X = 0.2351218160F, Y = 0.064807670F }, new BoundingBox { Height = 0.01003189570F, Left = 0.24657470F, Top = 0.05978973210F, Width = 0.20992330F }, new BoundingBox { Height = 0.0135654090F, Left = 0.2265904250F, Top = 0.058024960F, Width = 0.01706279630F }));
            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_ActualServices", "Statement ol Actual Services", "SELECTED", new PointF { X = 0.1241714730F, Y = 0.064893230F }, new PointF { X = 0.04861949760F, Y = 0.06473960F }, new BoundingBox { Height = 0.0081307550F, Left = 0.05927514280F, Top = 0.0608278550F, Width = 0.129792660F }, new BoundingBox { Height = 0.01320005390F, Left = 0.04036573690F, Top = 0.058139570F, Width = 0.01650752130F }));
            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_EPSDT", "EPSDT Title XIX", "NOT_SELECTED", new PointF { X = 0.09911474590F, Y = 0.079240630F }, new PointF { X = 0.04815450F, Y = 0.080179420F }, new BoundingBox { Height = 0.007850140F, Left = 0.059763680F, Top = 0.075315560F, Width = 0.078702120F }, new BoundingBox { Height = 0.0135377860F, Left = 0.0397218280F, Top = 0.073410530F, Width = 0.01686534470F }));
            namedCoordinates.Add(new NamedCoordinate("HeaderInfo", "Header_PreauthorizationNo", "2 Predeterminalon/Presuthorization Number", "", new PointF { X = 0.132349730F, Y = 0.09570876510F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0083782270F, Left = 0.029083170F, Top = 0.09151965380F, Width = 0.2065331190F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));

            //namedCoordinates.Add(new NamedCoordinate("InsuranceCompany", "InsuranceCompany_NameAndAddr", "3. Company/Plan Name, Address, City state, zip Code", "UNITED FOOD AND COMMERCIAL WORKERS LOCAL 661 N. ERICSON CORDOVA, TN 38018-1006", new PointF { X = 0.1530409310F, Y = 0.140920550F }, new PointF { X = 0.2213092150F, Y = 0.1751632690F }, Field new BoundingBox { Height = 0.02916995F, Left = 0.02768238F, Top = 0.405916482F, Width = 0.129820108F }, new BoundingBox { Height = 0.04220372810F, Left = 0.036848870F, Top = 0.15406140F, Width = 0.3689206840F }));

            namedCoordinates.Add(new NamedCoordinate("InsuranceCompany", "InsuranceCompany_NameAndAddr", "3. Company/Plan Name, Address, City state, zip Code", "UNITED FOOD AND COMMERCIAL WORKERS LOCAL 661 N. ERICSON CORDOVA, TN 38018-1006", new PointF { X = 0.1530409310F, Y = 0.140920550F }, new PointF { X = 0.2213092150F, Y = 0.1751632690F }, new BoundingBox { Height = 0.0093557540F, Left = 0.02794478270F, Top = 0.1362426730F, Width = 0.2501922850F }, new BoundingBox { Height = 0.04220372810F, Left = 0.036848870F, Top = 0.15406140F, Width = 0.3689206840F }));

            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_NameAndAddr", "12. Pollcyholder/Subscriber Name (Last, First Aldidle inittal, Suffix) Address, City, State, Zip Code", "SIMMONS, SHARRON \"SHAY\" 1505 EVERGREEN COVINGTON, TN 38019", new PointF { X = 0.72438350F, Y = 0.111075170F }, new PointF { X = 0.6089580660F, Y = 0.1451145710F }, new BoundingBox { Height = 0.01176857670F, Left = 0.5019352440F, Top = 0.1051908810F, Width = 0.44489640F }, new BoundingBox { Height = 0.037312760F, Left = 0.5094237330F, Top = 0.12645820F, Width = 0.1990686360F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB", "13 Date of Birth (MM/DO/CCYY)", "7/5/1949", new PointF { X = 0.5760950F, Y = 0.1848598270F }, new PointF { X = 0.54971970F, Y = 0.1993373190F }, new BoundingBox { Height = 0.008858561520F, Left = 0.50233430F, Top = 0.1804305460F, Width = 0.1475213920F }, new BoundingBox { Height = 0.009211630F, Left = 0.5238185520F, Top = 0.19473150F, Width = 0.051802320F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Sex", "14 Gender", "NOT_SELECTED", new PointF { X = 0.69505640F, Y = 0.184935510F }, new PointF { X = 0.68882650F, Y = 0.2000305350F }, new BoundingBox { Height = 0.007205961270F, Left = 0.6697106360F, Top = 0.1813325290F, Width = 0.05069151520F }, new BoundingBox { Height = 0.01296181420F, Left = 0.68074620F, Top = 0.1935496330F, Width = 0.01616065580F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_ID", "15. Policyholder/subsenber ID (SSN or ID#)", "SSN: 413-82-0058", new PointF { X = 0.85564850F, Y = 0.1862719510F }, new PointF { X = 0.81976420F, Y = 0.2010277660F }, new BoundingBox { Height = 0.0098040620F, Left = 0.7540757660F, Top = 0.1813699160F, Width = 0.2031455640F }, new BoundingBox { Height = 0.0097269930F, Left = 0.7641499640F, Top = 0.1961642650F, Width = 0.1112284360F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PlanNo", "16.Plan/Group Number", "81041", new PointF { X = 0.55581640F, Y = 0.2163759320F }, new PointF { X = 0.5278451440F, Y = 0.2289061840F }, new BoundingBox { Height = 0.0086775380F, Left = 0.50195570F, Top = 0.2120371610F, Width = 0.1077214180F }, new BoundingBox { Height = 0.0095982460F, Left = 0.5093941690F, Top = 0.2241070570F, Width = 0.0369020F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Employer", "17. Employer Name", "KROGER", new PointF { X = 0.70358670F, Y = 0.216848850F }, new PointF { X = 0.6969343420F, Y = 0.2294298560F }, new BoundingBox { Height = 0.0091732770F, Left = 0.65817030F, Top = 0.2122622130F, Width = 0.090832880F }, new BoundingBox { Height = 0.0096043080F, Left = 0.66666240F, Top = 0.22462770F, Width = 0.06054389480F }));

            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Dental", "4. Dental?", "NOT_SELECTED", new PointF { X = 0.052789180F, Y = 0.2303630710F }, new PointF { X = 0.09515414390F, Y = 0.2302596270F }, new BoundingBox { Height = 0.0075195870F, Left = 0.030247620F, Top = 0.2266032840F, Width = 0.045083120F }, new BoundingBox { Height = 0.01323242210F, Left = 0.0866622850F, Top = 0.2236434220F, Width = 0.01698372330F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Medical", "Medical?", "NOT_SELECTED", new PointF { X = 0.151335180F, Y = 0.23034970F }, new PointF { X = 0.1878378540F, Y = 0.2302631880F }, new BoundingBox { Height = 0.0074667090F, Left = 0.1307596420F, Top = 0.2266163530F, Width = 0.04115106540F }, new BoundingBox { Height = 0.01339061090F, Left = 0.1781261410F, Top = 0.2235678880F, Width = 0.01942342150F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Name", "5. Name of Policyhoider/Subscriber in #4 (Last, Frit, Middle Initial, Suffix)", "", new PointF { X = 0.1958282590F, Y = 0.24650570F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.01092065590F, Left = 0.02465241770F, Top = 0.2410453710F, Width = 0.3423516750F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_DOB", "6. Date of Birth (MM/DD/CCYY)", "", new PointF { X = 0.09981587530F, Y = 0.2769922610F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0096058170F, Left = 0.02894609240F, Top = 0.2721893490F, Width = 0.1417395620F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_ID", "8. Palicyhotder/Subscriber ID (SSN or 1D#)", "", new PointF { X = 0.3807810840F, Y = 0.2779095170F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.01099752910F, Left = 0.2815750540F, Top = 0.272410750F, Width = 0.1984120760F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Dependent", "Dependert", "NOT_SELECTED", new PointF { X = 0.3750986160F, Y = 0.3204282520F }, new PointF { X = 0.3383529780F, Y = 0.31920080F }, new BoundingBox { Height = 0.01049452740F, Left = 0.3498779540F, Top = 0.3151810F, Width = 0.05044134330F }, new BoundingBox { Height = 0.01321024260F, Left = 0.3299430610F, Top = 0.31259570F, Width = 0.01681983660F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Self", "Self", "NOT_SELECTED", new PointF { X = 0.2307960090F, Y = 0.3193213340F }, new PointF { X = 0.2099778210F, Y = 0.318942070F }, new BoundingBox { Height = 0.008048680F, Left = 0.2216283680F, Top = 0.3152970F, Width = 0.018335270F }, new BoundingBox { Height = 0.01327046750F, Left = 0.2015153910F, Top = 0.3123068210F, Width = 0.01692484880F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Spouse", "Spouse", "NOT_SELECTED", new PointF { X = 0.2977111640F, Y = 0.3203338380F }, new PointF { X = 0.268828660F, Y = 0.3188688760F }, new BoundingBox { Height = 0.0097119920F, Left = 0.2801381650F, Top = 0.3154778480F, Width = 0.03514598680F }, new BoundingBox { Height = 0.01272645870F, Left = 0.2606448830F, Top = 0.3125056330F, Width = 0.016367540F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Other", "Other", "NOT_SELECTED", new PointF { X = 0.44468670F, Y = 0.3196005230F }, new PointF { X = 0.4195953610F, Y = 0.3192823230F }, new BoundingBox { Height = 0.0079265810F, Left = 0.4314938780F, Top = 0.3156372310F, Width = 0.026385680F }, new BoundingBox { Height = 0.01305216550F, Left = 0.4110130370F, Top = 0.312756240F, Width = 0.01716464760F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Sex_M", "M", "NOT_SELECTED", new PointF { X = 0.22592990F, Y = 0.2902667220F }, new PointF { X = 0.2097541690F, Y = 0.2899326680F }, new BoundingBox { Height = 0.0081841860F, Left = 0.2210433480F, Top = 0.2861746250F, Width = 0.0097731090F }, new BoundingBox { Height = 0.01341334820F, Left = 0.200855270F, Top = 0.2832259830F, Width = 0.01779779790F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_Sex_F", "F", "NOT_SELECTED", new PointF { X = 0.2625646290F, Y = 0.2902605240F }, new PointF { X = 0.2457748650F, Y = 0.2898185250F }, new BoundingBox { Height = 0.007387628310F, Left = 0.2572153810F, Top = 0.28656670F, Width = 0.01069848240F }, new BoundingBox { Height = 0.01288386530F, Left = 0.2375697490F, Top = 0.28337660F, Width = 0.01641022790F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_PlanNo", "9. Plan/Group Number", "", new PointF { X = 0.07992290F, Y = 0.30573680F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0086739190F, Left = 0.029691170F, Top = 0.3013998570F, Width = 0.1004634650F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("OtherCoverage", "OtherCoverage_OtherPlan", "11. Other Insurance Company/Dental 8enefit Plan Name; Address; City. State; Zip Code:", "", new PointF { X = 0.2291871310F, Y = 0.3352328840F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.009599140F, Left = 0.0348869860F, Top = 0.33043330F, Width = 0.38860030F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));

            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_NameAndAddr", "20. Name (Last, First; Middle Initial, Suffbx), Address, Clty. State, Zip Code", "SIMMONS, SHARRON \"SHAY\" 1505 EVERGREEN COVINGTON, TN 38019", new PointF { X = 0.66741960F, Y = 0.29240510F }, new PointF { X = 0.6083858610F, Y = 0.3264730F }, new BoundingBox { Height = 0.01105445810F, Left = 0.4988605680F, Top = 0.2868778710F, Width = 0.3371180590F }, new BoundingBox { Height = 0.03762391210F, Left = 0.50919440F, Top = 0.3076610570F, Width = 0.1983829740F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Self", "Self", "SELECTED", new PointF { X = 0.54712180F, Y = 0.2762729820F }, new PointF { X = 0.526167870F, Y = 0.2758292560F }, new BoundingBox { Height = 0.0075132740F, Left = 0.53776760F, Top = 0.272516340F, Width = 0.01870847490F }, new BoundingBox { Height = 0.0136975460F, Left = 0.51753370F, Top = 0.2689804730F, Width = 0.01726829820F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Dependent", "Dependent Child", "NOT_SELECTED", new PointF { X = 0.70353340F, Y = 0.2771307230F }, new PointF { X = 0.6541732550F, Y = 0.2760799830F }, new BoundingBox { Height = 0.008610398510F, Left = 0.6651355620F, Top = 0.27282550F, Width = 0.076795740F }, new BoundingBox { Height = 0.01387531590F, Left = 0.64562590F, Top = 0.269142330F, Width = 0.01709478530F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Spouse", "Spouse", "NOT_SELECTED", new PointF { X = 0.61270250F, Y = 0.27694530F }, new PointF { X = 0.5841417310F, Y = 0.2759624420F }, new BoundingBox { Height = 0.0079889020F, Left = 0.59528790F, Top = 0.2729508280F, Width = 0.03482916580F }, new BoundingBox { Height = 0.0132464170F, Left = 0.57560740F, Top = 0.2693392340F, Width = 0.01706866360F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Other", "Other", "NOT_SELECTED", new PointF { X = 0.7838595510F, Y = 0.2773009840F }, new PointF { X = 0.759320140F, Y = 0.27684780F }, new BoundingBox { Height = 0.007063960F, Left = 0.771330F, Top = 0.2737690F, Width = 0.02505914680F }, new BoundingBox { Height = 0.01335038620F, Left = 0.7506350F, Top = 0.2701726260F, Width = 0.01737024260F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Reserved", "Reserved For Future", "Use", new PointF { X = 0.9132209420F, Y = 0.2668088670F }, new PointF { X = 0.86506380F, Y = 0.2715901430F }, new BoundingBox { Height = 0.01690327380F, Left = 0.87445140F, Top = 0.2583572270F, Width = 0.077539140F }, new BoundingBox { Height = 0.006189048760F, Left = 0.85567620F, Top = 0.2684956190F, Width = 0.01877521350F }));

            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB", "21. Date ol Birth (MM/DO/CCYY}", "7/5/1949", new PointF { X = 0.5749332310F, Y = 0.365450650F }, new PointF { X = 0.58377620F, Y = 0.37838590F }, new BoundingBox { Height = 0.01009968390F, Left = 0.5001859660F, Top = 0.36040080F, Width = 0.1494945730F }, new BoundingBox { Height = 0.0092116720F, Left = 0.5577802660F, Top = 0.3737800720F, Width = 0.051991780F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Sex", "22 Gender", "NOT_SELECTED", new PointF { X = 0.6937895420F, Y = 0.365340620F }, new PointF { X = 0.688047230F, Y = 0.3793206210F }, new BoundingBox { Height = 0.00754796760F, Left = 0.6679932480F, Top = 0.3615666330F, Width = 0.051592580F }, new BoundingBox { Height = 0.01272590F, Left = 0.67991770F, Top = 0.3729576770F, Width = 0.01625911150F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_ID", "23. Patent ID/Account # (Assigned by Derifist)", "SSN: 413-82-0058", new PointF { X = 0.85823690F, Y = 0.36692140F }, new PointF { X = 0.8189160F, Y = 0.37984650F }, new BoundingBox { Height = 0.009749507530F, Left = 0.7553260330F, Top = 0.3620466290F, Width = 0.2058218120F }, new BoundingBox { Height = 0.0113950260F, Left = 0.7635152340F, Top = 0.3741490F, Width = 0.1108016220F }));

            namedCoordinates.Add(new NamedCoordinate("Services", "Services_MissingTeath", "33, Missing Teeth Information (Place an \"X\" on each missing looth,)", "1 2 3 4 5 6. 7 a 9 10 11 12 13 14 15 16", new PointF { X = 0.1764949860F, Y = 0.5893190F}, new PointF { X = 0.2219357490F, Y = 0.6030310F}, new BoundingBox { Height = 0.01015155670F, Left = 0.02524270860F, Top = 0.5842432380F, Width = 0.3025045390F}, new BoundingBox { Height = 0.0094517620F, Left = 0.042533520F, Top = 0.59830510F, Width = 0.3588044640F}));
            namedCoordinates.Add(new NamedCoordinate("Services", "Services_Remarks", "35. Remarks", "", new PointF { X = 0.05612135680F, Y = 0.63235420F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0070639880F, Left = 0.02778469210F, Top = 0.62882220F, Width = 0.0566733330F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));

            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Location", "38. Place of Treatment", "11", new PointF { X = 0.536384940F, Y = 0.6783771510F }, new PointF { X = 0.60912020F, Y = 0.67918810F }, new BoundingBox { Height = 0.0093596360F, Left = 0.4853369590F, Top = 0.6736973520F, Width = 0.1020960140F }, new BoundingBox { Height = 0.0084897810F, Left = 0.6024675370F, Top = 0.67494320F, Width = 0.01330530080F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Enclosures", "39. Encfosures (Y or N)", "", new PointF { X = 0.8302607540F, Y = 0.6792086360F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0085237480F, Left = 0.77662750F, Top = 0.67494680F, Width = 0.10726660F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_ApplianceDate", "41. Date Appliance Pleced (MM/DD/CCYY)", "", new PointF { X = 0.86387430F, Y = 0.70986730F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.01014848430F, Left = 0.76764070F, Top = 0.7047930360F, Width = 0.1924671680F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_No", "No (skip 41-42)", "SELECTED", new PointF { X = 0.57262810F, Y = 0.7225850F }, new PointF { X = 0.52274960F, Y = 0.7220416670F }, new BoundingBox { Height = 0.01051882930F, Left = 0.53524070F, Top = 0.71732560F, Width = 0.074774790F }, new BoundingBox { Height = 0.01327733880F, Left = 0.5143196580F, Top = 0.7154030F, Width = 0.0168598350F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_TreatmentRemaining", "Yes sIComp(ete41-42)", "NOT_SELECTED", new PointF { X = 0.68781230F, Y = 0.7231906650F }, new PointF { X = 0.6276225450F, Y = 0.7225410F }, new BoundingBox { Height = 0.01025619640F, Left = 0.63821040F, Top = 0.71806260F, Width = 0.099203780F }, new BoundingBox { Height = 0.0137329940F, Left = 0.6188425420F, Top = 0.715674460F, Width = 0.017559960F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Replacement_Yes", "42. Months of Treatment Remaining", "", new PointF { X = 0.54084610F, Y = 0.74132780F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0145712290F, Left = 0.4850623610F, Top = 0.73404220F, Width = 0.1115675420F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_PriorPlacementDate", "44. Date of Pnor Plecement (MM/DD/CCYY)", "", new PointF { X = 0.86520840F, Y = 0.73998080F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.0094043860F, Left = 0.76731170F, Top = 0.73527860F, Width = 0.1957933750F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_No", "No", "NOT_SELECTED", new PointF { X = 0.6441520F, Y = 0.7527970F }, new PointF { X = 0.6271910F, Y = 0.7532272340F }, new BoundingBox { Height = 0.007806969340F, Left = 0.63727050F, Top = 0.74889350F, Width = 0.01376299190F }, new BoundingBox { Height = 0.012420780F, Left = 0.61831830F, Top = 0.7470168470F, Width = 0.01774531790F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Orthadontics_Yes", "Yes (Complete 44)", "NOT_SELECTED", new PointF { X = 0.7158320550F, Y = 0.7534944420F }, new PointF { X = 0.66215270F, Y = 0.7533794640F }, new BoundingBox { Height = 0.0082124550F, Left = 0.6740734580F, Top = 0.74938820F, Width = 0.0835171640F }, new BoundingBox { Height = 0.01229960380F, Left = 0.65370570F, Top = 0.7472296360F, Width = 0.0168940220F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Iliness", "OcdupationalilnessAnfur", "NOT_SELECTED", new PointF { X = 0.59265710F, Y = 0.78181990F }, new PointF { X = 0.52255980F, Y = 0.7824099660F }, new BoundingBox { Height = 0.0082344660F, Left = 0.53465340F, Top = 0.77770260F, Width = 0.1160073430F }, new BoundingBox { Height = 0.01252491680F, Left = 0.51388620F, Top = 0.77614750F, Width = 0.017347210F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Auto", "Auto accident", "NOT_SELECTED", new PointF { X = 0.7397734520F, Y = 0.7822878360F }, new PointF { X = 0.696584940F, Y = 0.7831877470F }, new BoundingBox { Height = 0.007212539670F, Left = 0.7080645560F, Top = 0.77868160F, Width = 0.063417780F }, new BoundingBox { Height = 0.01245692650F, Left = 0.6879235510F, Top = 0.77695930F, Width = 0.01732272280F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_Other", "Other accident", "NOT_SELECTED", new PointF { X = 0.8596170540F, Y = 0.78300220F }, new PointF { X = 0.81413960F, Y = 0.7844081520F }, new BoundingBox { Height = 0.007533777510F, Left = 0.82516670F, Top = 0.77923530F, Width = 0.06890070F }, new BoundingBox { Height = 0.01299839280F, Left = 0.8055020570F, Top = 0.7779090F, Width = 0.01727505590F }));
            namedCoordinates.Add(new NamedCoordinate("AncillaryInfo", "AncillaryInfo_DateOfAccident", "46. Date of Accident (MM/DDICCYY)", "", new PointF { X = 0.56812170F, Y = 0.79779760F }, new PointF { X = 0F, Y = 0F }, new BoundingBox { Height = 0.009745010F, Left = 0.4835175280F, Top = 0.79292510F, Width = 0.1692083480F }, new BoundingBox { Height = 0F, Left = 0F, Top = 0F, Width = 0F }));

            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignature", "Patient/Guardian Signature", "SHARRON \"SHAY\" SIM", new PointF { X = 0.10182950F, Y = 0.7368814350F }, new PointF { X = 0.12943440F, Y = 0.7238236670F }, new BoundingBox { Height = 0.008406538520F, Left = 0.04096308720F, Top = 0.73267820F, Width = 0.1217328240F }, new BoundingBox { Height = 0.01013401520F, Left = 0.048225050F, Top = 0.71875670F, Width = 0.1624187080F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignatureDate", "Date", "6/3/2020", new PointF { X = 0.3641693590F, Y = 0.7373070F }, new PointF { X = 0.3768631520F, Y = 0.72409340F }, new BoundingBox { Height = 0.0076056790F, Left = 0.3531028630F, Top = 0.73350420F, Width = 0.02213298340F }, new BoundingBox { Height = 0.0094534940F, Left = 0.35071160F, Top = 0.71936660F, Width = 0.052303060F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignature", "Subscriber Signature", "SHARRON \"SHAY\" SIM", new PointF { X = 0.086983570F, Y = 0.794952750F }, new PointF { X = 0.1258313660F, Y = 0.7825759650F }, new BoundingBox { Height = 0.0081763090F, Left = 0.03965889660F, Top = 0.79086460F, Width = 0.09464934470F }, new BoundingBox { Height = 0.009711768480F, Left = 0.04291400310F, Top = 0.77772010F, Width = 0.1658347250F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_InsuredSignatureDate", "Date", "6/3/2020", new PointF { X = 0.3638826610F, Y = 0.7949939370F }, new PointF { X = 0.3759182390F, Y = 0.78300820F }, new BoundingBox { Height = 0.007228350270F, Left = 0.3529643710F, Top = 0.791379750F, Width = 0.02183660F }, new BoundingBox { Height = 0.0089365520F, Left = 0.3496748210F, Top = 0.7785399560F, Width = 0.05248685550F }));

            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NameAndAddr", "48, Neme, Address, City, State; Zip Code", "James Baddour, III, DDS, PLLC 426 Highway 51 South Covington, TN 38019", new PointF { X = 0.11745180F, Y = 0.83944190F }, new PointF { X = 0.1265593170F, Y = 0.8738050F }, new BoundingBox { Height = 0.0096062360F, Left = 0.0259529930F, Top = 0.83463880F, Width = 0.1829976140F }, new BoundingBox { Height = 0.03907624260F, Left = 0.03210529310F, Top = 0.85426690F, Width = 0.1889080550F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_NPI", "49. NPI", "1326142852", new PointF { X = 0.04282417520F, Y = 0.91215970F }, new PointF { X = 0.07115964590F, Y = 0.92642890F }, new BoundingBox { Height = 0.007260306270F, Left = 0.025764420F, Top = 0.90852950F, Width = 0.034119510F }, new BoundingBox { Height = 0.0096792540F, Left = 0.036273790F, Top = 0.92158930F, Width = 0.06977171450F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_LicenseNo", "50. Liconse Number", "DS3557", new PointF { X = 0.22242520F, Y = 0.9133042690F }, new PointF { X = 0.2112009530F, Y = 0.9274220470F }, new BoundingBox { Height = 0.0082483220F, Left = 0.1756290F, Top = 0.90918010F, Width = 0.093592380F }, new BoundingBox { Height = 0.00961780F, Left = 0.184680670F, Top = 0.9226131440F, Width = 0.05304056780F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_EIN", "51. SSN or TIN", "27-4403837", new PointF { X = 0.363116860F, Y = 0.9140410420F }, new PointF { X = 0.3822781440F, Y = 0.92751770F }, new BoundingBox { Height = 0.0080379250F, Left = 0.32730760F, Top = 0.91002210F, Width = 0.07161850480F }, new BoundingBox { Height = 0.0090112470F, Left = 0.3469866810F, Top = 0.92301210F, Width = 0.07058290390F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_PhoneNo", "52, Phone Number", "(901) 476-3626", new PointF { X = 0.05191572380F, Y = 0.9422805310F }, new PointF { X = 0.1323290910F, Y = 0.9423039560F }, new BoundingBox { Height = 0.014797660F, Left = 0.02448390F, Top = 0.93488170F, Width = 0.05486364660F }, new BoundingBox { Height = 0.01053698640F, Left = 0.086749440F, Top = 0.9370354410F, Width = 0.091159310F }));
            namedCoordinates.Add(new NamedCoordinate("BillingDentist", "BillingDentist_AdditionalID", "52a Additional", "Provider ID", new PointF { X = 0.3062708380F, Y = 0.93947530F }, new PointF { X = 0.3187879320F, Y = 0.9474628570F }, new BoundingBox { Height = 0.007602941240F, Left = 0.27110670F, Top = 0.9356738330F, Width = 0.07032826540F }, new BoundingBox { Height = 0.0083720680F, Left = 0.2922761440F, Top = 0.94327680F, Width = 0.05302356560F }));

            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Name_Signature", "Signed (Treating Dentist)", "James Baddour, DDS", new PointF { X = 0.5675823690F, Y = 0.8708126540F }, new PointF { X = 0.56877790F, Y = 0.8569840F }, new BoundingBox { Height = 0.0093599720F, Left = 0.51068570F, Top = 0.86613270F, Width = 0.1137933280F }, new BoundingBox { Height = 0.01048363280F, Left = 0.5056698320F, Top = 0.85174220F, Width = 0.1262161280F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SignatureDate", "Date", "6/3/2020", new PointF { X = 0.8420080540F, Y = 0.8717961310F }, new PointF { X = 0.83824160F, Y = 0.85799920F }, new BoundingBox { Height = 0.007464334370F, Left = 0.8310312630F, Top = 0.8680640F, Width = 0.02195354550F }, new BoundingBox { Height = 0.0090697410F, Left = 0.8117912410F, Top = 0.8534643650F, Width = 0.05290062350F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_NPI", "54, NPI", "1326142852", new PointF { X = 0.5018690F, Y = 0.88361980F }, new PointF { X = 0.57568340F, Y = 0.8845913410F }, new BoundingBox { Height = 0.007413215470F, Left = 0.4847014250F, Top = 0.8799131510F, Width = 0.03433514390F }, new BoundingBox { Height = 0.009230806490F, Left = 0.539098740F, Top = 0.87997590F, Width = 0.073169380F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_LicenseNo", "55. License Numbor", "DS3557", new PointF { X = 0.76941350F, Y = 0.8852559330F }, new PointF { X = 0.8396930690F, Y = 0.88605590F }, new BoundingBox { Height = 0.0081131780F, Left = 0.7237484460F, Top = 0.881199360F, Width = 0.09133010F }, new BoundingBox { Height = 0.0092159450F, Left = 0.81578370F, Top = 0.88144790F, Width = 0.047818790F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_SpecialityCode", "56e Provider Specialty Code", "122360001X", new PointF { X = 0.7577135560F, Y = 0.90195620F }, new PointF { X = 0.85662140F, Y = 0.9018001560F }, new BoundingBox { Height = 0.01600333490F, Left = 0.7230999470F, Top = 0.89395450F, Width = 0.069227230F }, new BoundingBox { Height = 0.0095703690F, Left = 0.8165083530F, Top = 0.8970150F, Width = 0.08022606370F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_Addr", "56. Address. City, State, Zip Code", "426 Highway 51 South Covington, TN 38019", new PointF { X = 0.5609063510F, Y = 0.89966430F }, new PointF { X = 0.5730505590F, Y = 0.92299060F }, new BoundingBox { Height = 0.01016178080F, Left = 0.4841849210F, Top = 0.89458340F, Width = 0.1534428150F }, new BoundingBox { Height = 0.02440691180F, Left = 0.5044015650F, Top = 0.9107871650F, Width = 0.1372980F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_PhoneNo", "57. Phono Number", "(901) 476-3626", new PointF { X = 0.5103026630F, Y = 0.94412990F }, new PointF { X = 0.61331110F, Y = 0.94492390F }, new BoundingBox { Height = 0.015676230F, Left = 0.482555270F, Top = 0.9362917540F, Width = 0.05549472570F }, new BoundingBox { Height = 0.01084073730F, Left = 0.56709640F, Top = 0.93950350F, Width = 0.09242935480F }));
            namedCoordinates.Add(new NamedCoordinate("TreatingDentist", "TreatingDentist_AdditionalID", "58. Additional", "Provider ID", new PointF { X = 0.75479660F, Y = 0.9424570F }, new PointF { X = 0.766142130F, Y = 0.94980540F }, new BoundingBox { Height = 0.0069396270F, Left = 0.72364880F, Top = 0.93898720F, Width = 0.062295680F }, new BoundingBox { Height = 0.0085107310F, Left = 0.73945630F, Top = 0.945550F, Width = 0.0533716120F }));


            var n = new NamedCoordinates();
            n.AddRange(namedCoordinates);

            return n;

        }

        public static NamedCoordinates CMSDefaultNamedCoordinateInstance()
        {
            List<NamedCoordinate> namedCoordinates = new List<NamedCoordinate>();

            //namedCoordinates.Add(new NamedCoordinate("Ignore", "n", "PICA", "", new PointF { X = 0.089332990F, Y = 0.1313906310F}, new PointF { X = 0F, Y = 0F}));
            //namedCoordinates.Add(new NamedCoordinate("Ignore", "n", "(For Program in Item 1)", "", new PointF { X = 0.86024120F, Y = 0.1465050580F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "FECA BLK LUNG (IDA)", "", new PointF { X = 0.51709120F, Y = 0.153408810F}, new PointF { X = 0.4823314250F, Y = 0.1601114570F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "GROUP HEALTH PLAN (IDA)", "", new PointF { X = 0.4376653430F, Y = 0.15378470F}, new PointF { X = 0.3909258840F, Y = 0.1601572190F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "(I04)", "", new PointF { X = 0.5743102430F, Y = 0.160084710F}, new PointF { X = 0.55092610F, Y = 0.1604797840F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "(Memnter IDM)", "", new PointF { X = 0.3514858480F, Y = 0.1603415460F}, new PointF { X = 0.3104415540F, Y = 0.1605250F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "(Medicarea)", "", new PointF { X = 0.089510730F, Y = 0.16060460F}, new PointF { X = 0.05196243520F, Y = 0.1606004240F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "(ID/DoDa)", "", new PointF { X = 0.2464504840F, Y = 0.1607921420F}, new PointF { X = 0.2083872710F, Y = 0.160611480F}));
            // namedCoordinates.Add(new NamedCoordinate("TypeInfo", "n", "(Medicaidm)", "", new PointF { X = 0.1671287420F, Y = 0.16080470F}, new PointF { X = 0.1292747410F, Y = 0.1605033730F}));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Name", "2 PATIENT'S NAME (Last Name, First Name, Middle Initial)", "MAYFIELD, DAX", new PointF { X = 0.1718656120F, Y = 0.175472960F }, new PointF { X = 0.11519850F, Y = 0.18818320F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB_YY", "YY", "2018", new PointF { X = 0.47639870F, Y = 0.1802170130F }, new PointF { X = 0.4668975170F, Y = 0.1902915240F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB_MM", "MM", "02", new PointF { X = 0.3978462820F, Y = 0.1803497370F }, new PointF { X = 0.39885180F, Y = 0.190617010F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_DOB_DD", "DO", "08", new PointF { X = 0.4330226480F, Y = 0.18039210F }, new PointF { X = 0.4269304280F, Y = 0.1904007350F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Sex_M", "M", "", new PointF { X = 0.5025785570F, Y = 0.1880619820F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Sex_F", "F", "", new PointF { X = 0.55772060F, Y = 0.1885004790F }, new PointF { X = 0.57282350F, Y = 0.187892020F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Other", "Other", "", new PointF { X = 0.55059110F, Y = 0.2165920440F }, new PointF { X = 0.5742193460F, Y = 0.2166849820F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Child", "Child", "", new PointF { X = 0.4958350660F, Y = 0.2169636190F }, new PointF { X = 0.51687820F, Y = 0.2172819230F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Spouse", "Spouse", "", new PointF { X = 0.4441422520F, Y = 0.2170997410F }, new PointF { X = 0.413670570F, Y = 0.21730070F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Addr_Street", "5 PATIENTS ADDRESS (No. Street)", "2105 OPPURTUNITY DR APT B", new PointF { X = 0.1262183790F, Y = 0.2017061710F }, new PointF { X = 0.1724122170F, Y = 0.21570210F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Addr_City", "CITY", "MURRAY", new PointF { X = 0.05755080280F, Y = 0.2314863650F }, new PointF { X = 0.082385250F, Y = 0.2444023190F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Addr_State", "STATE", "KY", new PointF { X = 0.3455435340F, Y = 0.23078210F }, new PointF { X = 0.3423876760F, Y = 0.2439339760F })); 
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_Addr_Zip", "ZIP CODE", "42071", new PointF { X = 0.068029570F, Y = 0.2584164740F }, new PointF { X = 0.079066280F, Y = 0.2735215720F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_PhoneNo", "TELEPHONE (Include Area Code)", "( 270) 8750827", new PointF { X = 0.2667689620F, Y = 0.25888780F }, new PointF { X = 0.2567547260F, Y = 0.274280250F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_OtherInsuranceName", "9 OTHER INSURED'S NAME (Last Name, First Name. Middie initial)", "", new PointF { X = 0.19081220F, Y = 0.2898785770F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_OtherInsuranceNo", "a. OTHER INSURED'S POLICY OR GROUP NUMBER", "", new PointF { X = 0.1631380F, Y = 0.3160028460F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("PatientInfo", "Patient_PlanName", "d. INSURANCE PLAN NAME OR PROGRAM NAME", "", new PointF { X = 0.1551711560F, Y = 0.40182180F }, new PointF { X = 0F, Y = 0F }));

            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignatureDate", "DATE", "08 09 2019", new PointF { X = 0.4244120720F, Y = 0.47293340F }, new PointF { X = 0.51102730F, Y = 0.46685680F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_PatientSignature", "SIGNED", "SOF", new PointF { X = 0.079358250F, Y = 0.47310240F }, new PointF { X = 0.1183926460F, Y = 0.46769810F }));
            // namedCoordinates.Add(new NamedCoordinate("Ignore", "n", "8 RESERVED FOR NUCC USE", "", new PointF { X = 0.4404818420F, Y = 0.2315743860F}, new PointF { X = 0F, Y = 0F}));

            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_ID", "1a. INSURED'S ID. NUMBER", "3336956", new PointF { X = 0.66764470F, Y = 0.1465457680F }, new PointF { X = 0.64441420F, Y = 0.1598681510F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Name", "4 INSURED'S NAME (Last Name, First Name, Middle Initial)", "MAYFIELD, CHRIS, A", new PointF { X = 0.72928670F, Y = 0.1750096830F }, new PointF { X = 0.6969021560F, Y = 0.1880053130F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Addr_Street", "7. INSURED'S ADDRESS (No., Street)", "503 BROAD ST", new PointF { X = 0.68455340F, Y = 0.2015550430F }, new PointF { X = 0.66720650F, Y = 0.2147629560F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Addr_City", "CITY", "MURRAY", new PointF { X = 0.61137590F, Y = 0.2314259560F }, new PointF { X = 0.63778070F, Y = 0.24308170F })); 
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Addr_State", "STATE", "KY", new PointF { X = 0.8868964310F, Y = 0.23011440F }, new PointF { X = 0.89451780F, Y = 0.2419798520F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Addr_Zip", "ZIP CODE", "42071", new PointF { X = 0.6230354310F, Y = 0.2588628530F }, new PointF { X = 0.63347580F, Y = 0.27099650F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PhoneNo", "TELEPHONE (Include Area Code)", "( )", new PointF { X = 0.8218226430F, Y = 0.2582785790F }, new PointF { X = 0.78894180F, Y = 0.2736313640F }));
            
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PlanNo", "11, INSURED' S POLICY GROUP OR FECA NUMBER", "", new PointF { X = 0.7144977450F, Y = 0.28850440F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB_YY", "YY", "1989", new PointF { X = 0.7249340420F, Y = 0.32297010F }, new PointF { X = 0.7230220F, Y = 0.3320098220F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB_MM", "MM", "05", new PointF { X = 0.6483863590F, Y = 0.32301350F }, new PointF { X = 0.6555056570F, Y = 0.33254470F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_DOB_DD", "DD", "30", new PointF { X = 0.6825068590F, Y = 0.32305550F }, new PointF { X = 0.6847715380F, Y = 0.3326693480F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Sex_F", "F", "", new PointF { X = 0.8763857480F, Y = 0.3286112250F }, new PointF { X = 0.8926638360F, Y = 0.33008890F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_Sex_M", "M", "X", new PointF { X = 0.79352480F, Y = 0.32907530F }, new PointF { X = 0.8109222650F, Y = 0.3301745650F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_PlanName", "c INSURANCE PLAN NAME OR PROGRAM NAME", "", new PointF { X = 0.70892680F, Y = 0.3731647730F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_OtherPlan_Yes", "YES", "", new PointF { X = 0.64964540F, Y = 0.415663630F }, new PointF { X = 0.62806310F, Y = 0.41527860F }));
            namedCoordinates.Add(new NamedCoordinate("SubscriberInfo", "Subscriber_OtherPlan_No", "NO", "", new PointF { X = 0.70500690F, Y = 0.4160280820F }, new PointF { X = 0.685140F, Y = 0.41551490F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_SubscriberSignature", "SIGNED", "SOF", new PointF { X = 0.64023810F, Y = 0.4731571670F }, new PointF { X = 0.68463970F, Y = 0.4665860830F }));

            // namedCoordinates.Add(new NamedCoordinate("Ignore", "n", "b. RESERVED FOR NucOUS", "", new PointF { X = 0.1144818290F, Y = 0.343575180F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("Ignore", "n", "c RESERVED FOR NUCC USE", "", new PointF { X = 0.1144276340F, Y = 0.3733326490F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("Ignore", "n", "10d CLAIM CODES (Designated by NUCC)", "", new PointF { X = 0.4636410180F, Y = 0.403028220F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "NO", "", new PointF { X = 0.5241582390F, Y = 0.33068970F}, new PointF { X = 0.5039719340F, Y = 0.3309481740F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "YES", "", new PointF { X = 0.45833220F, Y = 0.3311078550F}, new PointF { X = 0.4355930690F, Y = 0.3308649360F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "b OTHER CLAIM ID (Desigriated by NUCC)", "", new PointF { X = 0.69504680F, Y = 0.3471943740F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "PLACE (State)", "", new PointF { X = 0.5580644610F, Y = 0.3476837580F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "NO", "", new PointF { X = 0.52409830F, Y = 0.3583777250F}, new PointF { X = 0.5040439370F, Y = 0.3587793410F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "YES", "", new PointF { X = 0.4585745330F, Y = 0.3589110370F}, new PointF { X = 0.4355938730F, Y = 0.35885010F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "NO", "", new PointF { X = 0.52384750F, Y = 0.3869236410F}, new PointF { X = 0.5040360690F, Y = 0.3871670F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionRelatedToInfo", "n", "YES", "", new PointF { X = 0.4584301410F, Y = 0.3869670630F}, new PointF { X = 0.4367774720F, Y = 0.3870404660F}));

            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_IllnessDate_MM", "MM", "08", new PointF { X = 0.070452470F, Y = 0.4931610F }, new PointF { X = 0.072575060F, Y = 0.50343260F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_IllnessDate_DD", "DO", "07", new PointF { X = 0.1036584820F, Y = 0.4939506650F }, new PointF { X = 0.1016909850F, Y = 0.50339060F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_IllnessDate_YY", "YY", "2019", new PointF { X = 0.14500020F, Y = 0.4936199190F }, new PointF { X = 0.1441237630F, Y = 0.50310550F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_IllnessDate_Qual", "QUAL", "431", new PointF { X = 0.1887862680F, Y = 0.50225870F }, new PointF { X = 0.23680150F, Y = 0.5016818640F }));

            // namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "n", "15 OTHER DATE", "", new PointF { X = 0.3867726620F, Y = 0.4868100580F}, new PointF { X = 0F, Y = 0F}));

            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_OtherDate_MM", "MM", "08", new PointF { X = 0.4641273320F, Y = 0.49309120F }, new PointF { X = 0.4689467550F, Y = 0.5027926560F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_OtherDate_DD", "DD", "05", new PointF { X = 0.50035170F, Y = 0.49316620F }, new PointF { X = 0.4978668390F, Y = 0.5025492310F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_OtherDate_YY", "YY", "2019", new PointF { X = 0.5418370F, Y = 0.49328720F }, new PointF { X = 0.53642090F, Y = 0.50225290F }));



            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ReferringDr_Name", "17 NAME OF REFERRING PROVIDER OR OTHER SOURCE", "JOYCE HUGHES MD", new PointF { X = 0.1742210390F, Y = 0.5172317620F }, new PointF { X = 0.1435999870F, Y = 0.5276379590F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ReferringDr_ID", "17a", "", new PointF { X = 0.3584669230F, Y = 0.5147115590F }, new PointF { X = 0F, Y = 0F }));
            // namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ReferringDr_NPI", "17b", "", new PointF { X = 0F, Y = 0F}, new PointF { X = 0F, Y = 0F}));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_AdditionalInfo", "19. ADOITIONAL CLAIM INFORMATION (Designated by NUCC)", "", new PointF { X = 0.1824135180F, Y = 0.5427370F }, new PointF { X = 0F, Y = 0F }));

            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_Ind", "0", "", new PointF { X = 0.51835350F, Y = 0.57030380F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_A", "A.", "J18.", new PointF { X = 0.05310242250F, Y = 0.59029060F }, new PointF { X = 0.099589030F, Y = 0.586498260F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_B", "B.", "L", new PointF { X = 0.2003821730F, Y = 0.5905140640F }, new PointF { X = 0.2498324510F, Y = 0.5891835690F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_C", "C", "", new PointF { X = 0.3472473320F, Y = 0.59025640F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_E", "E.", "", new PointF { X = 0.053180640F, Y = 0.6036243440F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_F", "F,", "", new PointF { X = 0.2000679820F, Y = 0.60483610F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionOccurrenceInfo", "Condition_ICD_G", "G", "", new PointF { X = 0.3472714420F, Y = 0.6047530F }, new PointF { X = 0F, Y = 0F }));

            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_NoWorkDate_from_DD", "DD", "", new PointF { X = 0.69211020F, Y = 0.49312390F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_NoWorkDate_from_YY", "YY", "", new PointF { X = 0.73614890F, Y = 0.49348880F }, new PointF { X = 0F, Y = 0F }));
            // namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_NoWorkDate_from_DD", "Db", "", new PointF { X = 0.8522534370F, Y = 0.49275180F}, new PointF { X = 0F, Y = 0F}));
            // namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "n", "TO", "", new PointF { X = 0.79199080F, Y = 0.5005244610F}, new PointF { X = 0F, Y = 0F}));

            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_HospitalizationDate_from_MM", "MM", "08", new PointF { X = 0.65596460F, Y = 0.51970160F }, new PointF { X = 0.6594714520F, Y = 0.52768350F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_HospitalizationDate_from_DD", "DD", "05", new PointF { X = 0.69166420F, Y = 0.51965390F }, new PointF { X = 0.68821850F, Y = 0.52737760F }));

            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_HospitalizationDate_to_MM", "MM", "08", new PointF { X = 0.8155622480F, Y = 0.5192246440F }, new PointF { X = 0.81963250F, Y = 0.52686020F }));

            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_OutsideLab_Yes", "YES", "", new PointF { X = 0.6505970F, Y = 0.55784830F }, new PointF { X = 0.6283036470F, Y = 0.5572409630F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_OutsideLab_No", "NO", "", new PointF { X = 0.7055262330F, Y = 0.5574348570F }, new PointF { X = 0.68513390F, Y = 0.5562963490F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_OutsideLab_Charges", "$ CHARGES", "", new PointF { X = 0.80947960F, Y = 0.54191110F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_ResubmissionCode", "22 RESUBMISSION CODE", "", new PointF { X = 0.642865360F, Y = 0.57404480F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ConditionAdditionalInfo", "Condition_PriorAuth", "23. PRIOR AUTHORIZATION NUMBER", "", new PointF { X = 0.6847663520F, Y = 0.59913390F }, new PointF { X = 0F, Y = 0F }));

            // namedCoordinates.Add(new NamedCoordinate("ServiceLineHeader", "n", "YY", "", new PointF { X = 0.1231492160F, Y = 0.64556860F}, new PointF { X = 0F, Y = 0F}));

            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_EIN", "25. FEDERAL TAX ID. NUMBER", "610924889", new PointF { X = 0.1160632220F, Y = 0.8265353440F }, new PointF { X = 0.092115950F, Y = 0.84327760F }));
            // namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "n", "SSN EIN", "", new PointF { X = 0.2435490340F, Y = 0.82696230F}, new PointF { X = 0F, Y = 0F}));
            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_AccountNo", "26. PATIENT'S ACCOUNT NO", "95780261766", new PointF { X = 0.35748760F, Y = 0.8265604380F }, new PointF { X = 0.34920450F, Y = 0.84218670F }));
            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_TotalCharge", "28. TOTAL CHARGE", "$ 32 00", new PointF { X = 0.6418920F, Y = 0.8264627460F }, new PointF { X = 0.65795770F, Y = 0.8415493370F }));
            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_AmountPaid", "29 AMOUNT PAID", "$", new PointF { X = 0.7653855680F, Y = 0.8252410F }, new PointF { X = 0.7779702540F, Y = 0.84134320F }));
            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_NUUCuse", "30 Rsvd for NUCC Use", "", new PointF { X = 0.88614310F, Y = 0.8247409460F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_AcceptAssign_Yes", "YES", "", new PointF { X = 0.4906630220F, Y = 0.8416230F }, new PointF { X = 0.4681899850F, Y = 0.8403601650F }));
            namedCoordinates.Add(new NamedCoordinate("ProviderInfo", "Provider_AcceptAssign_No", "NO", "", new PointF { X = 0.54455550F, Y = 0.841660F }, new PointF { X = 0.5255790350F, Y = 0.84142010F }));
            namedCoordinates.Add(new NamedCoordinate("Authorizations", "Authorizations_ProviderSignature", "31. SIGNATURE OF PHYSICIAN OR SUPPLIER INCLUDING DEGREES OR CREDENTIALS (I certify that the statements on the reverse apply to this bill and are made a part thereof.)", "HINES, MD", new PointF { X = 0.1489969490F, Y = 0.8676269650F }, new PointF { X = 0.1630005840F, Y = 0.9049610F }));
            namedCoordinates.Add(new NamedCoordinate("BillingProviderInfo", "BillingProvider_PhoneNo", "33 BILLING PROVIDER INFO &amp; PH", "7594524", new PointF { X = 0.6785484550F, Y = 0.85491590F }, new PointF { X = 0.8671478630F, Y = 0.8551962380F }));
            namedCoordinates.Add(new NamedCoordinate("BillingProviderInfo", "BillingProvider_ID", "a.", "", new PointF { X = 0.2954315540F, Y = 0.9122315650F }, new PointF { X = 0F, Y = 0F }));
            namedCoordinates.Add(new NamedCoordinate("BillingProviderInfo", "BillingProvider_AdditionalID", "a.", "1497789523", new PointF { X = 0.601529360F, Y = 0.91151720F }, new PointF { X = 0.66467390F, Y = 0.91196650F }));

            var n = new NamedCoordinates();
            n.AddRange(namedCoordinates);

            return n;
        }

        public static void GenerateIntermediateConfig(NamedCoordinates nc, string fn, string destPath)
        {
            AWSTextractClaimCache<ADA.ADAClaim> cache = new AWSTextractClaimCache<ADA.ADAClaim>(fn);
            cache.OpenTextractClaim();
            
            string keyName = string.Format("GeneratedIntermediateConfig_{0}", Path.GetFileName(fn));
            string xmlFn = Path.Combine(destPath, Path.ChangeExtension(keyName, "xml"));
            string htmlFn = Path.Combine(destPath, Path.ChangeExtension(keyName, "html"));

            NamedCoordinatesConfigurationGenerator generator = new NamedCoordinatesConfigurationGenerator(cache.TextractDocument);
            generator.Build();
            NamedCoordinatesConfigurationGenerator.Save(generator.HtmlFormat, htmlFn);
            NamedCoordinatesConfigurationGenerator.Save(generator.MethodCallFormat, xmlFn);
        }

        public static void GenerateConfig(NamedCoordinates nc, string fn)
        {
            Serializer.SerializeToXML<NamedCoordinates>(nc, fn);
        }

    }
}
