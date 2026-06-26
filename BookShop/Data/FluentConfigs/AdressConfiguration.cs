using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs
{
    public class AdressConfiguration : IEntityTypeConfiguration<Adress>
    {
        public void Configure(EntityTypeBuilder<Adress> builder)
        {
            builder.ToTable("Adresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .IsRequired();

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(a => a.User)
                .WithMany(u => u.Adresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}