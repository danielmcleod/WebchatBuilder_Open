using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebchatBuilder.DataModels
{
    public class ChatRequest
    {
        public string SupportedContentTypes { get; set; }

        public Participant Participant { get; set; }

        public bool TranscriptRequired { get; set; }

        public string EmailAddress { get; set; }

        public string Target { get; set; }

        public string TargetType { get; set; }

        public string Language { get; set; }

        public string CustomInfo { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public ICollection<RoutingContext> RoutingContexts { get; set; }
    }

    public class Participant
    {
        public string Name { get; set; }
        public string Credentials { get; set; }
    }

    public class RoutingContext
    {
        public string Context { get; set; }
        public string Category { get; set; }
    }
}