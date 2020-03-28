using System.ComponentModel.DataAnnotations;

namespace WebChatBuilderModels.Models
{
    public class Utilization
    {
        [Key]
        public int UtilizationId { get; set; }

        [Required]
        public virtual MediaType MediaType { get; set; }

        [Required]
        public int MaxAssignable { get; set; }

        [Required]
        public int UtilizationPercent { get; set; }
    }
}
