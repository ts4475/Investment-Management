// File: InvestmentSelectionViewModel.cs
using Investment1.Models;
using System.ComponentModel.DataAnnotations;

namespace Investment1.ViewModels
{
    public class InvestmentSelectionViewModel
    {
        // The MutualFund object representing the mutual fund selected for investment
        public MutualFund MutualFund { get; set; }

        // Minimum amount required for a Systematic Investment Plan (SIP)
        public decimal MinimumSIPAmount { get; set; }

        // ID of the user making the investment
        public int UserId { get; set; } 

        // The type of investment (e.g., SIP, lump sum) - must be provided
        [Required]
        public PortfolioType InvestmentType { get; set; } // Enum for Investment Type

        // The amount to be invested - must be greater than 0
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        // Number of units to be purchased - must be greater than 0
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Units must be greater than 0.")]
        public int Units { get; set; }

        // ID of the mutual fund selected for investment
        public int MfId { get; set; } // Mutual Fund Id
    }
}
