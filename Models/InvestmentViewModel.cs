namespace Investment1.Models
{
    public class InvestmentViewModel
    {
        // Identifier for the mutual fund being invested in
        public int MfId { get; set; }

        // The amount to be invested in the mutual fund through SIP (Systematic Investment Plan)
        public decimal SIPAmount { get; set; }

    }
}
