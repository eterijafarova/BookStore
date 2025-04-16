using AutoMapper;
using BookShop.ADMIN.DTOs;
using BookShop.ADMIN.DTOs.PublisherDto;
using BookShop.Data.Contexts;
using BookShop.Data.Models;
using BookShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Services.Implementations
{
    public class PublisherService : IPublisherService
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public PublisherService(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PublisherDto>> GetAllAsync()
        {
            var publishers = await _context.Publishers.ToListAsync();
            return _mapper.Map<List<PublisherDto>>(publishers);
        }

        public async Task<PublisherDto?> GetByIdAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            return publisher == null ? null : _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<PublisherDto?> CreateAsync(CreatePublisherDto dto)
        {
            var exists = await _context.Publishers.AnyAsync(p => p.Name == dto.Name);
            if (exists) return null;

            var publisher = _mapper.Map<Publisher>(dto);
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePublisherDto dto)
        {
            if (id != dto.Id) return false;

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return false;

            _mapper.Map(dto, publisher);
            _context.Publishers.Update(publisher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return false;

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
