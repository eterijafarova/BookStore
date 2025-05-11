using BookShop.Auth.ModelsAuth;
using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// Ensure the correct namespace for your models

namespace BookShop.Auth.DataAuth.ConfigAuth
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            // Устанавливаем основной ключ
            entity.HasKey(e => e.Id);

            // Индекс для поля Email
            entity.HasIndex(e => e.Email)
                .IsUnique();

            // Конфигурация для поля UserName
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("userName");

            // Конфигурация для поля Email
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("email");

            // Конфигурация для поля IsEmailConfirmed (по умолчанию false)
            entity.Property(e => e.IsEmailConfirmed)
                .HasDefaultValue(false)
                .HasColumnName("emailConfirmed");

            // Конфигурация для поля PasswordHash
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnName("passwordHash");

            // Конфигурация для поля RefreshToken
            entity.Property(e => e.RefreshToken)
                .HasColumnName("refreshToken");

            // Конфигурация для поля RefreshTokenExpiration
            entity.Property(e => e.RefreshTokenExpiration)
                .HasColumnName("refreshTokenExpiration");

            // Индекс для RefreshToken с фильтром, исключая NULL значения
            entity.HasIndex(e => e.RefreshToken)
                .IsUnique()
                .HasDatabaseName("UQ_RefreshToken")
                .HasFilter("[refreshToken] IS NOT NULL");

            // Связь с таблицей Orders (один пользователь - много заказов)
            entity.HasMany(e => e.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict для предотвращения удаления пользователя с заказами

            // Связь с таблицей Reviews (один пользователь - много отзывов)
            entity.HasMany(e => e.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade для удаления отзывов при удалении пользователя

            // // Связь с таблицей Addresses (один пользователь - один адрес)
            // entity.HasOne(u => u.Adress)  // Один пользователь - один адрес
            //     .WithOne(a => a.User)       // Один адрес - один пользователь
            //     .HasForeignKey<Adress>(a => a.UserId)  // Устанавливаем внешний ключ в Address
            //     .OnDelete(DeleteBehavior.Cascade);  // Удаление адреса при удалении пользователя

            // Связь с таблицей BankCards (один пользователь - много карт)
            entity.HasMany(u => u.BankCards)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade для удаления карт при удалении пользователя
        }
    }
}
