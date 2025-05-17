using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Primary key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasDefaultValueSql("NEWID()")
                .ValueGeneratedOnAdd();
            
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(255);
            builder.HasIndex(b => b.Title)
                .IsUnique();
            
            builder.Property(b => b.Price)
                .HasPrecision(18, 2)
                .IsRequired();
            builder.Property(b => b.Stock)
                .IsRequired();
            
            builder.Property(b => b.Description)
                .HasMaxLength(4000);
            builder.Property(b => b.ImageUrl)
                .HasMaxLength(4000);
            
            builder.HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(b => b.PublisherId)
                .IsRequired(false);
            builder.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasOne(b => b.Warehouse)
                .WithOne(w => w.Book)
                .HasForeignKey<Warehouse>(w => w.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(b => b.Reviews)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(b => b.BookAttributeValues)
                .WithOne(v => v.Book)
                .HasForeignKey(v => v.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany<OrderItem>(b => b.OrderItems)
                .WithOne(oi => oi.Book)
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
