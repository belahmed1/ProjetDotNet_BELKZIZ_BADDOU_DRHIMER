using AutoMapper;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;

namespace Gauniv.WebServer.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping between Game and GameDto (including nested Categories)
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .ReverseMap();

            // Mapping between Category and CategoryDto
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
