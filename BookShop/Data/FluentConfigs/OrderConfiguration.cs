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
            
            builder.Property(o => o.OriginalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(o => o.PromoCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired(false);
            
            builder.Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            
            builder.Property(o => o.FinalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            builder.Property(o => o.Status)
                .IsRequired();
            
            builder.Property(o => o.OrderDate)
                .HasColumnType("datetime2")
                .IsRequired();
        }
    }
}