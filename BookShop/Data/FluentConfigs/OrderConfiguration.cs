using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.UserAdress)
                .WithMany()
                .HasForeignKey(o => o.UserAdressId) 
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.UserBankCard) 
                .WithMany()
                .HasForeignKey(o => o.UserBankCardId) 
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.OrderDate)
                .HasColumnType("datetime2")
                .IsRequired();
        }
    }
}