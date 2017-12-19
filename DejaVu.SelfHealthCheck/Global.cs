using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using NServiceBus.Features;

namespace DejaVu.SelfHealthCheck
{
    public class Global
    {
        public static IBus Bus { get; internal set; }

        public static void Init()
        {

            BusConfiguration configuration = new BusConfiguration();
            configuration.EndpointName("SelfHealthCheck");
            configuration.UseTransport<MsmqTransport>();
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.Transactions().Enable();
            configuration.UseSerialization<XmlSerializer>();
            configuration.DisableFeature<TimeoutManager>();
            configuration.EnableFeature<InMemoryTimeoutPersistence>();
            configuration.EnableInstallers();


            Bus = NServiceBus.Bus.Create(configuration);
            
        }
    }
}
