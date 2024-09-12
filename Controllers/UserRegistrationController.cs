using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using Investment1.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace Investment1.Controllers
{
    public class UserRegistrationController : Controller
    {
        private readonly InvestmentDbContext _context;

        public UserRegistrationController(InvestmentDbContext context)
        {
            _context = context;
        }

        // Display the basic user registration form
        [HttpGet("userregistration/basicuser")]
        public IActionResult BasicUser()
        {
            return View();
        }

        // Handle submission of the basic user registration form
        [HttpPost("userregistration/basicuser")]
        public IActionResult BasicUser(BasicUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Store the basic user information in the session
                HttpContext.Session.SetString("BasicUserEmail", model.Email);
                HttpContext.Session.SetString("BasicUserPassword", model.Password);

                // Redirect to the personal details form
                return RedirectToAction("PersonalDetails");
            }

            // If the model is not valid, return the view with validation errors
            return View(model);
        }

        // Display the personal details form
        [HttpGet("userregistration/personaldetails")]
        public IActionResult PersonalDetails()
        {
            var email = HttpContext.Session.GetString("BasicUserEmail");
            // Check if the basic user email exists in the session
            if (string.IsNullOrEmpty(email))
            {
                // If not, redirect to the basic user registration form
                return RedirectToAction("BasicUser");
            }

            // Return the personal details view with an empty model
            return View(new PersonalDetailsViewModel());
        }

        // Handle submission of the personal details form
        [HttpPost("userregistration/personaldetails")]
        public IActionResult PersonalDetails(PersonalDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Serialize the personal details and store it in the session
                var personalDetailsJson = JsonSerializer.Serialize(model);
                HttpContext.Session.SetString("PersonalDetails", personalDetailsJson);

                // Redirect to the bank details form
                return RedirectToAction("BankDetails");
            }

            // If the model is not valid, return the view with validation errors
            return View(model);
        }

        // Display the bank details form
        [HttpGet("userregistration/bankdetails")]
        public IActionResult BankDetails()
        {
            // Retrieve personal details from the session
            var personalDetailsJson = HttpContext.Session.GetString("PersonalDetails");
            if (string.IsNullOrEmpty(personalDetailsJson))
            {
                // If personal details are not in the session, redirect to the personal details form
                return RedirectToAction("PersonalDetails");
            }

            // Deserialize personal details and pass to the view
            var personalDetails = JsonSerializer.Deserialize<PersonalDetailsViewModel>(personalDetailsJson);
            if (personalDetails == null)
            {
                // If deserialization fails, redirect to the personal details form
                return RedirectToAction("PersonalDetails");
            }

            // Initialize the bank details view model with the ID from personal details
            return View(new BankDetailsViewModel { Id = personalDetails.Id });
        }

        // Handle submission of the bank details form
        [HttpPost("userregistration/bankdetails")]
        public async Task<IActionResult> BankDetails(BankDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve personal details from the session
                var personalDetailsJson = HttpContext.Session.GetString("PersonalDetails");
                if (string.IsNullOrEmpty(personalDetailsJson))
                {
                    // If personal details are not in the session, redirect to the personal details form
                    return RedirectToAction("PersonalDetails");
                }

                var personalDetails = JsonSerializer.Deserialize<PersonalDetailsViewModel>(personalDetailsJson);
                if (personalDetails == null)
                {
                    // If deserialization fails, redirect to the personal details form
                    return RedirectToAction("PersonalDetails");
                }

                // Hash the password for security
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(HttpContext.Session.GetString("BasicUserPassword"));

                // Create a new user object with the collected data
                var user = new User
                {
                    Name = personalDetails.Name,
                    Password = hashedPassword,
                    DOB = personalDetails.DOB,
                    Mobile = personalDetails.Mobile,
                    Email = HttpContext.Session.GetString("BasicUserEmail"),
                    Address = personalDetails.Address,
                    PIN = personalDetails.PIN,
                    AccountNo = model.AccountNo,
                    IFSCCode = model.IFSCCode,
                    PAN = model.PAN,
                    Aadhaar = model.Aadhaar,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Add the user to the database
                _context.Users.Add(user);

                try
                {
                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Return a server error if saving fails
                    return StatusCode(500, "Internal server error while saving user data.");
                }

                // Clear session data for security
                HttpContext.Session.Remove("BasicUserEmail");
                HttpContext.Session.Remove("PersonalDetails");

                // Redirect to the login page after successful registration
                return RedirectToAction("Login", "UserAuthentication");
            }

            // If the model is not valid, return the view with validation errors
            return View(model);
        }

        // Display the registration confirmation page
        [HttpGet("userregistration/confirmation")]
        public IActionResult Confirmation()
        {
            // Return the confirmation view
            return View();
        }
    }
}
