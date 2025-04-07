using BookShop.ADMIN.DTOs.GenreDto;
using BookShop.Shared.DTO.Requests;
using BookShop.Shared.DTO.Response;

namespace BookShop.Services.Interfaces
{
    public interface IGenreService
    {
        Task<GenreResponseDTO> CreateGenreAsync(CreateGenreDTO dto);

        Task<GenreResponseDTO> GetGenreAsync(string name);

        Task<IEnumerable<GenreResponseDTO>> GetGenresAsync(int page = 1, int pageSize = 20);

        // Новый метод для обновления жанра
        Task<GenreResponseDTO> UpdateGenreAsync(int id, UpdateGenreDto dto);

        // Новый метод для удаления жанра
        Task<bool> DeleteGenreAsync(int id);
    }
}

