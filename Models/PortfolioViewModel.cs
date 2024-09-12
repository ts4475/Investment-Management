namespace Investment1.Models
{
    public class PortfolioViewModel
    {
        // Unique identifier for the investment
        public int InvestmentId { get; set; }

        // Name of the mutual fund associated with the portfolio
        public string MfName { get; set; }

        // Total amount invested in this portfolio
        public decimal TotalInvestment { get; set; }

        // Number of units owned in this portfolio
        public decimal Unit { get; set; }

        // Net Asset Value (NAV) of the mutual fund
        public decimal NAV { get; set; }
    }
}
