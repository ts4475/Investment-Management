namespace Investment1.ViewModels
{
    // ViewModel for handling investment payment information
    public class InvestmentPaymentViewModel
    {
        // ID of the user making the investment payment
        public int UserId { get; set; }

        // ID of the portfolio associated with the investment
        public int PortfolioId { get; set; }

        // Amount of money being invested
        public decimal Amount { get; set; }
    }
}
