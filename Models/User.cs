using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Column(TypeName = "date")]
        public DateTime DOB { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        [StringLength(10)]
        public string PIN { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        // UserBank properties
        [Required]
        [StringLength(20)]
        public string AccountNo { get; set; }

        [Required]
        [StringLength(20)]
        public string IFSCCode { get; set; }

        // UserIdentification properties
        [StringLength(20)]
        public string PAN { get; set; }

        [StringLength(20)]
        public string Aadhaar { get; set; }

        // Navigation Properties
        public ICollection<Portfolio> Portfolios { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Support> Supports { get; set; }
    }
}
