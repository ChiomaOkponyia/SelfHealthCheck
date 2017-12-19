using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using System.Net;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.Engine
{
    public class CustomCheckRunner : ICheckRunner
    {
        public ICheckResult Check(ICheckConfiguration checkDetails)
        {
            var customDetails = checkDetails as ICustomConfiguration;

            ICheckResult result = new CheckResult()
            {
                Title = customDetails.Title
            };
            Stopwatch timer = new Stopwatch();
            try
            {
                timer.Start();
                KeyValuePair<bool,string> response = customDetails.TheCustomCheck.Invoke();
                timer.Stop();

                result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);

                if (response.Key)
                {
                    result.Status = customDetails.ResponseTime > result.TimeElasped ? CheckResultStatus.Up : CheckResultStatus.PerfomanceDegraded;
                    result.AdditionalInformation = string.Format("Sucessfull: {0}", response.Value);
                }
                else
                {
                    result.AdditionalInformation = string.Format("Error: {0}", response.Value);
                    result.Status = CheckResultStatus.Down;
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
                result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);
                result.AdditionalInformation = string.Format("Error: {0}", ex.Message);
                result.Status = CheckResultStatus.Down;
            }

            if (customDetails.AfterCheckAction != null)
            {
                result = customDetails.AfterCheckAction.Invoke(customDetails, result);
            }

            return result;
        }
    }
}
