using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Data.FluentConfigs;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .ValueGeneratedOnAdd();

        builder.Property(g => g.GenreName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("The name of the genre");

        builder.HasIndex(g => g.GenreName)
            .IsUnique();

        builder.Property(g => g.ParentGenreId)
            .IsRequired(false);

        builder.HasOne(g => g.ParentGenre)
            .WithMany(g => g.SubGenres)
            .HasForeignKey(g => g.ParentGenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}