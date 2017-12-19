using DejaVu.SelfHealthCheck.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Collector
{
    public class HealthMessageDAO
    {
        string ConnectionString { get; set; }
        public HealthMessageDAO(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public HealthMessageDAO()
        {
            ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        }
        public void Save(SelfHealthMessage message)
        {
            var sqlQuery = String.Format("Insert into dbo.SelfHealthMessages([AppID],[Title],[DateChecked],[OverallStatus],[TimeElasped],[AdditionalInformation]) values('{0}','{1}','{2}','{3}','{4}','{5}') SELECT SCOPE_IDENTITY();",
                message.AppID, message.Results[0].Title, message.DateChecked, message.OverallStatus, message.Results[0].TimeElasped, message.Results[0].AdditionalInformation);


            try
            {

                using (var connection = new SqlConnection(ConnectionString))
                {
                    using (var sCommand = new SqlCommand(sqlQuery, connection))
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        sCommand.ExecuteScalar();
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SelfHealthMessage> ReturnLastUniqueMessagesWithStatusNotUp(string appID)
        {
            List<SelfHealthMessage> messages = new List<SelfHealthMessage>();
            var sqlQuery = String.Format(@"WITH CTE AS (
    SELECT  * , ROW_NUMBER() OVER ( PARTITION BY TITLE ORDER BY Id DESC) AS rownumber 
    FROM [dbo].[SelfHealthMessages]
	WHERE (AppID ='{0}')
    )
SELECT  * 
FROM CTE
WHERE rownumber = 1 AND OverallStatus != 'Up'", appID);

            try
            {

                using (var connection = new SqlConnection(ConnectionString))
                {
                    using (var sCommand = new SqlCommand(sqlQuery, connection))
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlDataReader reader = sCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                SelfHealthMessage message = new SelfHealthMessage();
                                message.AppID = reader["AppID"].ToString();
                                message.DateChecked = Convert.ToDateTime(reader["DateChecked"]);
                                message.OverallStatus = (Contracts.CheckResultStatus)Enum.Parse(typeof(Contracts.CheckResultStatus), reader["OverallStatus"].ToString());
                                messages.Add(message);
                            }
                        }
                        else
                        {
                            messages = null;
                        }
                        reader.Close();
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return messages;
        }
    }
}


