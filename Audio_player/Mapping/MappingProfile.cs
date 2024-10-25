using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;
using AutoMapper;

namespace Audio_player.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Genre, GenreDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CoverPath, opt => opt.MapFrom(src => src.CoverPath));
    }
}
