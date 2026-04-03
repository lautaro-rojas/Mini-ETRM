using Microsoft.EntityFrameworkCore;
using Mini_ETRM.Domain.Entities;

namespace Mini_ETRM.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API for mapping the Trade entity without polluting the Domain with [Table] or [Column] attributes
            modelBuilder.Entity<Trade>(builder =>
            {
                builder.ToTable("Trades");
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Commodity).HasConversion<string>().IsRequired();
                builder.Property(t => t.Type).HasConversion<string>().IsRequired();

                // Precision for financial decimals (e.g., 18 total digits, 4 decimal places)
                builder.Property(t => t.Volume).HasPrecision(18, 4).IsRequired();
                builder.Property(t => t.ExecutionPrice).HasPrecision(18, 4).IsRequired();
            });
        }
    }
}