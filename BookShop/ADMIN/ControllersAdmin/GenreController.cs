using AutoMapper;
using BookShop.ADMIN.DTOs.GenreDto;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;  
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.ADMIN.ControllersAdmin
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly ILogger<GenreController> _logger;
        private readonly IMapper _mapper;
        private readonly IGenreService _genreService;  

        public GenreController(LibraryContext context, ILogger<GenreController> logger, IMapper mapper, IGenreService genreService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _genreService = genreService;  
        }
        
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GenreResponseDto>>> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();  
            return Ok(genres);
        }
        
        
        [HttpPost("createParent")]
        public async Task<ActionResult<GenreDto>> CreateParentGenre(CreateGenreDto dto)
        {
            if (_context.Genres.Any(g => g.GenreName == dto.Name))
            {
                _logger.LogWarning("Genre exists: {Name}", dto.Name);
                return BadRequest("Genre already exists");
            }

            var genre = new Genre
            {
                GenreName = dto.Name
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var genreDto = new GenreDto
            {
                Id = genre.Id,
                GenreName = genre.GenreName,
                ParentGenreId = genre.ParentGenreId,
                SubGenres = new(),
                Books = new()
            };

            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genreDto);
        }
        
        
        [HttpPost("createSub")]
        public async Task<ActionResult<GenreDto>> CreateSubGenre(CreateSubGenreDto dto)
        {
            if (_context.Genres.Any(g => g.GenreName == dto.Name))
                return BadRequest("Genre already exists");

            var parent = await _context.Genres.FindAsync(dto.ParentGenreId);
            if (parent == null)
                return BadRequest("Parent genre not found");

            var subGenre = new Genre
            {
                GenreName = dto.Name,
                ParentGenreId = dto.ParentGenreId
            };

            _context.Genres.Add(subGenre);
            await _context.SaveChangesAsync();

            var dtoResult = new GenreDto
            {
                Id = subGenre.Id,
                GenreName = subGenre.GenreName,
                ParentGenreId = subGenre.ParentGenreId,
                SubGenres = new(),
                Books = new()
            };

            return CreatedAtAction(nameof(GetGenreById), new { id = subGenre.Id }, dtoResult);
        }
        
        
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDto>> GetGenreById(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.SubGenres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
            {
                _logger.LogWarning("Genre with ID: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Fetched genre with ID: {Id}", id);
            var genreDto = _mapper.Map<GenreDto>(genre);
            return genreDto;
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.SubGenres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
            {
                _logger.LogWarning("Genre with ID: {Id} not found.", id);
                return NotFound("Genre not found.");
            }

            if (genre.SubGenres.Any())
            {
                _logger.LogWarning("Cannot delete genre with id {Id} because it has subgenres.", id);
                return BadRequest("Cannot delete genre with subgenres.");
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Genre with ID: {Id} deleted successfully.", id);
            return NoContent();
        }

      
        [HttpDelete("deleteSub/{name}")]
        public async Task<IActionResult> DeleteSubGenre(string name)
        {
            var subGenre = await _context.Genres
                .FirstOrDefaultAsync(g => g.GenreName == name);

            if (subGenre == null)
            {
                _logger.LogWarning("Subgenre with name: {Name} not found.", name);
                return NotFound("Subgenre not found.");
            }

            if (subGenre.SubGenres.Any())
            {
                _logger.LogWarning("Cannot delete subgenre with name {Name} because it has subgenres.", name);
                return BadRequest("Cannot delete subgenre with subgenres.");
            }

            _context.Genres.Remove(subGenre);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subgenre with name: {Name} deleted successfully.", name);
            return NoContent();
        }
    }
}
