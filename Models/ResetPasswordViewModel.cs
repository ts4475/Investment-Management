using System.ComponentModel.DataAnnotations;

namespace Investment1.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "OTP is required")]
        public string Otp { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
