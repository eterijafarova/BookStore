using BookShop.ADMIN.DTOs;
using BookShop.ADMIN.DTOs.PublisherDto;

namespace BookShop.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDto>> GetAllAsync();
        Task<PublisherDto?> GetByIdAsync(int id);
        Task<PublisherDto?> CreateAsync(CreatePublisherDto dto);
        Task<bool> UpdateAsync(int id, UpdatePublisherDto dto);
        Task<bool> DeleteAsync(int id);
    }
}