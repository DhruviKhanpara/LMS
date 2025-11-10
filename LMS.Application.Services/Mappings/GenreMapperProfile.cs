using AutoMapper;
using LMS.Application.Contracts.DTOs.Genre;
using LMS.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Services.Mappings;

public class GenreMapperProfile : Profile
{
    public GenreMapperProfile()
    {
        CreateMap<Genre, GetGenreDto>()
            .ForMember(dest => dest.TotalActiveBooks, src => src.MapFrom(act => act.Books.LongCount(x => x.IsActive)))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Genre, ExportGenreDto>()
            .ForMember(dest => dest.TotalActiveBooks, src => src.MapFrom(act => act.Books.LongCount(x => x.IsActive)))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Genre, SelectListItem>()
            .ForMember(dest => dest.Text, src => src.MapFrom(act => act.Name))
            .ForMember(dest => dest.Value, src => src.MapFrom(act => act.Id));

        CreateMap<Genre, UpdateGenreDto>();

        CreateMap<UpdateGenreDto, Genre>();

        CreateMap<AddGenreDto, Genre>();
    }
}
