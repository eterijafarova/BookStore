using BookShop.Auth.DTOAuth.Requests;
using BookShop.Auth.ModelsAuth;
using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data.Contexts;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> ops) : base(ops)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PreOrder> PreOrders { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<Adress> Adresses { get; set; }
    public DbSet<BankCard> BankCards { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryContext).Assembly);
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Role>().HasData(
            new Role 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                RoleName = "AppUser" 
            },
            new Role 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                RoleName = "Admin" 
            },
            new Role 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), 
                RoleName = "SuperAdmin" 
            }
        );
    }
}
