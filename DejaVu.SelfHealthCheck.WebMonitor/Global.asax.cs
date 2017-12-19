using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB;
using DejaVu.SelfHealthCheck.WebMonitor.Workers;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Connection;
using NServiceBus;

namespace DejaVu.SelfHealthCheck.WebMonitor
{
    public class Global : System.Web.HttpApplication
    {
        public static IBus Bus { get; set; }
        private static IDocumentStore store;
        public static IDocumentStore Store
        {
            get
            {
                if (store == null) throw new InvalidOperationException("::IDocumentStore has not been initialized::");
                return store;
            }
            set
            {
                store = value;
            }
        }

        public static void InitializeStore()
        {
            //store = new EmbeddableDocumentStore { ConnectionStringName = "TreeStore", UseEmbeddedHttpServer = true,  };
            store = new EmbeddableDocumentStore { RunInMemory = true };
            store.Initialize();
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            //InitializeStore();
            RavenStore.InitializeStore();
            Store = RavenStore.Store;
            Bus = Configure.With()
                  .DefineEndpointName(() => "SelfHealthCheck.WebMonitor")
                  .DefaultBuilder()
                  .UseTransport<Msmq>()
                      .PurgeOnStartup(false)
                  .UnicastBus()
                      .RunHandlersUnderIncomingPrincipal(false)
                  .MsmqSubscriptionStorage()
                  .CreateBus()
                  .Start(() => Configure.Instance.ForInstallationOn<NServiceBus.Installation.Environments.Windows>().Install());
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}