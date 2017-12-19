using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Messages;
using NServiceBus;

namespace DejaVu.SelfHealthCheck.Events
{
    public class IMessageReceivedEvent : IEvent
    {
        public SelfHealthMessage Message { get; set; }
    }
}
