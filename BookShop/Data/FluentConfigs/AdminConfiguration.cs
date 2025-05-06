using BookShop.ADMIN.ModelsAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> entity)
        {
   
            entity.HasKey(e => e.Id)
                .HasName("PK_Admin");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWID()");
                
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("userId");
                
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("username");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("email");

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnName("passwordHash");
            
            entity.HasOne(a => a.User)
                .WithOne(u => u.Admin)  
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull) 
                .HasConstraintName("FK_Admin_User");
        }
    }
}