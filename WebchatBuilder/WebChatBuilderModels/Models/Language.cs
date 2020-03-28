using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class Language
    {
        [Key]
        public int LanguageId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }
    }
}