using System.ComponentModel.DataAnnotations;

namespace Investment1.ViewModels
{
    public class BasicUserViewModel
    {
        // Email address of the user
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        // The Email property must be a valid email format and is required for the form submission.
        public string Email { get; set; }

        // Password for user account
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        // The Password property must be at least 6 characters long and can be up to 100 characters long.
        // This ensures that users have a sufficiently long password for security.
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
        // The RegularExpression ensures that the password contains at least one uppercase letter and one special character.
        // This pattern helps to enforce a stronger password policy.
        public string Password { get; set; }
    }
}
