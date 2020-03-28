using System.ComponentModel.DataAnnotations;

namespace WebchatBuilder.ViewModels
{
    public class NewUserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}