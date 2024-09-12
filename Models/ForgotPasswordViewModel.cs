using System.ComponentModel.DataAnnotations;

namespace Investment1.Models
{
    public class ForgotPasswordViewModel
    {
        // Email address of the user requesting a password reset
        [Required(ErrorMessage = "Email is required")]
        // Ensures that the Email field is not left empty.
        // The error message "Email is required" will be displayed if the field is empty.
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        // Validates that the provided email address follows the correct email format.
        // The error message "Invalid Email Address" will be displayed if the email format is incorrect.
        public string Email { get; set; }
    }
}
