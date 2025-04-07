namespace BookShop.ADMIN.DTOs.GenreDto
{
    public class UpdateGenreDto
    {
        public string Name { get; set; }  // Название жанра (обновляемое поле)
        public int? ParentGenreId { get; set; }  // ID родительского жанра (необязательное поле)
    }
    
    public class UpdateSubGenreDto
    {
        public string Name { get; set; }  // Название поджанра (обновляемое поле)
        public int? ParentGenreId { get; set; }  // ID родительского жанра (необязательное поле)
    }
}