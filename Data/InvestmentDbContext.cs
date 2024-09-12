using Microsoft.EntityFrameworkCore;
using Investment1.Models;

namespace Investment1.Data
{
    public class InvestmentDbContext : DbContext
    {
        // Constructor to initialize the DbContext with options
        public InvestmentDbContext(DbContextOptions<InvestmentDbContext> options)
            : base(options)
        {
        }

        // DbSets representing the tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<MutualFund> MutualFunds { get; set; }
        public DbSet<ROI> ROIs { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Support> Supports { get; set; }

        // Configures the model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the one-to-many relationship between Portfolio and User
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.User) // Portfolio has one User
                .WithMany(u => u.Portfolios) // User has many Portfolios
                .HasForeignKey(p => p.UserId) // Foreign key in Portfolio
                .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of a User if they have associated Portfolios

            // Configure the one-to-many relationship between Portfolio and MutualFund
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.MutualFund) // Portfolio has one MutualFund
                .WithMany() // MutualFund has no navigation property back to Portfolio
                .HasForeignKey(p => p.MfId) // Foreign key in Portfolio
                .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of a MutualFund if it has associated Portfolios

            // Configure the one-to-many relationship between Transaction and Portfolio
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Portfolio) // Transaction has one Portfolio
                .WithMany(p => p.Transactions) // Portfolio has many Transactions
                .HasForeignKey(t => t.InvestmentId) // Foreign key in Transaction
                .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of a Portfolio if it has associated Transactions

            // Configure the one-to-many relationship between Transaction and User
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User) // Transaction has one User
                .WithMany(u => u.Transactions) // User has many Transactions
                .HasForeignKey(t => t.UserId) // Foreign key in Transaction
                .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of a User if they have associated Transactions

            // Configure the one-to-many relationship between Support and User
            modelBuilder.Entity<Support>()
                .HasOne(s => s.User) // Support has one User
                .WithMany(u => u.Supports) // User has many Supports
                .HasForeignKey(s => s.UserId) // Foreign key in Support
                .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of a User if they have associated Supports

            base.OnModelCreating(modelBuilder); // Call the base class implementation
        }
    }
}
