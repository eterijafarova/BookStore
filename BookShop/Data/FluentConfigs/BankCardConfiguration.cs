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
           
            builder.Property(card => card.CardNumber)
                .HasColumnType("nvarchar(4000)")
                .IsRequired();

            builder.Property(card => card.CardHolderName)
                .HasColumnType("nvarchar(100)")
                .IsRequired();

            builder.Property(card => card.ExpirationDate)
                .HasColumnType("datetime2")
                .IsRequired();

       
            builder.Property(card => card.CVV)
                .HasColumnType("nvarchar(4000)")
                .IsRequired();

         
            builder.Property(card => card.Last4Digits)
                .HasColumnType("nvarchar(4)")
                .IsRequired();
            
            builder.HasOne(card => card.User)
                .WithMany(user => user.BankCards)
                .HasForeignKey(card => card.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}