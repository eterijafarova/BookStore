using AutoMapper;
using BookShop.ADMIN.DTOs;
using BookShop.Data.Models;
using BookShop.ADMIN.DTOs.GenreDto;
using static BookShop.ADMIN.DTOs.PublisherDto;

namespace BookShop.Mappings
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            // Маппинг из Genre в GenreDto
            CreateMap<Genre, GenreDto>()
                .ForMember(dest => dest.SubGenres, opt => opt.MapFrom(src => src.SubGenres));

            // Маппинг из CreateGenreDto в Genre
            CreateMap<CreateGenreDto, Genre>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Name));

            // Маппинг из CreateSubGenreDto в Genre
            CreateMap<CreateSubGenreDto, Genre>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Name));

            // Добавляем маппинг из Publisher в PublisherDto
            CreateMap<Publisher, PublisherDto>()
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));
        }
    }
}