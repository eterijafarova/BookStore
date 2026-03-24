using BookShop.Data.Models;
using BookShop.ADMIN.DTOs;
using BookShop.Data.Contexts;
using BookShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BookShop.Shared.DTO.Response;

namespace BookShop.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly LibraryContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public BookService(LibraryContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title cannot be empty.");

            if (dto.Price < 0)
                throw new ArgumentException("Price cannot be negative.");

            if (dto.Stock < 0)
                throw new ArgumentException("Stock cannot be negative.");

            var genreExists = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!genreExists)
                throw new Exception("Genre not found");

            if (dto.PublisherId != null)
            {
                var publisherExists = await _context.Publishers.AnyAsync(p => p.Id == dto.PublisherId);
                if (!publisherExists)
                    throw new Exception("Publisher not found");
            }

            var duplicate = await _context.Books.AnyAsync(b => b.Title == dto.Title);
            if (duplicate)
                throw new Exception("Book already exists");

            string? imageUrl = null;

            if (dto.Image != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image);
            }

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Author = dto.Author,
                Price = dto.Price,
                Stock = dto.Stock,
                Description = dto.Description,
                ImageUrl = imageUrl,
                GenreId = dto.GenreId,
                PublisherId = dto.PublisherId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                Stock = book.Stock,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                GenreName = null,
                PublisherName = null
            };
        }

        public async Task<BookDto> GetBookAsync(Guid id)
        {
            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                Stock = book.Stock,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                GenreName = book.Genre?.GenreName,
                PublisherName = book.Publisher?.Name
            };
        }

        public async Task<PaginatedResponse<BookDto>> GetBooksAsync(int page = 1, int pageSize = 20)
        {
            var query = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Select(book => new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Price = book.Price,
                    Stock = book.Stock,
                    Description = book.Description,
                    ImageUrl = book.ImageUrl,
                    GenreName = book.Genre.GenreName,
                    PublisherName = book.Publisher.Name
                })
                .AsQueryable();

            return await PaginatedResponse<BookDto>.CreateAsync(query, page, pageSize);
        }
        public async Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return null;

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Price = dto.Price;
            book.Stock = dto.Stock;
            book.Description = dto.Description;
            book.GenreId = dto.GenreId;
            book.PublisherId = dto.PublisherId;
            
            if (dto.Image != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image);
                book.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                Stock = book.Stock,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                GenreName = null,
                PublisherName = null
            };
        }

        public async Task<bool> UpdateStockAsync(Guid bookId, int newStock)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            book.Stock = newStock;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePriceAsync(Guid bookId, decimal newPrice)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            book.Price = newPrice;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            var orders = _context.OrderItems.Where(oi => oi.BookId == id);
            _context.OrderItems.RemoveRange(orders);

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}