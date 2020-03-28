using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebchatBuilder.DataModels
{
    public class VisitorMessageChat
    {
        public long VisitorMessageId { get; set; }

        public string ParticipantId { get; set; }

        public VisitorMessageState State { get; set; }

        public string Message { get; set; }
    }

    public enum VisitorMessageState
    {
        Connected,
        InConfirmation,
        InVoicemail,
        VoicemailSent
    }
}