namespace Investment1.Models
{
    public class TransactionViewModel
    {
        public int MfId { get; set; } // Identifier for the Mutual Fund involved in the transaction

        public int InvestmentId { get; set; } // Identifier for the Portfolio or Investment related to the transaction

        public int UserId { get; set; } // Identifier for the User making the transaction

        public decimal Amount { get; set; } // The amount of money involved in the transaction

        public string TransactionType { get; set; } // Type of transaction ('BUY' or 'SELL')
    }
}
