namespace BookShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string GenreName { get; set; } = string.Empty;

        public int? ParentGenreId { get; set; }
        public Genre? ParentGenre { get; set; }

        public ICollection<Genre> SubGenres { get; set; } = new List<Genre>();
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}