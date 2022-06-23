
namespace ACST.AWS.Data.EDI
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using ACST.AWS.Common;

    public class Context
    {

        #region Fields & Properties

        string _connectionString = ConfigurationManager.ConnectionStrings["EDIContext"].ConnectionString;

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();

                sb.ConnectionString = value;
                if (sb.DataSource.IsNullOrEmpty())
                    throw new ApplicationException(@"ConnectionString.set: Data Source missing.");

                if (sb.InitialCatalog.IsNullOrEmpty())
                    throw new ApplicationException(@"ConnectionString.set: Initial Catalog missing.");

                this._connectionString = value;
            }
        }
        #endregion

        #region Constructors

        public Context() { }

        public Context(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        #endregion

        public bool SetImageTransmission(DateTime dateReceived, string insuredId, string dcn, string accountNumber
            , string claimEngineKey, string memberSuffix, string subscriberSSN, string ediClaimKey, string imageName
            , string engineIdentifer = "OCR"
            , string transmissionType = "Send"
            , string imageType = "Claim"
            )
        {
            int cnt = 0;

            try
            {
                using (var connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();
                    
                    string sql = "INSERT INTO tbl_BTS_ImageTransmission ( SubscriberAMI, DCN, DateRecieved, AccountNumber, transmissionType, ClaimEngineKey "
                                + ", ImageType, MemberSuffix, SubscriberSSN, EDI_Claim_Key, ClaimEngineIdentifier, ImageName, RunTimeStamp ) "
                                + " VALUES ( @p_SubscriberAMI, @p_DCN, @p_DateRecieved, @p_AccountNumber, @p_transmissionType, @p_ClaimEngineKey "
                                + ", @p_ImageType, @p_MemberSuffix, @p_SubscriberSSN, @p_EDI_Claim_Key, @p_ClaimEngineIdentifier, @p_ImageName, @p_RunTimeStamp)";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.Add("@p_SubscriberAMI", SqlDbType.VarChar, 11).Value = insuredId ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@p_DCN", SqlDbType.VarChar, 15).Value = dcn ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@p_DateRecieved", SqlDbType.VarChar, 8).Value = dateReceived.ToString("yyyyMMdd");

                        cmd.Parameters.Add("@p_AccountNumber", SqlDbType.VarChar, 8).Value = accountNumber ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@p_transmissionType", SqlDbType.VarChar, 4).Value = transmissionType;
                        cmd.Parameters.Add("@p_ClaimEngineKey", SqlDbType.VarChar, 15).Value = claimEngineKey ?? (object)DBNull.Value;

                        cmd.Parameters.Add("@p_ImageType", SqlDbType.VarChar, 5).Value = imageType;
                        cmd.Parameters.Add("@p_MemberSuffix", SqlDbType.VarChar, 1).Value = memberSuffix ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@p_SubscriberSSN", SqlDbType.VarChar, 9).Value = subscriberSSN;

                        cmd.Parameters.Add("@p_EDI_Claim_Key", SqlDbType.VarChar, 15).Value = ediClaimKey ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@p_ClaimEngineIdentifier", SqlDbType.VarChar, 3).Value = engineIdentifer ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@p_ImageName", SqlDbType.VarChar, 18).Value = (imageName.Length >= 18 ? imageName.Substring(imageName.Length - 18, 18) : imageName).SafeTrim();

                        //cmd.Parameters.Add("@p_ImageName", SqlDbType.VarChar, 254).Value = imageName;

                        cmd.Parameters.Add("@p_RunTimeStamp", SqlDbType.DateTime2).Value = DateTime.Now;

                        cmd.CommandType = CommandType.Text;

                        // Trace parametrized query
                        string query = cmd.CommandText;
                        foreach (SqlParameter p in cmd.Parameters)
                        {
                            query = query.Replace(p.ParameterName, p.Value == null ? "null" : p.Value.ToString());
                        }
                        //Logger.TraceVerbose(query);
                        Logger.TraceInfo(query);

                        cnt = cmd.ExecuteNonQuery();
                    }

                    Logger.TraceInfo($"Insert tbl_BTS_ImageTransmission: {(cnt == 1 ? "Success" : "Failure")}.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cnt == 1;
        }

        [Conditional("DEBUG")]
        public void TestInsert()
        {
            bool f = SetImageTransmission(new DateTime(2020, 7, 30), "FX11122233", null, "AccctNo", null, null
                , "221223333", "NotFound", "ImageName.jpg");
                //, "PRI"
                //, "Send"
                //, "Claim");
        }
    }
}
