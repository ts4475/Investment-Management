namespace Investment1.ViewModels
{
    public class PersonalDetailsViewModel
    {
        // Unique identifier for the user
        // This property is used to uniquely identify the user in the system.
        public int Id { get; set; }

        // Full name of the user
        // This property stores the user's name as provided in the form.
        public string Name { get; set; }

        // Residential address of the user
        // This property stores the user's address, which might be used for correspondence or verification purposes.
        public string Address { get; set; }

        // Mobile phone number of the user
        // This property stores the user's mobile number for contact and verification purposes.
        public string Mobile { get; set; }

        // Postal Index Number (PIN) or ZIP code for the user's address
        // This property stores the PIN or ZIP code for the user's address, helping to identify the user's location.
        public string PIN { get; set; }

        // Date of Birth of the user
        // This property stores the user's date of birth, which might be used for age verification or profile purposes.
        public DateTime DOB { get; set; }
    }
}
