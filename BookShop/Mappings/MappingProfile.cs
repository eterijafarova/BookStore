using AutoMapper;
using BookShop.ADMIN.DTOs;
using BookShop.ADMIN.DTOs.PublisherDto;
using BookShop.Data.Models;

namespace BookShop.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // -------- Publisher Mapping --------
            CreateMap<Publisher, PublisherDto>().ReverseMap();
            CreateMap<CreatePublisherDto, Publisher>();
            CreateMap<UpdatePublisherDto, Publisher>();

            // -------- Book Mapping --------
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.GenreName))
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name));

            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
        }
    }
}