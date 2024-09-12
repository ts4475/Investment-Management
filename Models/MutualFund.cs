using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment1.Models
{
    // Enumeration to classify mutual funds into different categories
    public enum FundCategory
    {
        LargeCap, // Large capitalization funds
        MidCap,   // Mid capitalization funds
        SmallCap   // Small capitalization funds
    }

    public class MutualFund
    {
        // Unique identifier for each mutual fund
        [Key]
        public int MfId { get; set; }

        // Name of the mutual fund, with a maximum length of 100 characters
        [Required]
        [StringLength(100)]
        public string MfName { get; set; }

        // Category of the mutual fund as defined by the FundCategory enum
        [Required]
        public FundCategory Category { get; set; }

        // Net Asset Value (NAV) of the mutual fund, with precision of 18 digits and 2 decimal places
        [Column(TypeName = "decimal(18, 2)")]
        public decimal NAV { get; set; }

        // Lock-in period for the mutual fund in years
        public int LockinPeriod { get; set; }

    }
}
