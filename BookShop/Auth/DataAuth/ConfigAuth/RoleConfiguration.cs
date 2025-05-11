using BookShop.Auth.ModelsAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Auth.DataAuth.ConfigAuth
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_Roles");
            
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("roleName");
        }
    }
}