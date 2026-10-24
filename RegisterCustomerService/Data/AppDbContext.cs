using Microsoft.EntityFrameworkCore;
using RegisterCustomerService.Models;

namespace RegisterCustomerService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map CLR model to existing PostgreSQL table/columns (lowercase, snake_case)
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.HasKey(e => e.Id).HasName("id");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}
