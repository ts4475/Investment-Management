using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment1.Models
{
    public class ROI
    {
        // Unique identifier for the ROI record
        [Key]
        public int Id { get; set; }

        // Foreign key to link the ROI record with a Mutual Fund
        [ForeignKey("MutualFund")]
        public int MfId { get; set; }

        // Return on Investment for the past 1 month
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ROI_1M { get; set; }

        // Return on Investment for the past 6 months
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ROI_6M { get; set; }

        // Return on Investment for the past 1 year
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ROI_1Y { get; set; }

        // Return on Investment for the past 3 years
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ROI_3Y { get; set; }

        // Return on Investment for the past 5 years
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ROI_5Y { get; set; }

        // Navigation property to access the related Mutual Fund entity
        public MutualFund MutualFund { get; set; }
    }
}
