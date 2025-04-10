using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class ResetPasswordViewModel
    {

        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }  // Remove Required attribute

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password does not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
