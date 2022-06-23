
namespace ACST.AWS.Data.CAMS
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    using ACST.AWS.Common;

    public class Context
    {

        #region Fields & Properties
        
        string _connectionString = ConfigurationManager.ConnectionStrings["CAMSContext"].ConnectionString;

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

        public double GetNextClaimNumber()
        {
            double nextClaimNo = 0;

            try
            {
                using (var connection = new SqlConnection(this.ConnectionString))
                {
                    var command = new SqlCommand("NextClaimNo", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    var ret = command.ExecuteScalar();

                    if (!(double.TryParse(ret.ToString(), out nextClaimNo)))
                        throw new ApplicationException("Invalid value returned from NextClaimNo.");

                    Logger.TraceInfo($"Next ClaimNo: {nextClaimNo}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return nextClaimNo;
        }

    }
}
