using AutoMapper;
using LMS.Application.Contracts.DTOs.BookFileMapping;
using LMS.Core.Entities;

namespace LMS.Application.Services.Mappings;

public class BookFileMappingMapperProfile : Profile
{
    public BookFileMappingMapperProfile()
    {
        CreateMap<BookFileMapping, GetBookFileMappingDto>()
            .ForMember(dest => dest.fileLocation, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.fileLocation) ? "/" + act.fileLocation : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"));
    }
}
