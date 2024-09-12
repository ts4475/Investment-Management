using System.ComponentModel.DataAnnotations;

namespace Investment1.Models
{
    public class SupportViewModel
    {

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }
    }
}