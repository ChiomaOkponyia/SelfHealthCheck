using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Data
{
    public class HealthMessageDAO: EntityDb<SelfHealthMessage>
    {
        string ConnectionString { get; set; }
        public HealthMessageDAO(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public HealthMessageDAO()
        {
            ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        }
        public void Save(SelfHealthMessage message)
        {
            string date = message.DateChecked.ToString("MM/dd/yyyy h:mm:ss");
            // DateTime formatedDate;
            // message.DateChecked = DateTime.Parse(date);
            //if (!DateTime.TryParse(date, out formatedDate))
            //{
            //    Console.WriteLine("failed");
            //}
            //message.DateChecked = formatedDate;
            var sqlQuery = String.Format("Insert into dbo.SelfHealthMessage([AppID],[Title],[DateChecked],[OverallStatus],[TimeElapsed],[AdditionalInformation]) values('{0}','{1}','{2}','{3}','{4}','{5}') SELECT SCOPE_IDENTITY();",
                message.AppID, message.Title, date, message.OverallStatus, message.TimeElapsed, message.AdditionalInformation);


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

        public static List<SelfHealthMessage> Find(string searchCriteria, int page, int pageSize, string sort, string direction, out int totalItemsCount)
        {
            List<SelfHealthMessage> result = new List<SelfHealthMessage>();
            ISession session = BuildSession();
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(SelfHealthMessage));


                //Order in Ascending order of Name
                if (!String.IsNullOrEmpty(searchCriteria) )
                {
                    criteria.Add(Expression.Or(Expression.Or(Expression.Or(Expression.Like("AppName", searchCriteria, MatchMode.Anywhere),
                        Expression.Eq("AppID", searchCriteria)), Expression.Eq("IPAddress", searchCriteria)), Expression.Eq("Status", searchCriteria)));
                }
                result = criteria.List<SelfHealthMessage>() as List<SelfHealthMessage>;

               

                if (result != null)
                    totalItemsCount = result.Count;
                else totalItemsCount = 0;
                result = new List<SelfHealthMessage>();


                //Before doing the sorting, i get a count Criteria so that it doesnt crash.
                ICriteria countCriteria = CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCountInt64());
                ICriteria listCriteria = CriteriaTransformer.Clone(criteria).SetFirstResult(page * pageSize).SetMaxResults(pageSize);


                //This section then performs the sort operations on the list. Sorting defaults to the Name column
                if (direction == "Default")
                {
                    //listCriteria.AddOrder(Order.Asc("LastName"));
                    listCriteria.AddOrder(Order.Asc("Id"));
                }
                else
                {
                    if (direction == "DESC")
                    {
                        listCriteria.AddOrder(Order.Desc(sort));
                    }
                    else
                    {
                        listCriteria.AddOrder(Order.Asc(sort));
                    }
                }
                //Add the two criteria to the session and retrieve their result.
                //IList allResults = session.CreateMultiCriteria().Add(listCriteria).Add(countCriteria).List();
                IList allResults = listCriteria.List();
                foreach (var o in allResults)
                {
                    result.Add((SelfHealthMessage)o);
                }

            }
            catch(Exception ex)
            {
                Trace.TraceInformation("Error while Searching " + ex.Message);
                throw;
            }
            return result;

        }

       
        public List<SelfHealthMessage> ReturnLastUniqueMessagesWithStatusNotUp(string appID)
        {
            List<SelfHealthMessage> messages = new List<SelfHealthMessage>();
            var sqlQuery = String.Format(@"WITH CTE AS (
    SELECT  * , ROW_NUMBER() OVER ( PARTITION BY TITLE ORDER BY Id DESC) AS rownumber 
    FROM [dbo].[SelfHealthMessage]
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
        public List<SelfHealthMessage> GetMostRecentFailedHealthMessageChecks(string appID, string IPAddress)
        {
            List<SelfHealthMessage> messages = new List<SelfHealthMessage>();
            string query = string.Format("SELECT * FROM  dbo.HealthMessageStatus where AppID ='{0}' and IPAddress = '{1}' and Status != '{2}' ", appID, IPAddress, Contracts.CheckResultStatus.Up.ToString());

            try
            {

                using (var connection = new SqlConnection(ConnectionString))
                {
                    using (var sCommand = new SqlCommand(query, connection))
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
                                message.Title = reader["CheckType"].ToString();
                                message.IPAddress = reader["IPAddress"].ToString();
                                message.AdditionalInformation = reader["AdditionalInformation"].ToString();
                                string date = reader["LastUpdated"].ToString();
                                message.DateChecked = Convert.ToDateTime(date);
                                message.OverallStatus = (Contracts.CheckResultStatus)Enum.Parse(typeof(Contracts.CheckResultStatus), reader["Status"].ToString());
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
                Trace.TraceInformation("Error Occured while geting message: {0}      ,    stacktrace {1}", ex.Message, ex.StackTrace);
                throw ex;
            }
            return messages;

        }
        public void SaveHealthStatus(SelfHealthMessage message)
        {
            string lastUpdated = DateTime.Now.ToString("MM/dd/yyyy h:mm:ss");
            string nextCheckTime = message.NextCheckTime.ToString("MM/dd/yyyy h:mm:ss");

            var sqlQuery = String.Format("Insert into dbo.HealthMessageStatus([AppID],[CheckType],[Status],[LastUpdated],[IPAddress],[AdditionalInformation],[NextCheckTime]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') SELECT SCOPE_IDENTITY();",
               message.AppID, message.Title, message.OverallStatus, lastUpdated, message.IPAddress, message.AdditionalInformation, nextCheckTime);


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
                Trace.TraceInformation("Error Occured while Saving health status: " + ex.Message + "\n" + ex.StackTrace);

                throw ex;

            }
        }
        public void UpdateHealthStatus(SelfHealthMessage message)
        {
            string lastUpdated = DateTime.Now.ToString("MM/dd/yyyy h:mm:ss");
            string nextCheckTime = message.NextCheckTime.ToString("MM/dd/yyyy h:mm:ss");

            var sqlQuery = String.Format("Update dbo.HealthMessageStatus SET status = '{0}' , LastUpdated ='{1}' , NextCheckTime = '{2}', AdditionalInformation = '{3}' where AppID = '{4}' and CheckType ='{5}' and IPAddress ='{6}';",
               message.OverallStatus, lastUpdated, nextCheckTime, message.AdditionalInformation, message.AppID, message.Title, message.IPAddress);


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
                Trace.TraceInformation("Error Occured while Updating  : " + ex.Message + "\n" + ex.StackTrace);

                throw ex;

            }
        }

        public SelfHealthMessage GetCheckTypeForApp(SelfHealthMessage message)
        {
            Trace.TraceInformation("About to Get Check Type");

            SelfHealthMessage healthStatus = null;
            string query = string.Format("SELECT * FROM dbo.HealthMessageStatus where AppID ='{0}' and CheckType ='{1}' and IPAddress = '{2}'  ", message.AppID, message.Title, message.IPAddress);
            try
            {

                using (var connection = new SqlConnection(ConnectionString))
                {
                    using (var sCommand = new SqlCommand(query, connection))
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlDataReader reader = sCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            healthStatus = new SelfHealthMessage();
                            while (reader.Read())
                            {
                                healthStatus.AppID = reader["AppID"].ToString();
                                healthStatus.Title = reader["CheckType"]?.ToString();
                                healthStatus.IPAddress = reader["IPAddress"]?.ToString();
                                healthStatus.AdditionalInformation = reader["AdditionalInformation"]?.ToString();
                                string date = reader["LastUpdated"].ToString();
                                healthStatus.DateChecked = Convert.ToDateTime(date);
                                healthStatus.NextCheckTime = Convert.ToDateTime(reader["NextCheckTime"].ToString());
                                healthStatus.OverallStatus = (Contracts.CheckResultStatus)Enum.Parse(typeof(Contracts.CheckResultStatus), reader["Status"].ToString());

                            }
                        }

                        reader.Close();
                    }
                };
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Error Occured while getting health status : " + ex.Message + "\n" + ex.StackTrace);

                throw ex;
            }
            return healthStatus;
        }

        public List<string> GetAllIPAddressesForApp(string appID)
        {
            List<string> IPAddresses = null;

            string query = String.Format(@"WITH CTE AS (
    SELECT * , ROW_NUMBER() OVER(PARTITION BY IPAddress ORDER BY Id DESC) AS rownumber
    FROM[dbo].[HealthMessageStatus]

    WHERE(AppID = '{0}')
    )
SELECT*
FROM CTE
WHERE rownumber = 1 ", appID);


            try
            {

                using (var connection = new SqlConnection(ConnectionString))
                {
                    using (var sCommand = new SqlCommand(query, connection))
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlDataReader reader = sCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            IPAddresses = new List<string>();
                            while (reader.Read())
                            {
                                IPAddresses.Add(reader["IPAddress"].ToString());
                            }
                        }
                        
                        reader.Close();
                    }
                };
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Error while getting IPAddresses: {0}  Stacktrace {1}", ex.Message, ex.StackTrace);
                throw ex;
            }
            return IPAddresses;
        }

       
    }
}


