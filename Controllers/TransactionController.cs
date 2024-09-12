using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using System.Threading.Tasks;

namespace Investment1.Controllers
{
    public class TransactionController : Controller
    {
        private readonly InvestmentDbContext _context;

        // Constructor to inject the InvestmentDbContext dependency
        public TransactionController(InvestmentDbContext context)
        {
            _context = context;
        }

        // GET: Transaction/Create
        // Action to display the transaction creation form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
        // Action to handle the form submission for creating a new transaction
        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the view with the current model
                return View(model);
            }

            // Check if the user exists in the database
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                // Add an error to the model state if the user does not exist
                ModelState.AddModelError("", "User does not exist.");
                // Return the view with the current model and error message
                return View(model);
            }

            // Create a new transaction object
            var transaction = new Transaction
            {
                UserId = model.UserId,
                SipAmount = model.Amount,
                // Other properties can be set here
            };

            // Add the transaction to the context
            _context.Transactions.Add(transaction);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Index action after successful creation
            return RedirectToAction("Index");
        }
    }
}
