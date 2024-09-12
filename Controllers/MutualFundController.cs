using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using Investment1.ViewModels;

namespace Investment1.Controllers
{
    public class MutualFundController : Controller
    {
        private readonly InvestmentDbContext _context;
        private readonly IEmailService _emailService;

        // Constructor to inject DbContext and EmailService dependencies
        public MutualFundController(InvestmentDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /MutualFund/SelectInvestmentType
        [HttpGet("MutualFund/SelectInvestmentType")]
        public IActionResult SelectInvestmentType(int mfId)
        {
            var mutualFund = _context.MutualFunds.Find(mfId);
            if (mutualFund == null)
            {
                return NotFound();
            }

            return View(mutualFund);
        }

        // GET: /MutualFund/Index
        public async Task<IActionResult> Index(FundCategory? category)
        {
            // Retrieve mutual funds based on the specified category or all funds if category is not specified
            var funds = category.HasValue
                ? await _context.MutualFunds.Where(mf => mf.Category == category.Value).ToListAsync()
                : await _context.MutualFunds.ToListAsync();

            return View(funds);
        }

        // GET: /MutualFund/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Retrieve the mutual fund details by ID
            var mutualFund = await _context.MutualFunds.FindAsync(id);
            if (mutualFund == null)
            {
                return NotFound();
            }

            // Retrieve the ROI details associated with the mutual fund
            var roi = await _context.ROIs.Where(r => r.MfId == id).FirstOrDefaultAsync();

            // Create and populate the view model for mutual fund details
            var model = new MutualFundDetailsViewModel
            {
                MutualFund = mutualFund,
                ROI = new ROIViewModel
                {
                    ROI1M = roi?.ROI_1M ?? 0m,
                    ROI6M = roi?.ROI_6M ?? 0m,
                    ROI1Y = roi?.ROI_1Y ?? 0m,
                    ROI3Y = roi?.ROI_3Y ?? 0m,
                    ROI5Y = roi?.ROI_5Y ?? 0m
                },
                // MinimumSIPAmount = mutualFund.MinSIP // Optional, uncomment if needed
            };

            return View(model);
        }

        // GET: /MutualFund/OneTimeInvestment/5
        [HttpGet("MutualFund/OneTimeInvestment/{mfId}")]
        public IActionResult OneTimeInvestment(int mfId)
        {
            var mutualFund = _context.MutualFunds.Find(mfId);
            if (mutualFund == null)
            {
                return NotFound();
            }

            var model = new InvestmentViewModel
            {
                MfId = mfId
            };

            return View(model);
        }

        // POST: /MutualFund/ProcessOneTimeInvestment
        [HttpPost("MutualFund/ProcessOneTimeInvestment")]
        public async Task<IActionResult> ProcessOneTimeInvestment(InvestmentViewModel model)
        {
            // Debugging ModelState validation
            Console.WriteLine("Debug: Checking ModelState.IsValid");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Debug: ModelState is not valid");
                return View("OneTimeInvestment", model);
            }

            // Debugging session userId retrieval
            Console.WriteLine("Debug: Retrieving UserId from session");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                Console.WriteLine("Debug: UserId is null, redirecting to BasicUser");
                return RedirectToAction("BasicUser", "UserRegistration");
            }

            // Debugging mutual fund retrieval
            Console.WriteLine($"Debug: Finding MutualFund with MfId = {model.MfId}");
            var mutualFund = await _context.MutualFunds.FindAsync(model.MfId);
            if (mutualFund == null)
            {
                Console.WriteLine($"Debug: MutualFund with MfId = {model.MfId} not found");
                return NotFound();
            }

            // Debugging portfolio retrieval or creation
            Console.WriteLine($"Debug: Finding or creating Portfolio for UserId = {userId.Value}, MfId = {model.MfId}, Type = OneTime");
            var portfolio = await _context.Portfolios
                .Where(p => p.UserId == userId.Value && p.MfId == model.MfId && p.Type == PortfolioType.OneTime)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (portfolio == null)
            {
                Console.WriteLine("Debug: Portfolio not found, creating a new portfolio");
                // Create a new portfolio if not found
                portfolio = new Portfolio
                {
                    UserId = userId.Value,
                    MfId = model.MfId,
                    Type = PortfolioType.OneTime,
                    TotalInvestment = model.SIPAmount,
                    TotalCurrent = model.SIPAmount,
                    Unit = model.SIPAmount / mutualFund.NAV,
                    Status = PortfolioStatus.Active, // Default status
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Portfolios.Add(portfolio);
                await _context.SaveChangesAsync(); // Save the new portfolio to generate an InvestmentId
            }
            else
            {
                Console.WriteLine("Debug: Portfolio found, updating existing portfolio");
                // Update existing portfolio
                var units = (decimal)(model.SIPAmount / mutualFund.NAV);
                portfolio.TotalInvestment += model.SIPAmount;
                portfolio.TotalCurrent += model.SIPAmount;
                portfolio.Unit += units;
                portfolio.UpdatedAt = DateTime.UtcNow;
                _context.Entry(portfolio).State = EntityState.Modified;
            }

            // Debugging save changes
            try
            {
                Console.WriteLine("Debug: Saving changes to portfolio");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Debug: DbUpdateException occurred while saving portfolio changes");
                Console.WriteLine("Debug: Exception message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Debug: Inner exception message: " + ex.InnerException.Message);
                }
                ModelState.AddModelError("", "An error occurred while processing your investment. Please try again.");
                return View("OneTimeInvestment", model);
            }

