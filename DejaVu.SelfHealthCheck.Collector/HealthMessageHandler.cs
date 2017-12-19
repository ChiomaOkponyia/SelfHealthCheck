using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using DejaVu.SelfHealthCheck.Messages;
using DejaVu.SelfHealthCheck.Events;
using Raven.Client;
using Newtonsoft.Json;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.Collector
{
    public class HealthMessageHandler : IHandleMessages<SelfHealthMessage>
    {
        public IBus Bus { get; set; }

        public HealthMessageHandler(IBus bus)
        {
            this.Bus = bus;
        }
        public void Handle(SelfHealthMessage message)
        {
            Console.WriteLine();
            Console.WriteLine("New message receieved");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("About to save to DataBase.");
            var connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            HealthMessageDAO messageDao = new HealthMessageDAO(connectionString);
            try
            {
                if (message.OverallStatus == CheckResultStatus.HealthBeat)
                {
                    Console.WriteLine("It's an health Beat");
                    HealthMessageUtility.SendHealthBeat(message);
                }
                else
                {
                    messageDao.Save(message);
                    HealthMessageUtility.SendHealthMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message- {0}: Stack Trace- {1} :Inner Exception- {2}", ex.Message, ex.StackTrace, ex.InnerException == null ? "" : ex.InnerException.Message);
                //Bus.Send("DejaVu.SelfHealthCheck.Retries", message);
            }
            Console.WriteLine();
            Console.WriteLine("Done saving.");
        }


    }
}
