using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using System.Threading;
using DejaVu.SelfHealthCheck.Messages;
using System.Messaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using DejaVu.SelfHealthCheck.Utility;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.Engine
{
    public class Runner : IRunner, IDisposable
    {
        private IConfiguration _SelfHealthCheckConfiguration;
        private Timer _checkIntervalTimer;

        private Dictionary<ICheckConfiguration, Timer> _checkTimers;
        private Timer heartBeatTimer;

        private readonly static Lazy<HttpClient> Client = new Lazy<HttpClient>(() =>
        {
              var healthCheckUrl = System.Configuration.ConfigurationManager.AppSettings["SelfHealthCheckUrl"];
            //var healthCheckUrl = System.Configuration.ConfigurationManager.AppSettings["SignalrUrl"];

            var _client = new HttpClient();
            _client.BaseAddress = new Uri(healthCheckUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _client;
        });
        public static HttpClient client { get { return Client.Value; } }


        private Runner(IConfiguration selfHealthCheckConfiguration)
        {
            this._SelfHealthCheckConfiguration = selfHealthCheckConfiguration;
        }

        public static IRunner Initialize(IConfiguration selfHealthCheckConfiguration)
        {
            var runner = new Runner(selfHealthCheckConfiguration);
            runner.InitMulti();
            return runner;
        }

        public void Init()
        {
            _checkIntervalTimer = new Timer(new TimerCallback(RunChecks), null, TimeSpan.FromMilliseconds(this._SelfHealthCheckConfiguration.CheckInterval), TimeSpan.FromMilliseconds(-1));
        }

        public void InitMulti()
        {
            _checkTimers = new Dictionary<ICheckConfiguration, Timer>();
            heartBeatTimer = new Timer(new TimerCallback(SendContinualHealthBeat), null, 0, Timeout.Infinite);
            foreach (var check in this._SelfHealthCheckConfiguration.Checks)
            {
                _checkTimers.Add(check, new Timer(new TimerCallback(RunChecksMulti), check, 0, Timeout.Infinite));
            }

        }

        private void RunChecks(object state)
        {
            List<ICheckResult> checkResults = new List<ICheckResult>();

            try
            {
                var checks = this._SelfHealthCheckConfiguration.Checks.AsParallel();

                //Run all checks
                checks.ForAll(check =>
                    {
                        var checkRunnerProxy = (CheckRunnerAttribute)Attribute.GetCustomAttribute(check.GetType(), typeof(CheckRunnerAttribute));
                        if (checkRunnerProxy != null)
                        {
                            var checkRunner = (ICheckRunner)Activator.CreateInstance(checkRunnerProxy.CheckRunnerType);
                            if (checkRunner != null)
                            {
                                try
                                {
                                    checkResults.Add(checkRunner.Check(check));
                                }
                                catch (Exception ex)
                                {
                                    //TODO: Log Exception and return failed result
                                    checkResults.Add(new CheckResult()
                                    {
                                        Title = check.Title,
                                        Status = CheckResultStatus.Unknown,
                                        AdditionalInformation = string.Format("Error: {0}", ex.Message)
                                    });
                                }
                            }
                            else
                            {
                                //TODO: Log Check Runner specified not valid
                                checkResults.Add(new CheckResult()
                                {
                                    Title = check.Title,
                                    Status = CheckResultStatus.Unknown,
                                    AdditionalInformation = "Check Runner specified is not valid"
                                });
                            }
                        }
                        else
                        {
                            //TODO: Log No Check Runner Attribute found
                            checkResults.Add(new CheckResult()
                            {
                                Title = check.Title,
                                Status = CheckResultStatus.Unknown,
                                AdditionalInformation = "Check Runner Attribute not found"
                            });
                        }
                    });


                //Send Results to the BUS
                Global.Bus.Send<SelfHealthMessage>(x =>
                    {
                        x.DateChecked = DateTime.Now;
                        x.AppID = _SelfHealthCheckConfiguration.AppID;
                        x.Results = checkResults;

                        //TODO: Compute overall status based on check results
                    });
            }
            finally
            {
                if (_checkIntervalTimer != null)
                {
                    _checkIntervalTimer.Change(TimeSpan.FromMilliseconds(this._SelfHealthCheckConfiguration.CheckInterval), TimeSpan.FromMilliseconds(-1));
                }
            }
        }


        private void RunChecksMulti(object state)
        {
            var check = state as ICheckConfiguration;
            try
            {
                //Run all checks   
                List<ICheckResult> checkResults = new List<ICheckResult>();
                var checkRunnerProxy = (CheckRunnerAttribute)Attribute.GetCustomAttribute(check.GetType(), typeof(CheckRunnerAttribute));
                if (checkRunnerProxy != null)
                {
                    var checkRunner = (ICheckRunner)Activator.CreateInstance(checkRunnerProxy.CheckRunnerType);
                    if (checkRunner != null)
                    {
                        try
                        {
                            checkResults.Add(checkRunner.Check(check));
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation("Exception Occured while running check" + ex.Message + ex.StackTrace);
                            //TODO: Log Exception and return failed result
                            checkResults.Add(new CheckResult()
                            {
                                Title = check.Title,
                                Status = CheckResultStatus.Unknown,
                                AdditionalInformation = string.Format("Error: {0}", ex.Message)
                            });
                        }
                    }
                    else
                    {
                        //TODO: Log Check Runner specified not valid
                        checkResults.Add(new CheckResult()
                        {
                            Title = check.Title,
                            Status = CheckResultStatus.Unknown,
                            AdditionalInformation = "Check Runner specified is not valid"
                        });
                    }
                }
                else
                {
                    //TODO: Log No Check Runner Attribute found
                    checkResults.Add(new CheckResult()
                    {
                        Title = check.Title,
                        Status = CheckResultStatus.Unknown,
                        AdditionalInformation = "Check Runner Attribute not found"
                    });
                }


                //Send Results to the BUS
                //Global.Bus.Send("DejaVu.SelfHealthCheck", new SelfHealthMessage()
                //{
                //    DateChecked = DateTime.Now,
                //    AppID = _SelfHealthCheckConfiguration.AppID,
                //    Results = checkResults,
                //    OverallStatus = checkResults[0].Status
                //    //TODO: Compute overall status based on Check results
                //});
                DateTime currentTime = DateTime.Now;
                double val = _SelfHealthCheckConfiguration.CheckInterval / 1000;
                    this._SelfHealthCheckConfiguration.NextCheckTime = DateTime.Now.AddSeconds(_SelfHealthCheckConfiguration.CheckInterval/1000);
                var msg = new SelfHealthMessage()
                {
                    DateChecked = DateTime.Now,
                    AppID = _SelfHealthCheckConfiguration.AppID,
                    Results = checkResults,
                    OverallStatus = checkResults[0].Status,
                    Title = checkResults[0].Title,
                    AdditionalInformation = checkResults[0].AdditionalInformation,
                    TimeElapsed = checkResults[0].TimeElasped,
                    IPAddress = _SelfHealthCheckConfiguration.IPAddress,
                    NextCheckTime = _SelfHealthCheckConfiguration.NextCheckTime,

                    //TODO: Compute overall status based on Check results
                };
                SendMessage(msg);

            }
            finally
            {
                if (_checkTimers.ContainsKey(check) && _checkTimers[check] != null)
                {
                    _checkTimers[check].Change((long)this._SelfHealthCheckConfiguration.CheckInterval, Timeout.Infinite);
                }
            }
        }

        private Func<ICheckConfiguration, ICheckResult, ICheckResult> AfterCheckActionChaged()
        {
            throw new NotImplementedException();
        }

        private void SendContinualHealthBeat(object state)
        {

            var msg = new SelfHealthMessage()
            {
                DateChecked = DateTime.Now,
                AppID = _SelfHealthCheckConfiguration.AppID,
                IPAddress = _SelfHealthCheckConfiguration.IPAddress,
                Results = null,
                OverallStatus = CheckResultStatus.HealthBeat
            };
            SendMessage(msg);
            heartBeatTimer.Change(5000, Timeout.Infinite);
        }

        public void Stop()
        {
            if (_checkIntervalTimer != null)
            {
                _checkIntervalTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _checkIntervalTimer.Dispose();
                _checkIntervalTimer = null;
            }

            foreach (var checkTimer in _checkTimers.Values)
            {
                checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
                checkTimer.Dispose();
            }
            _checkTimers = null;
        }

        public void Dispose()
        {
            Stop();
        }


        void SendMessage(SelfHealthMessage msg)
        {
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync<SelfHealthMessage>("SelfHealthCheck/api/HealthMessage/ReceiveMessage", msg).Result;
                if (!response.IsSuccessStatusCode)
                {
                    new LogWriter(string.Format("Error while posting- {0}", response.Content.ReadAsStringAsync().Result));
                }
            }
            catch (Exception ex)
            {
                new LogWriter(string.Format("Error Message- {0}: Stack Trace- {1} :Inner Exception- {2}", ex.Message, ex.StackTrace, ex.InnerException == null ? "" : ex.InnerException.Message));
            }
        }
    }
}