            // Debugging transaction creation
            Console.WriteLine("Debug: Creating a new transaction");
            var transaction = new Transaction
            {
                InvestmentId = portfolio.InvestmentId, // Use existing InvestmentId
                UserId = userId.Value,
                SipAmount = model.SIPAmount,
                Type = TransactionType.Buy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            // Debugging save changes for transaction
            try
            {
                Console.WriteLine("Debug: Saving changes to transaction");
                await _context.SaveChangesAsync();
                // Fetch user email and send confirmation email
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    var subject = "Investment Confirmation";
                    var message = $"Dear {user.Name},<br><br>Your one-time investment of {model.SIPAmount:C} in mutual fund ID {model.MfId} has been successfully processed.<br><br>Thank you,<br>Investment Team";

                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Debug: DbUpdateException occurred while saving transaction");
                Console.WriteLine("Debug: Exception message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Debug: Inner exception message: " + ex.InnerException.Message);
                }
                ModelState.AddModelError("", "An error occurred while processing your investment. Please try again.");
                return View("OneTimeInvestment", model);
            }

            // Debugging successful transaction
            Console.WriteLine("Debug: Transaction successful, displaying success view");
            ViewBag.Message = "Transaction Successful";
            return View("TransactionSuccess");
        }

        // GET: /MutualFund/TransactionSuccess
        public IActionResult TransactionSuccess()
        {
            return View();
        }

        // GET: /MutualFund/MonthlySIPInvestment/5
        [HttpGet("MutualFund/MonthlySIPInvestment/{mfId}")]
        public IActionResult MonthlySIPInvestment(int mfId)
        {
            var mutualFund = _context.MutualFunds.Find(mfId);
            if (mutualFund == null)
            {
                return NotFound();
            }

            var model = new InvestmentViewModel
            {
                MfId = mfId
            };

            return View(model);
        }

        // POST: /MutualFund/ProcessMonthlySIPInvestment
        [HttpPost("MutualFund/ProcessMonthlySIPInvestment")]
        public async Task<IActionResult> ProcessMonthlySIPInvestment(InvestmentViewModel model)
        {
            // Debugging ModelState validation
            Console.WriteLine("Debug: Checking ModelState.IsValid");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Debug: ModelState is not valid");
                return View("MonthlySIPInvestment", model);
            }

            // Debugging session userId retrieval
            Console.WriteLine("Debug: Retrieving UserId from session");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                Console.WriteLine("Debug: UserId is null, redirecting to BasicUser");
                return RedirectToAction("BasicUser", "UserRegistration");
            }

            // Debugging mutual fund retrieval
            Console.WriteLine($"Debug: Finding MutualFund with MfId = {model.MfId}");
            var mutualFund = await _context.MutualFunds.FindAsync(model.MfId);
            if (mutualFund == null)
            {
                Console.WriteLine($"Debug: MutualFund with MfId = {model.MfId} not found");
                return NotFound();
            }

            // Debugging portfolio retrieval or creation
            Console.WriteLine($"Debug: Finding or creating Portfolio for UserId = {userId.Value}, MfId = {model.MfId}, Type = SIP");
            var portfolio = await _context.Portfolios
                .Where(p => p.UserId == userId.Value && p.MfId == model.MfId && p.Type == PortfolioType.SIP)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (portfolio == null)
            {
                Console.WriteLine("Debug: Portfolio not found, creating a new portfolio");

                // Create a new portfolio if not found
                portfolio = new Portfolio
                {
                    UserId = userId.Value,
                    MfId = model.MfId,
                    Type = PortfolioType.SIP,
                    TotalInvestment = model.SIPAmount,
                    TotalCurrent = model.SIPAmount,
                    Status = PortfolioStatus.Active,
                    Unit = model.SIPAmount / mutualFund.NAV,
                    SipAmount = model.SIPAmount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Portfolios.Add(portfolio);
            }
            else
            {
                Console.WriteLine("Debug: Portfolio found, updating existing portfolio");

                // Update existing portfolio
                portfolio.TotalInvestment += model.SIPAmount;
                portfolio.TotalCurrent += model.SIPAmount;
                portfolio.Unit += model.SIPAmount / mutualFund.NAV;
                portfolio.SipAmount = model.SIPAmount;
                portfolio.UpdatedAt = DateTime.UtcNow;
                _context.Entry(portfolio).State = EntityState.Modified;
            }

            // Debugging save changes for portfolio
            try
            {
                Console.WriteLine("Debug: Saving changes to portfolio");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Debug: DbUpdateException occurred while saving portfolio changes");
                Console.WriteLine("Debug: Exception message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Debug: Inner exception message: " + ex.InnerException.Message);
                }
                ModelState.AddModelError("", "An error occurred while processing your investment. Please try again.");
                return View("MonthlySIPInvestment", model);
            }

            // Debugging transaction creation
            Console.WriteLine("Debug: Creating a new transaction");
            var transaction = new Transaction
            {
                InvestmentId = portfolio.InvestmentId, // Ensure InvestmentId is valid
                UserId = userId.Value,
                SipAmount = model.SIPAmount,
                Type = TransactionType.Buy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            // Debugging save changes for transaction
            try
            {
                Console.WriteLine("Debug: Saving changes to transaction");
                await _context.SaveChangesAsync();
                // Fetch user email and send confirmation email
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    var subject = "SIP Investment Confirmation";
                    var message = $"Dear {user.Name},<br><br>Your monthly SIP investment of {model.SIPAmount:C} in mutual fund ID {model.MfId} has been successfully processed.<br><br>Thank you,<br>Investment Team";

                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Debug: DbUpdateException occurred while saving transaction");
                Console.WriteLine("Debug: Exception message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Debug: Inner exception message: " + ex.InnerException.Message);
                }
                ModelState.AddModelError("", "An error occurred while processing your investment. Please try again.");
                return View("MonthlySIPInvestment", model);
            }

            // Debugging successful transaction
            Console.WriteLine("Debug: Transaction successful, displaying success view");
            ViewBag.Message = "Transaction Successful";
            return View("TransactionSuccess");
        }

        // API Methods

        // POST: /api/Transactions/Create
        [HttpPost("api/Transactions/Create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = new Transaction
            {
                InvestmentId = model.InvestmentId,
                SipAmount = model.Amount,
                Type = Enum.Parse<TransactionType>(model.TransactionType),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // GET: /api/MutualFund/FetchMFsByCategory
        [HttpGet("api/MutualFund/FetchMFsByCategory")]
        public async Task<ActionResult<IEnumerable<MutualFund>>> FetchMFsByCategory(FundCategory category)
        {
            return await _context.MutualFunds.Where(mf => mf.Category == category).ToListAsync();
        }

        // GET: /api/MutualFund/FetchMFById/{id}
        [HttpGet("api/MutualFund/FetchMFById/{id}")]
        public async Task<ActionResult<MutualFund>> FetchMFById(int id)
        {
            var mutualFund = await _context.MutualFunds.FindAsync(id);
            if (mutualFund == null)
            {
                return NotFound();
            }
            return mutualFund;
        }

        // GET: /api/MutualFund/FetchMFsById
        [HttpGet("api/MutualFund/FetchMFsById")]
        public async Task<ActionResult<IEnumerable<MutualFund>>> FetchMFsById([FromQuery] List<int> ids)
        {
            return await _context.MutualFunds.Where(mf => ids.Contains(mf.MfId)).ToListAsync();
        }

        // GET: /api/MutualFund/FetchRoi1yById/{id}
        [HttpGet("api/MutualFund/FetchRoi1yById/{id}")]
        public async Task<ActionResult<decimal>> FetchRoi1yById(int id)
        {
            var roi = await _context.ROIs.Where(r => r.MfId == id)
                                         .Select(r => r.ROI_1Y)
                                         .FirstOrDefaultAsync();
            if (roi == null)
            {
                return NotFound();
            }
            return Ok(roi);
        }

        // PATCH: /api/MutualFund/UpdateMFById/{id}
        [HttpPatch("api/MutualFund/UpdateMFById/{id}")]
        public async Task<IActionResult> UpdateMFById(int id, MutualFund mutualFund)
        {
            if (id != mutualFund.MfId)
            {
                return BadRequest();
            }

            _context.Entry(mutualFund).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MutualFundExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // GET: /api/MutualFund/GetUserNameById/{id}
        [HttpGet("api/MutualFund/GetUserNameById/{id}")]
        public async Task<ActionResult<string>> GetUserNameById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.Name);
        }

        // Helper method to check if a mutual fund exists
        private bool MutualFundExists(int id)
        {
            return _context.MutualFunds.Any(e => e.MfId == id);
        }
    }
}
