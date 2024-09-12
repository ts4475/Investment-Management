using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment1.Models
{
    public class Support
    {
        // Unique identifier for the support request
        [Key]
        public int SupportId { get; set; }

        // Foreign key to the User table
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Subject of the support request
        [Required]
        [StringLength(100)]
        // Ensures that the Subject field is mandatory and cannot be left empty.
        // Limits the length of the subject to 100 characters.
        public string Subject { get; set; }

        // Detailed description of the support request
        // This field is optional, so it does not have a [Required] attribute.
        public string Description { get; set; }

        // Timestamp when the support request was created
        [Required]
        [Column(TypeName = "datetime2")]
        // Ensures that the CreatedAt field is mandatory and cannot be left empty.
        // Specifies that the column type in the database should be "datetime2" for better precision.
        public DateTime CreatedAt { get; set; }

        // Navigation property for the related User
        // This property allows navigation to the User entity that this support request is associated with.
        public User User { get; set; }
    }
}
