using System.ComponentModel.DataAnnotations;

namespace navision.api.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Your password must contain between 6 and 24 characters")]
        public string Password { get; set; } = string.Empty;

    }
}