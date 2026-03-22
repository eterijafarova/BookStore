using AutoMapper;
using BookShop.Data.Models;
using BookShop.ADMIN.DTOs.GenreDto;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Create Genre
      
        CreateMap<CreateSubGenreDto, Genre>()
            .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Name));

        // Entity → DTO
        CreateMap<Genre, GenreDto>()
            .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.GenreName))
            .ForMember(dest => dest.SubGenres, opt => opt.Ignore())
            .ForMember(dest => dest.Books, opt => opt.Ignore());
        
        CreateMap<Genre, GenreResponseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GenreName))
            .ForMember(dest => dest.ParentGenreName,
                opt => opt.MapFrom(src => src.ParentGenre != null ? src.ParentGenre.GenreName : null));
    }
}