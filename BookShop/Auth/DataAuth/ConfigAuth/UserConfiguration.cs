using BookShop.Auth.ModelsAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Auth.DataAuth.ConfigAuth
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            // Устанавливаем первичный ключ как Id (тип Guid), а не UserName
            entity.HasKey(e => e.Id);

            // Создаем уникальный индекс для Email
            entity.HasIndex(e => e.Email)
                .IsUnique();

            // Настройка свойства UserName
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("userName");

            // Настройка свойства Email
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("email");

            // Настройка для подтверждения email, предполагаем, что в модели свойство называется EmailConfirmed
            entity.Property(e => e.IsEmailConfirmed)
                .HasDefaultValue(false)
                .HasColumnName("emailConfirmed");

            // Настройка для хэша пароля (PasswordHash вместо Password)
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnName("passwordHash");

            entity.Property(e => e.RefreshToken)
                .HasColumnName("refreshToken");

            entity.Property(e => e.RefreshTokenExpiration)
                .HasColumnName("refreshTokenExpiration");

// Если нужен уникальный индекс:
            entity.HasIndex(e => e.RefreshToken)
                .IsUnique()
                .HasDatabaseName("UQ_RefreshToken")
                .HasFilter("[refreshToken] IS NOT NULL");

            // Настройка навигационных свойств для заказов
            entity.HasMany(e => e.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка навигационных свойств для отзывов
            entity.HasMany(e => e.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
