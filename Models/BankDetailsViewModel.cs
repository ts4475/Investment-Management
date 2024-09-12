using System.ComponentModel.DataAnnotations;

namespace Investment1.ViewModels
{
    public class BankDetailsViewModel
    {
        // Unique identifier for the user, typically used for identifying the user in the system.
        public int Id { get; set; }

        // Account number field
        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, ErrorMessage = "Account Number cannot exceed 20 characters.")]
        // The Account Number must be a string with a maximum length of 20 characters.
        public string AccountNo { get; set; }

        // IFSC code field for identifying the bank branch
        [Required(ErrorMessage = "IFSC Code is required.")]
        [StringLength(11, ErrorMessage = "IFSC Code must be 11 characters long.")]
        // The IFSC Code must be exactly 11 characters long and follow a specific format.
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC Code format.")]
        public string IFSCCode { get; set; }

        // PAN (Permanent Account Number) field for tax identification
        [StringLength(10, ErrorMessage = "PAN cannot exceed 10 characters.")]
        // The PAN must be a string of 10 characters and follow a specific format.
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format.")]
        public string PAN { get; set; }

        // Aadhaar number field for unique identification in India
        [StringLength(16, ErrorMessage = "Aadhaar cannot exceed 16 characters.")]
        // The Aadhaar number must be a string of exactly 16 digits.
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid Aadhaar format.")]
        public string Aadhaar { get; set; }
    }
}
