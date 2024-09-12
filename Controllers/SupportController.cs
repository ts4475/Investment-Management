using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Security.Policy;

namespace Investment1.Controllers
{
    [Route("Support/[action]")]
    public class SupportController : Controller
    {
        private readonly InvestmentDbContext _context;
        private readonly IEmailService _emailService;

        // Constructor to initialize context and email service
        public SupportController(InvestmentDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /Support/Create
        // Displays the form for creating a new support request
        [HttpGet]
        public IActionResult Create()
        {
            // Check if the user is logged in by looking for UserId in the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to the user registration page if the user is not logged in
                return RedirectToAction("BasicUser", "UserRegistration");
            }

            // Initialize a new instance of SupportViewModel for the view
            var model = new SupportViewModel();
            return View(model);
        }

        // POST: /Support/Create
        // Handles the form submission for creating a new support request
        [HttpPost]
        public async Task<IActionResult> Create(SupportViewModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // If not valid, return the view with the current model to display validation errors
                return View(model);
            }

            // Check if the user is logged in by looking for UserId in the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Return Unauthorized status if the user is not logged in
                return Unauthorized();
            }

            // Create a new Support object with the data from the model
            var support = new Support
            {
                UserId = userId.Value,
                Subject = model.Subject,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow
            };

            // Add the new support request to the database
            _context.Supports.Add(support);
            await _context.SaveChangesAsync();

            // Fetch the user's email address for notification
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                // Prepare the email subject and message
                var subject = "Support Request Raised";
                var message = $"Dear {user.Name},<br><br>Your support request with subject '{model.Subject}' has been successfully raised.<br><br>Thank you,<br>Support Team";

                // Use EmailService to send the email notification to the user
                await _emailService.SendEmailAsync(user.Email, subject, message);
            }

            // Set a success message and redirect to a success view
            ViewBag.Message = "Support ticket created successfully.";
            return View("Success");
        }

        // GET: /Support/Index
        // Displays a list of support requests for the logged-in user
        [HttpGet]
        public IActionResult Index()
        {
            // Check if the user is logged in by looking for UserId in the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to the user registration page if the user is not logged in
                return RedirectToAction("BasicUser", "UserRegistration");
            }

            // Retrieve and display support requests for the logged-in user
            var supports = _context.Supports
                .Include(s => s.User)
                .Where(s => s.UserId == userId.Value)
                .ToList();

            // Pass any success message to the view via ViewBag
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(supports);
        }
    }
}
