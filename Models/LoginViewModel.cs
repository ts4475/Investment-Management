namespace Investment1.Models
{
    public class LoginViewModel
    {
        // Email address of the user attempting to log in
        // This property is used to capture the email address from the login form.
        public string Email { get; set; }

        // Password of the user attempting to log in
        // This property is used to capture the password from the login form.
        public string Password { get; set; }
    }
}
