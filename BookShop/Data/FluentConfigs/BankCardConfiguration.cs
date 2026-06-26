using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs
{
    public class CardConfiguration : IEntityTypeConfiguration<BankCard>
    {
        public void Configure(EntityTypeBuilder<BankCard> builder)
        {
            builder.ToTable("BankCards");

            builder.HasKey(card => card.Id);

            builder.Property(card => card.Id)
                .IsRequired();

            builder.Property(card => card.CardNumber)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(card => card.CardHolderName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(card => card.ExpirationDate)
                .IsRequired();

            builder.Property(card => card.CVV)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(card => card.Last4Digits)
                .HasMaxLength(4)
                .IsRequired();

            builder.Property(card => card.UserId)
                .IsRequired();

            builder.HasOne(card => card.User)
                .WithMany(user => user.BankCards)
                .HasForeignKey(card => card.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}