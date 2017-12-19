using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using System.Net;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.Engine
{
    public class UrlCheckRunner : ICheckRunner
    {
        public ICheckResult Check(ICheckConfiguration checkDetails)
        {
            var urlDetails = checkDetails as IUrlConfiguration;
           

            ICheckResult result = new CheckResult()
                {
                    Title = urlDetails.Title
                };

            Stopwatch timer = new Stopwatch();
            
           

            try
            {
                //Open url
                HttpWebRequest request = HttpWebRequest.Create(urlDetails.Url) as HttpWebRequest;

                timer.Start();
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                timer.Stop();

                result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result.Status = urlDetails.ResponseTime > result.TimeElasped ? CheckResultStatus.Up : CheckResultStatus.PerfomanceDegraded;
                }
                else
                {
                    result.AdditionalInformation = string.Format("{0} - {1}", response.StatusCode, response.StatusDescription);
                    result.Status = CheckResultStatus.Down;
                }
            }
            catch (WebException ex)
            {
                Trace.TraceInformation("Error Occured In URL check " + ex.Message + ex.StackTrace);
                timer.Stop();
                result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    result.AdditionalInformation = string.Format("{0} - {1}", response.StatusCode, response.StatusDescription);
                }
                else
                {
                    result.AdditionalInformation = string.Format("Error: {0} - {1}", ex.Message, ex.Status);
                }
                result.Status = CheckResultStatus.Down;
            }

            if (urlDetails.AfterCheckAction != null)
            {
                result = urlDetails.AfterCheckAction.Invoke(urlDetails, result);
            }
           // Trace.TraceInformation("Return result " +result);


            return result;
        }
    }
}
