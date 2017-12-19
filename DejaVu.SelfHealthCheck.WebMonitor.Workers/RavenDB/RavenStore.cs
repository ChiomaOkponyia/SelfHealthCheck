using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Connection;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB
{
    public class RavenStore
    {
        private static IDocumentStore store;
        public static IDocumentStore Store
        {
            get
            {
                //if (store == null)
                    //throw new InvalidOperationException("::IDocumentStore has not been initialized::");
                    //InitializeStore();
                return store;
            }
        }

        public static void InitializeStore()
        {
            store = new EmbeddableDocumentStore { ConnectionStringName = "RavenStore" };
            //store = new EmbeddableDocumentStore { RunInMemory = true };
            store.Initialize();
        }
        public static void CustomInitializeStore()
        {
            if (store == null)
            {
                store = new EmbeddableDocumentStore { ConnectionStringName = "RavenStore" };
                //store = new EmbeddableDocumentStore { RunInMemory = true };
                store.Initialize();
            }
        }
    }
}
