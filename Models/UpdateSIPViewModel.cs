using System.ComponentModel.DataAnnotations;

namespace Investment1.Models
{
    public class UpdateSIPViewModel
    {
        // Identifier for the Portfolio being updated
        public int PortfolioId { get; set; }

        // The new SIP amount to be updated, must be greater than 100
        [Required(ErrorMessage = "New SIP Amount is required")]
        [Range(101, double.MaxValue, ErrorMessage = "New SIP Amount must be greater than 100")]
        public decimal NewSIPAmount { get; set; }

        // The current SIP amount before the update
        public decimal CurrentSIPAmount { get; set; }
    }
}
