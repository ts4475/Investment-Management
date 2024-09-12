using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investment1.Data;
using Investment1.Models;
using Investment1.ViewModels;

namespace Investment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeInvestmentController : ControllerBase
    {
        private readonly InvestmentDbContext _context;

        // Constructor to inject the DbContext dependency
        public MakeInvestmentController(InvestmentDbContext context)
        {
            _context = context;
        }

        // Endpoint to add a new investment portfolio
        [HttpPost("AddInvestment")]
        public async Task<ActionResult<Portfolio>> AddInvestment([FromBody] Portfolio portfolio)
        {
            if (portfolio == null)
            {
                return BadRequest("Portfolio data is null.");
            }

            // Set timestamps for creation and update
            portfolio.CreatedAt = DateTime.Now;
            portfolio.UpdatedAt = DateTime.Now;

            // Add the portfolio to the database
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            // Return a response indicating the portfolio was created, including its location
            return CreatedAtAction(nameof(GetInvestmentById), new { id = portfolio.InvestmentId }, portfolio);
        }

        // Endpoint to process a one-time payment
        [HttpPost("PayOneTime")]
        public async Task<IActionResult> PayOneTime([FromBody] InvestmentPaymentViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Payment data is null.");
            }

            // Retrieve the portfolio to which the payment will be applied
            var portfolio = await _context.Portfolios.FindAsync(model.PortfolioId);
            if (portfolio == null)
            {
                return NotFound();
            }

            // Update the portfolio with the payment amount
            portfolio.TotalInvestment += model.Amount;
            portfolio.UpdatedAt = DateTime.Now;
            _context.Entry(portfolio).State = EntityState.Modified;

            // Create a new transaction record for the payment
            var transaction = new Transaction
            {
                UserId = model.UserId,
                InvestmentId = portfolio.InvestmentId,
                SipAmount = model.Amount,
                Type = TransactionType.Buy, // Assuming Buy for one-time payments
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);

            // Save changes to the database
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Endpoint to process a SIP (Systematic Investment Plan) payment
        [HttpPost("PaySIP")]
        public async Task<IActionResult> PaySIP([FromBody] InvestmentPaymentViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Payment data is null.");
            }

            // Retrieve the portfolio to which the payment will be applied
            var portfolio = await _context.Portfolios.FindAsync(model.PortfolioId);
            if (portfolio == null)
            {
                return NotFound();
            }

            // Update the portfolio with the payment amount
            portfolio.TotalInvestment += model.Amount;
            portfolio.UpdatedAt = DateTime.Now;
            _context.Entry(portfolio).State = EntityState.Modified;

            // Create a new transaction record for the payment
            var transaction = new Transaction
            {
                UserId = model.UserId,
                InvestmentId = portfolio.InvestmentId,
                SipAmount = model.Amount,
                Type = TransactionType.Buy, // Assuming Buy for SIP payments as well
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);

            // Save changes to the database
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Endpoint to retrieve a portfolio by its ID
        [HttpGet("GetInvestmentById/{id}")]
        public async Task<ActionResult<Portfolio>> GetInvestmentById(int id)
        {
            // Find the portfolio by ID
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            return Ok(portfolio);
        }
    }
}
