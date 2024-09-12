using Investment1.Models;

namespace Investment1.ViewModels
{
    public class MutualFundDetailsViewModel
    {
        // Details of the mutual fund being displayed
        public MutualFund MutualFund { get; set; }

        // Return on Investment (ROI) information for the mutual fund
        public ROIViewModel ROI { get; set; }

        // Minimum SIP amount required for the mutual fund
        public decimal MinimumSIPAmount { get; set; }

        // Nested class to hold ROI data for various time periods
        public class ROIData
        {
            // ROI for the last 1 month
            public decimal ROI1M { get; set; }

            // ROI for the last 6 months
            public decimal ROI6M { get; set; }

            // ROI for the last 1 year
            public decimal ROI1Y { get; set; }

            // ROI for the last 3 years
            public decimal ROI3Y { get; set; }

            // ROI for the last 5 years
            public decimal ROI5Y { get; set; }
        }
    }
}
