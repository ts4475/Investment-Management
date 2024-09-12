using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using Investment1.ViewModels;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Investment1.Controllers
{
    [Route("[controller]/[action]")]
    public class UserAuthenticationController : Controller
    {
        private readonly InvestmentDbContext _context;
        private readonly IEmailService _emailService;

        public UserAuthenticationController(InvestmentDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Display login page
        [HttpGet]
        public IActionResult Login()
        {
            // Redirect to home if user is already logged in
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Return login view with an empty model
            return View(new LoginViewModel());
        }

        // Handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return view with validation errors if the model is not valid
                return View(model);
            }

            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                // Add error if user is not found or password is incorrect
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Set session values on successful login
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);

            // Redirect to home page
            return RedirectToAction("Index", "Home");
        }

        // Display forgot password page
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            // Return view for forgotten password request
            return View();
        }

        // Handle forgot password form submission
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Check if user exists by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // Add error if email is not found
                ModelState.AddModelError("", "Email not found.");
                return View();
            }

            // Generate OTP (One Time Password)
            string otp = new Random().Next(100000, 999999).ToString(); // Generate a 6-digit OTP
            string subject = "Your OTP Code";
            string message = $"Your OTP code is: {otp}";

            // Send OTP via email
            await _emailService.SendEmailAsync(email, subject, message);

            // Store OTP and email temporarily
            TempData["Email"] = email;
            TempData["Otp"] = otp;
            TempData["SuccessMessage"] = "OTP has been sent successfully.";

            // Redirect to reset password page
            return RedirectToAction("ResetPassword");
        }

        // Display reset password page
        [HttpGet]
        public IActionResult ResetPassword()
        {
            // Return view for resetting password
            return View();
        }

        // Handle reset password form submission
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string otp, string newPassword, string confirmPassword)
        {
            // Retrieve OTP and email from TempData
            var storedOtp = TempData["Otp"]?.ToString();
            var email = TempData["Email"]?.ToString();

            // Validate OTP
            if (storedOtp != otp)
            {
                ModelState.AddModelError("", "Invalid OTP.");
                return View();
            }

            // Validate password confirmation
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email not found.");
                return View();
            }

            // Hash and update the new password
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            // Notify success and redirect to login
            TempData["SuccessMessage"] = "Password updated successfully.";
            return RedirectToAction("Login");
        }

        // Handle logout
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            // Redirect to login page
            return RedirectToAction("Login");
        }

        // Display profile page
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Get user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to login if user ID is not found in session
                return RedirectToAction("Login");
            }

            // Find user by ID
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                // Return 404 if user is not found
                return NotFound();
            }

            // Prepare view model with user details
            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Mobile = user.Mobile,
                Address = user.Address,
                PIN = user.PIN,
                Email = user.Email,
                AccountNo = user.AccountNo,
                IFSCCode = user.IFSCCode,
                PAN = user.PAN,
                Aadhaar = user.Aadhaar
            };

            // Return profile view with user details
            return View(model);
        }

        // Handle profile update form submission
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return view with validation errors if model is not valid
                return View(model);
            }

            // Get user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to login if user ID is not found in session
                return RedirectToAction("Login");
            }

            // Find user by ID
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                // Return 404 if user is not found
                return NotFound();
            }

            // Update only the editable fields
            user.Name = model.Name;
            user.Mobile = model.Mobile;
            user.Address = model.Address;
            user.PIN = model.PIN;
            user.UpdatedAt = DateTime.Now;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Notify success and redirect to profile page
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }
    }
}
