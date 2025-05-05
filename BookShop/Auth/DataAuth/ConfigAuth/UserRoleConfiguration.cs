using BookShop.Auth.ModelsAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Auth.DataAuth.ConfigAuth;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
        public void Configure(EntityTypeBuilder<UserRole> entity)
        {
            entity.HasKey(e => e.UserRoleId)
                .HasName("PK__UserRole__CD3149CCDE4D7241");

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Role");

            // Настройка связи с User: Один User имеет много UserRole
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_User");
        }
    }
