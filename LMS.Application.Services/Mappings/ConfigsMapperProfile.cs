using AutoMapper;
using LMS.Application.Contracts.DTOs.Configs;
using LMS.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Services.Mappings;

public class ConfigsMapperProfile : Profile
{
    public ConfigsMapperProfile()
    {
        CreateMap<Configs, GetConfigsValueDto>();
        
        CreateMap<Configs, ExportConfigDto>();

        CreateMap<Configs, GetConfigsDto>()
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Configs, SelectListItem>()
            .ForMember(dest => dest.Text, src => src.MapFrom(act => act.KeyName))
            .ForMember(dest => dest.Value, src => src.MapFrom(act => act.Id));

        CreateMap<Configs, UpdateConfigsDto>();

        CreateMap<UpdateConfigsDto, Configs>()
            .ForMember(src => src.KeyName, opt => opt.Ignore())
            .ForMember(src => src.CreatedBy, opt => opt.Ignore())
            .ForMember(src => src.ModifiedBy, opt => opt.Ignore());

    }
}
