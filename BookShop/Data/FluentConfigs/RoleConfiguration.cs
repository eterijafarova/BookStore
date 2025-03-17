using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.RoleId);
        
        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(r => r.RoleName).IsUnique();
    }
}