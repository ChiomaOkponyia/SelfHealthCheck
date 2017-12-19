using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.Engine
{
    public class DatabaseCheckRunner : ICheckRunner
    {
        private string QUERY_FORMAT = "SELECT TOP 50 * FROM {0}";

        public ICheckResult Check(ICheckConfiguration checkDetails)
        {
            var databaseDetails = checkDetails as IDatabaseConfiguration;

            ICheckResult result = new CheckResult()
            {
                Title = databaseDetails.Title
            };


            Stopwatch timer = new Stopwatch();
            
            //Create Db Connection and attempt to connect
            using (SqlConnection conn = new SqlConnection(databaseDetails.ConnectionString))
            {
                try
                {
                    timer.Start();
                    conn.Open();

                    if (databaseDetails.TablesToCheck != null)
                    {
                        foreach (var table in databaseDetails.TablesToCheck)
                        {
                            SqlCommand command = new SqlCommand(string.Format(QUERY_FORMAT, table), conn);
                            var dataReader = command.ExecuteReader();
                            while (dataReader.Read())
                            {

                            }
                        }
                    }
                    timer.Stop();
                    result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);

                    result.Status = databaseDetails.ResponseTime > result.TimeElasped ? CheckResultStatus.Up : CheckResultStatus.PerfomanceDegraded;
                }
                catch (SqlException ex)
                {
                    timer.Stop();
                    result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);

                    result.AdditionalInformation = string.Format("{0} - {1}", ex.ErrorCode, ex.Message);

                    result.Status = CheckResultStatus.Down;
                }
                finally
                {
                    conn.Close();
                }
            }

            if (databaseDetails.AfterCheckAction != null)
            {
                result = databaseDetails.AfterCheckAction.Invoke(databaseDetails, result);
            }

            return result;
        }
    }
}
