


namespace DejaVu.SelfHealthCheck.Collector
{
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Persistence;
    using System;
    
	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    public class EndpointConfig : Feature, IConfigureThisEndpoint, AsA_Server
    {
        public EndpointConfig()
        {
            EnableByDefault();
            RegisterStartupTask<StartStopTask>();
        }
        protected override void Setup(FeatureConfigurationContext context)
        {
            
        }
       
        public void Customize(BusConfiguration configuration)
        {
            configuration.UseSerialization<XmlSerializer>();
            configuration.UsePersistence<RavenDBPersistence>();
            configuration.Transactions().Enable();
            string ServiceEndpoint = String.Empty;
            configuration.EndpointName("DejaVu.SelfHealthCheck");
            configuration.DisableFeature<TimeoutManager>();
            configuration.EnableFeature<InMemoryTimeoutPersistence>();
            configuration.DisableFeature<CriticalTimeMonitoring>();
            configuration.EnableInstallers();
            configuration.Transactions().DisableDistributedTransactions().DoNotWrapHandlersExecutionInATransactionScope();
        }
    }
}