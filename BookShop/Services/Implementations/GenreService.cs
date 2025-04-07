// using Microsoft.EntityFrameworkCore;
// using BookShop.Data;
// using BookShop.Data.Models;
// using BookShop.Services.Interfaces;
// using BookShop.Shared.DTO.Requests;
// using BookShop.Shared.DTO.Response;
// using System.Linq;
//
// namespace BookShop.Services.Implementations
// {
//     public class GenreService : IGenreService
//     {
//         private readonly LibraryContext _context;
//
//         public GenreService(LibraryContext context)
//         {
//             _context = context;
//         }
//
//         // Создание жанра
//         public async Task<GenreResponseDTO> CreateGenreAsync(CreateGenreDTO dto)
//         {
//             // Создаем новый жанр
//             var genre = new Genre
//             {
//                 GenreName = dto.Name,
//                 ParentGenreId = dto.ParentGenreId
//             };
//
//             _context.Genres.Add(genre);
//             await _context.SaveChangesAsync();
//
//             string? parentGenreName = null;
//             if (genre.ParentGenreId.HasValue)
//             {
//                 // Загрузим родительский жанр сразу
//                 var parentGenre = await _context.Genres
//                     .FirstOrDefaultAsync(g => g.Id == genre.ParentGenreId.Value);
//                 parentGenreName = parentGenre?.GenreName;
//             }
//
//             // Получаем поджанры
//             var subGenres = await _context.Genres
//                 .Where(g => g.ParentGenreId == genre.Id)
//                 .Select(sg => new GenreSubGenreDTO(sg.Id, sg.GenreName))
//                 .ToListAsync();
//
//             return new GenreResponseDTO(
//                 genre.Id,
//                 genre.GenreName,
//                 genre.ParentGenreId,
//                 parentGenreName,
//                 subGenres
//             );
//         }
//
//         // Получение жанра по имени
//         public async Task<GenreResponseDTO> GetGenreAsync(string name)
//         {
//             var genre = await _context.Genres
//                 .FirstOrDefaultAsync(g => g.GenreName.Equals(name, StringComparison.OrdinalIgnoreCase));
//
//             if (genre == null)
//                 throw new KeyNotFoundException("Genre not found");
//
//             // Получение родительского жанра
//             string? parentGenreName = null;
//             if (genre.ParentGenreId.HasValue)
//             {
//                 var parentGenre = await _context.Genres
//                     .FirstOrDefaultAsync(g => g.Id == genre.ParentGenreId.Value);
//                 parentGenreName = parentGenre?.GenreName;
//             }
//
//             // Получение поджанров
//             var subGenres = await _context.Genres
//                 .Where(g => g.ParentGenreId == genre.Id)
//                 .Select(sg => new GenreSubGenreDTO(sg.Id, sg.GenreName))
//                 .ToListAsync();
//
//             return new GenreResponseDTO(
//                 genre.Id,
//                 genre.GenreName,
//                 genre.ParentGenreId,
//                 parentGenreName,
//                 subGenres
//             );
//         }
//
//         // Получение списка жанров с поджанрами и родительскими жанрами
//         public async Task<IEnumerable<GenreResponseDTO>> GetGenresAsync(int page = 1, int pageSize = 20)
//         {
//             // Загружаем жанры с родительскими жанрами и поджанрами в одном запросе
//             var genres = await _context.Genres
//                 .Include(g => g.ParentGenre)  // Загружаем родительский жанр
//                 .Include(g => g.SubGenres)  // Загружаем поджанры
//                 .Skip((page - 1) * pageSize)
//                 .Take(pageSize)
//                 .ToListAsync();
//
//             var genreResponses = genres.Select(genre =>
//             {
//                 // Получаем родительский жанр
//                 string? parentGenreName = genre.ParentGenre?.GenreName;
//
//                 // Получаем поджанры
//                 var subGenres = genre.SubGenres.Select(sg => new GenreSubGenreDTO(sg.Id, sg.GenreName)).ToList();
//
//                 return new GenreResponseDTO(
//                     genre.Id,
//                     genre.GenreName,
//                     genre.ParentGenreId,
//                     parentGenreName,
//                     subGenres
//                 );
//             });
//
//             return genreResponses;
//         }
//     }
// }
