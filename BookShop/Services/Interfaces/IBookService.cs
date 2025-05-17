using BookShop.ADMIN.DTOs;
using BookShop.Shared.DTO.Response;

namespace BookShop.Services.Interfaces
{
    public interface IBookService
    {
        Task<BookDto> CreateBookAsync(CreateBookDto dto);
        Task<BookDto> GetBookAsync(Guid id);
        Task<IEnumerable<BookDto>> GetBooksAsync(int page = 1, int pageSize = 20);
        Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto dto);
        Task<bool> DeleteBookAsync(Guid id);
        Task<bool> UpdateStockAsync(Guid bookId, int newStock);
        Task<bool> UpdatePriceAsync(Guid bookId, decimal newPrice);
    }
}