using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class UserData
    {
        [Key]
        public long UserDataId { get; set; }

        public string FromUrl { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string CustomData { get; set; }
    }
}