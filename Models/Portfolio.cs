using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment1.Models
{
    // Enum representing the possible statuses of a portfolio
    public enum PortfolioStatus
    {
        Active,     // Portfolio is currently active
        Closed,     // Portfolio has been closed
        LockIn      // Portfolio is in a lock-in period
    }

    // Enum representing the types of portfolios
    public enum PortfolioType
    {
        OneTime,    // One-time investment
        SIP         // Systematic Investment Plan
    }

    public class Portfolio
    {
        // Unique identifier for the portfolio
        [Key]
        public int InvestmentId { get; set; }

        // Foreign key linking to the User entity
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Foreign key linking to the MutualFund entity
        [ForeignKey("MutualFund")]
        public int MfId { get; set; }

        // Type of the portfolio (OneTime or SIP)
        [Required]
        public PortfolioType Type { get; set; }

        // Amount for Systematic Investment Plan (optional)
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SipAmount { get; set; }

        // Date when the SIP amount is due (optional)
        [Column(TypeName = "datetime2")]
        public DateTime? DueDate { get; set; }

        // Total amount invested in the portfolio
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalInvestment { get; set; }

        // Total current value of the investment
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCurrent { get; set; }

        // Current status of the portfolio
        [Required]
        public PortfolioStatus Status { get; set; }

        // Number of units owned in the portfolio
        public decimal Unit { get; set; }

        // Date and time when the portfolio was created
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        // Date and time when the portfolio was last updated
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties to related entities
        public User User { get; set; }               // Navigation property to User entity
        public MutualFund MutualFund { get; set; }   // Navigation property to MutualFund entity
        public ICollection<Transaction> Transactions { get; set; } // Collection of transactions related to this portfolio
    }
}
