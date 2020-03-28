using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebchatBuilder.Helpers
{
    public class HubPipelineHelper : HubPipelineModule
    {
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            return base.OnBeforeIncoming(context);
        }
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            return base.OnBeforeOutgoing(context);
        }
    }
}