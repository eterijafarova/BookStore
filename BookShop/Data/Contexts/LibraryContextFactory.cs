using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookShop.Data.Contexts
{
    public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
    {
        public LibraryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();
            optionsBuilder.UseSqlite("Data Source=bookshop.db");

            return new LibraryContext(optionsBuilder.Options);
        }
    }
}