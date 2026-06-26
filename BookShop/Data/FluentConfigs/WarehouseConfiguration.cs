using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .IsRequired();

        builder.HasOne(w => w.Book)
            .WithOne(b => b.Warehouse)
            .HasForeignKey<Warehouse>(w => w.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(w => w.Quantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(w => w.UpdatedAt)
            .IsRequired();
    }
}