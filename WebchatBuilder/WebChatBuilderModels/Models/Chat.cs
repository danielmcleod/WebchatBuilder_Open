using System;
using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class Chat
    {
        [Key]
        public long ChatId { get; set; }

        [Required]
        public string SessionId { get; set; }

        public string ConnectionId { get; set; }

        public string ChatIdentifier { get; set; }

        public string ParticipantId { get; set; }

        public string InteractionId { get; set; }

        public string Profile { get; set; }

        public string QueueName { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateAnswered { get; set; }

        public DateTime? DateEnded { get; set; }

        public virtual UserData UserData { get; set; }
    }
}
