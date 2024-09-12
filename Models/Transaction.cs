using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment1.Models
{
    // Enum to represent the type of transaction
    public enum TransactionType
    {
        Buy,  // Purchase of units
        Sell  // Sale of units
    }

    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; } // Unique identifier for the transaction

        [ForeignKey("Portfolio")]
        public int InvestmentId { get; set; } // Foreign key to the Portfolio associated with this transaction

        [ForeignKey("User")]
        public int UserId { get; set; } // Foreign key to the User who made the transaction

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SipAmount { get; set; } // The amount involved in the transaction

        [Required]
        public TransactionType Type { get; set; } // Type of transaction (Buy or Sell)

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } // Date and time when the transaction was created

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; } // Date and time when the transaction was last updated

        // Navigation Properties
        public Portfolio Portfolio { get; set; } // Navigation property to the associated Portfolio
        public User User { get; set; } // Navigation property to the associated User
    }
}
