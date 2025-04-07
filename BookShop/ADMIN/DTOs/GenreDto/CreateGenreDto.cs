namespace BookShop.ADMIN.DTOs.GenreDto
{
    public class CreateGenreDto
    {
        public string Name { get; set; }  // Название жанра
    }

    public class CreateSubGenreDto
    {
        public string Name { get; set; }  // Название поджанра
        public int ParentGenreId { get; set; }  // ID родительского жанра через int
    }
}