namespace Investment1.Models
{
    public class PortfolioDetailsViewModel
    {
        // Unique identifier for the investment
        public int InvestmentId { get; set; }

        // Name of the mutual fund associated with the portfolio
        public string MfName { get; set; }

        // Total amount invested in the portfolio
        public decimal TotalInvestment { get; set; }

        // Number of units owned in the portfolio
        public decimal Unit { get; set; }

        // Net Asset Value (NAV) of the mutual fund
        public decimal NAV { get; set; }

        // Date and time when the portfolio was created
        public DateTime CreatedAt { get; set; }

        // Current status of the portfolio (e.g., Active, Closed, LockIn)
        public PortfolioStatus Status { get; set; }

        // Type of the portfolio (e.g., OneTime, SIP)
        public PortfolioType PortfolioType { get; set; }

        // SIP amount for systematic investment plans, if applicable (nullable)
        public decimal? SipAmount { get; set; }

        // Total gains from the portfolio, calculated as (Current Value - Total Investment)
        public decimal TotalGains { get; set; }
    }
}
