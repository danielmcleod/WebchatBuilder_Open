using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
