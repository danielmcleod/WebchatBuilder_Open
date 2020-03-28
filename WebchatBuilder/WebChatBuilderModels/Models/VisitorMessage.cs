using System;
using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class VisitorMessage
    {
        [Key]
        public long VisitorMessageId { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsProcessed { get; set; }

        public string Message { get; set; }

        public string Notes { get; set; }

        public string Workgroup { get; set; }

        public virtual VisitorMessageType Type { get; set; }

        public string Skills { get; set; }

        public string PhoneNumber { get; set; }

        public string CustomInfo { get; set; }

        public string AttributeNames { get; set; }

        public string AttributeValues { get; set; }

        public DateTime? RequestedTime { get; set; }
    }

    public enum VisitorMessageType
    {
        Voicemail,
        Callback
    }
}
