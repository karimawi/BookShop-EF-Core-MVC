using Microsoft.EntityFrameworkCore;
using BookShop.Models;
using BookShop.Models.Configurations;

namespace BookShop.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply configuration
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=BookShop;Trusted_Connection=true;TrustServerCertificate=true;");
            }
        }
    }
}
