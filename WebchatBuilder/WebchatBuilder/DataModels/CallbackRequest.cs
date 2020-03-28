using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebchatBuilder.DataModels
{
    public class CallbackRequest
    {
        public CallbackParticipant Participant { get; set; }

        public string Target { get; set; }

        public string TargetType { get; set; }

        public string Language { get; set; }

        public string CustomInfo { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public ICollection<CallbackRoutingContext> RoutingContexts { get; set; }

        //Max 2000 characters
        public string Subject { get; set; }
    }

    public class CallbackParticipant
    {
        public string Name { get; set; }
        public string Credentials { get; set; }
        public string Telephone { get; set; }
    }

    public class CallbackRoutingContext
    {
        public string Context { get; set; }
        public string Category { get; set; }
    }
}