using System.ComponentModel.DataAnnotations;

namespace Investment1.Models
{
    public class UserProfileViewModel
    {
        // Unique identifier for the user, used for updating user details
        public int Id { get; set; }

        // Full name of the user
        [Required]
        // Ensures that the Name field is mandatory and cannot be left empty.
        public string Name { get; set; }

        // Mobile phone number of the user
        [Required]
        [DataType(DataType.PhoneNumber)]
        // Ensures that the Mobile field is mandatory and cannot be left empty.
        // [DataType(DataType.PhoneNumber)] specifies that this field is expected to be a phone number.
        public string Mobile { get; set; }

        // Residential address of the user
        [Required]
        // Ensures that the Address field is mandatory and cannot be left empty.
        public string Address { get; set; }

        // Postal Index Number (PIN) or ZIP code for the user's address
        [Required]
        [StringLength(10, MinimumLength = 6)]
        // Ensures that the PIN field is mandatory and must be between 6 and 10 characters long.
        // This provides a range to accommodate different PIN or ZIP code formats.
        public string PIN { get; set; }

        // Email address of the user
        // This property stores the user's email address.
        public string Email { get; set; }

        // Bank account number of the user
        // This property stores the user's bank account number.
        public string AccountNo { get; set; }

        // IFSC code of the user's bank branch
        // This property stores the IFSC code for the bank branch where the user holds an account.
        public string IFSCCode { get; set; }

        // Permanent Account Number (PAN) for tax identification
        // This property stores the user's PAN.
        public string PAN { get; set; }

        // Aadhaar number for unique identification in India
        // This property stores the user's Aadhaar number.
        public string Aadhaar { get; set; }
    }
}
