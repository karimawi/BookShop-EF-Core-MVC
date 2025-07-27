using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Models.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "MasterSchema");
            
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(p => p.Description)
                .HasMaxLength(250);
            
            builder.Property(p => p.Author)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnName("BookPrice");
                
            // Configure relationship
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
