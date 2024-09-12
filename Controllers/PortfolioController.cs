using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Investment1.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly InvestmentDbContext _context;
        private readonly IEmailService _emailService;

        // Constructor to inject the InvestmentDbContext and IEmailService dependencies
        public PortfolioController(InvestmentDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Portfolio/TransactionList/{id}
        // Action to display the list of transactions for a given investment ID
        public IActionResult TransactionList(int id)
        {
            var transactions = _context.Transactions
                                        .Where(t => t.InvestmentId == id)
                                        .OrderByDescending(t => t.CreatedAt)
                                        .ToList();

            return View(transactions);
        }

        // GET: Portfolio
        // Action to display the index page with user's portfolios
        public async Task<IActionResult> Index()
        {
            // Retrieve user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to user registration page if user ID is not available
                return RedirectToAction("BasicUser", "UserRegistration");
            }

            // Fetch portfolios associated with the current user
            var portfolios = await _context.Portfolios
                .Where(p => p.UserId == userId.Value)
                .Include(p => p.MutualFund)
                .ToListAsync();

            // Separate portfolios into Active and Inactive
            var activePortfolios = portfolios
                .Where(p => p.Status == PortfolioStatus.Active)
                .Select(p => new PortfolioViewModel
                {
                    InvestmentId = p.InvestmentId,
                    MfName = p.MutualFund.MfName,
                    TotalInvestment = p.TotalInvestment,
                    Unit = p.Unit,
                    NAV = p.MutualFund.NAV
                })
                .ToList();

            var inactivePortfolios = portfolios
                .Where(p => p.Status == PortfolioStatus.Closed || p.Status == PortfolioStatus.LockIn)
                .Select(p => new PortfolioViewModel
                {
                    InvestmentId = p.InvestmentId,
                    MfName = p.MutualFund.MfName,
                    TotalInvestment = p.TotalInvestment,
                    Unit = p.Unit,
                    NAV = p.MutualFund.NAV
                })
                .ToList();

            // Calculate total invested amount and total gains for active portfolios
            var totalInvested = activePortfolios.Sum(p => p.TotalInvestment);
            var totalGains = activePortfolios.Sum(p => (p.Unit * p.NAV) - p.TotalInvestment);

            // Pass portfolios and calculations to the view using ViewBag
            ViewBag.ActivePortfolios = activePortfolios;
            ViewBag.InactivePortfolios = inactivePortfolios;
            ViewBag.TotalInvested = totalInvested;
            ViewBag.TotalGains = totalGains;

            return View();
        }

        // GET: Portfolio/Details/{id}
        // Action to display detailed information about a specific portfolio
        public async Task<IActionResult> Details(int id)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.MutualFund) // Include MutualFund details
                .FirstOrDefaultAsync(p => p.InvestmentId == id);

            if (portfolio == null)
            {
                return NotFound();
            }

            // Calculate total gains based on units and NAV
            var totalGains = (portfolio.Unit * portfolio.MutualFund.NAV) - portfolio.TotalInvestment;

            var model = new PortfolioDetailsViewModel
            {
                InvestmentId = portfolio.InvestmentId,
                MfName = portfolio.MutualFund.MfName,
                TotalInvestment = portfolio.TotalInvestment,
                Unit = portfolio.Unit,
                NAV = portfolio.MutualFund.NAV,
                CreatedAt = portfolio.CreatedAt,
                Status = portfolio.Status,
                PortfolioType = portfolio.Type,
                SipAmount = portfolio.SipAmount,
                TotalGains = totalGains // Assign the calculated total gains
            };

            return View(model);
        }

        // GET: Portfolio/UpdateSIP/{id}
        // Action to display the form for updating the SIP amount
        public async Task<IActionResult> UpdateSIP(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            var model = new UpdateSIPViewModel
            {
                PortfolioId = portfolio.InvestmentId,
                CurrentSIPAmount = portfolio.SipAmount ?? 0
            };

            return View(model);
        }

        // POST: Portfolio/UpdateSIP
        // Action to handle form submission for updating the SIP amount
        [HttpPost]
        public async Task<IActionResult> UpdateSIP(UpdateSIPViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var portfolio = await _context.Portfolios.FindAsync(model.PortfolioId);
            if (portfolio == null)
            {
                return NotFound();
            }

            // Update the SIP amount and timestamp
            portfolio.SipAmount = model.NewSIPAmount;
            portfolio.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Entry(portfolio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(portfolio.UserId);
                if (user != null)
                {
                    // Send email notification to user
                    var subject = "SIP Amount Updated";
                    var message = $"Dear {user.Name},<br><br>Your SIP amount has been successfully updated to {model.NewSIPAmount:C}.<br><br>Thank you,<br>Investment Team";
                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }

                ViewBag.Message = "SIP Amount Updated Successfully";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(portfolio.InvestmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: Portfolio/Redeem/{id}
        // Action to handle portfolio redemption
        [HttpPost]
        public async Task<IActionResult> Redeem(int id)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.MutualFund)
                .FirstOrDefaultAsync(p => p.InvestmentId == id);

            if (portfolio == null)
            {
                return NotFound();
            }

            // Check if the portfolio is still in the lock-in period
            var lockinPeriod = portfolio.MutualFund.LockinPeriod;
            var lockinDate = portfolio.CreatedAt.AddYears(lockinPeriod);

            if (DateTime.UtcNow < lockinDate)
            {
                portfolio.Status = PortfolioStatus.LockIn;
                ViewBag.AlertMessage = "Lock-in period is not covered. Portfolio status updated to Lock-In.";
            }
            else
            {
                portfolio.Status = PortfolioStatus.Closed;
                ViewBag.AlertMessage = "Lock-in period covered. Portfolio status updated to Closed.";
            }

            portfolio.UpdatedAt = DateTime.UtcNow;

            // Create a new transaction record for the redemption
            var transaction = new Transaction
            {
                UserId = portfolio.UserId,
                InvestmentId = portfolio.InvestmentId,
                SipAmount = portfolio.TotalInvestment, // Record the total investment in the transaction
                Type = TransactionType.Sell, // Assume redeeming is a "Sell" transaction
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            try
            {
                _context.Entry(portfolio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(portfolio.UserId);
                if (user != null)
                {
                    // Send email notification to user about redemption
                    var subject = "Portfolio Redemption";
                    var message = $"Dear {user.Name},<br><br>Your portfolio with ID {portfolio.InvestmentId} has been successfully redeemed. The status is now {portfolio.Status}.<br><br>Thank you,<br>Investment Team";
                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }

                return RedirectToAction("Details", new { id = portfolio.InvestmentId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(portfolio.InvestmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: Portfolio/PaySIP/{id}
        // Action to handle SIP payment
        [HttpPost]
        public async Task<IActionResult> PaySIP(int id)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.MutualFund) // Include MutualFund details
                .FirstOrDefaultAsync(p => p.InvestmentId == id);

            if (portfolio == null)
            {
                return NotFound();
            }

            if (portfolio.Type != PortfolioType.SIP || portfolio.SipAmount == null || portfolio.SipAmount <= 0)
            {
                return BadRequest("Invalid SIP amount or portfolio type.");
            }

            var sipAmount = portfolio.SipAmount.Value;
            var nav = portfolio.MutualFund.NAV;

            // Calculate new units based on SIP amount
            var newUnits = sipAmount / nav;
            portfolio.Unit += newUnits;

            // Update TotalInvestment
            portfolio.TotalInvestment += sipAmount;

            // Create a new transaction record for the SIP payment
            var transaction = new Transaction
            {
                UserId = portfolio.UserId,
                InvestmentId = portfolio.InvestmentId,
                SipAmount = sipAmount,
                UpdatedAt = DateTime.UtcNow,
                Type = TransactionType.Buy
            };

            _context.Transactions.Add(transaction);

            // Update portfolio
            portfolio.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Entry(portfolio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(portfolio.UserId);
                if (user != null)
                {
                    // Send email notification to user about SIP payment
                    var subject = "SIP Payment Confirmation";
                    var message = $"Dear {user.Name},<br><br>Your SIP payment of {sipAmount:C} has been successfully processed. New units purchased: {newUnits}.<br><br>Thank you,<br>Investment Team";
                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }

                return RedirectToAction("Details", new { id = portfolio.InvestmentId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(portfolio.InvestmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Helper method to check if a portfolio exists
        private bool PortfolioExists(int id)
        {
            return _context.Portfolios.Any(e => e.InvestmentId == id);
        }
    }
}
