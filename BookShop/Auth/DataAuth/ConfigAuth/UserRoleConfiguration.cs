using BookShop.Auth.ModelsAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Auth.DataAuth.ConfigAuth;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> entity)
    {
        entity.HasKey(e => e.UserRoleId);

        entity.Property(e => e.UserRoleId)
            .HasColumnName("userRoleId");

        entity.Property(e => e.RoleId)
            .IsRequired()
            .HasColumnName("roleId");

        entity.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("userId");

        entity.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserRoles_Role");

        entity.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserRoles_User");
    }
    }
