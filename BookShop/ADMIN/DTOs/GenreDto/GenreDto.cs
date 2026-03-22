namespace BookShop.ADMIN.DTOs.GenreDto;

public class GenreDto
{
    public int Id { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public int? ParentGenreId { get; set; }

    public List<GenreDto> SubGenres { get; set; } = new();
    public List<UpdateBookDto> Books { get; set; } = new();
}