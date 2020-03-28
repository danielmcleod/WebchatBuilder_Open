using System.ComponentModel.DataAnnotations;

namespace WebchatBuilder.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Title { get; set; }

        public string ImgPath { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool IsActive { get; set; }
        
    }
}