using BookShop.ADMIN.DTOs;
using BookShop.BlobStorage;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using BookShop.Shared.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.ADMIN.ControllersAdmin
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IBlobService _blobService;

        public BooksController(LibraryContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(int page = 1, int pageSize = 10, string search = "")
        {
            var query = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Where(b => b.Title.Contains(search) || b.Author.Contains(search));

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    Stock = b.Stock,
                    Description = b.Description,
                    ImageUrl = b.ImageUrl,
                    GenreName = b.Genre.GenreName,
                    GenreId = b.GenreId,
                    PublisherName = b.Publisher.Name
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookDto>> GetBookById(Guid id)
        {
            var b = await _context.Books
                .Include(x => x.Genre)
                .Include(x => x.Publisher)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (b == null) return NotFound();

            var dto = new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Price = b.Price,
                Stock = b.Stock,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                GenreName = b.Genre.GenreName,
                GenreId = b.GenreId,
                PublisherName = b.Publisher.Name
            };
            return Ok(dto);
        }

        // POST: api/books
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> CreateBook(
            [FromForm] BookCreateDto dto,
            IFormFile? imageFile)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Price = dto.Price,
                Stock = dto.Stock,
                Description = dto.Description,
                GenreId = dto.GenreId,
                PublisherId = dto.PublisherId
            };

            if (imageFile is not null)
            {
                book.ImageUrl = await _blobService.UploadFileAsync(imageFile);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        // PUT: api/books/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBook(
            Guid id,
            [FromBody] UpdateBookDto dto)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return NotFound(new { message = "Book not found" });

            existingBook.Title = dto.Title;
            existingBook.Author = dto.Author;
            existingBook.Price = dto.Price;
            existingBook.Stock = dto.Stock;
            existingBook.Description = dto.Description;
            existingBook.ImageUrl = dto.ImageUrl;
            existingBook.GenreId = dto.GenreId;
            existingBook.PublisherId = dto.PublisherId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(new { message = "Book not found" });

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
