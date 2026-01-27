using AutoMapper;
using LMS.Application.Contracts.DTOs.Home;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Common.Helpers;
using LMS.Core.Entities;

namespace LMS.Application.Services.Mappings;

public class PenaltyMapperProfile : Profile
{
    public PenaltyMapperProfile()
    {
        CreateMap<Penalty, GetPenaltyDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.UserProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.User.ProfilePhoto) ? FileService.ConvertToRelativePath(act.User.ProfilePhoto) : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.TransectionDueDate, src => src.MapFrom(act => act.Transection != null ? act.Transection.DueDate : (DateTimeOffset?)null))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"))
            .ForMember(dest => dest.PenaltyTypeName, src => src.MapFrom(act => act.PenaltyType.Label))
            .ForMember(dest => dest.PenaltyTypeInfo, src => src.MapFrom(act => act.PenaltyType.Description))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Penalty, ExportPenaltyDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.TransectionDueDate, src => src.MapFrom(act => act.Transection != null ? act.Transection.DueDate : (DateTimeOffset?)null))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.PenaltyTypeName, src => src.MapFrom(act => act.PenaltyType.Label))
            .ForMember(dest => dest.PenaltyTypeInfo, src => src.MapFrom(act => act.PenaltyType.Description))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Penalty, PenaltyData>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.UserProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.User.ProfilePhoto) ? FileService.ConvertToRelativePath(act.User.ProfilePhoto) : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.PenaltyTypeName, src => src.MapFrom(act => act.PenaltyType.Label));

        CreateMap<Penalty, UpdatePenaltyDto>()
            .ForMember(dest => dest.TransectionStatusId, opt => opt.Ignore())
            .ForMember(dest => dest.TransectionStatus, opt => opt.Ignore());

        CreateMap<UpdatePenaltyDto, Penalty>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Description, src => src.MapFrom(act => act.Description ?? "Other reasons"))
            .ForMember(dest => dest.Amount, src => src.MapFrom(act => act.Amount ?? 0));

        CreateMap<AddPenaltyDto, Penalty>()
            .ForMember(dest => dest.Description, src => src.MapFrom(act => act.Description ?? "Other reasons"))
            .ForMember(dest => dest.Amount, src => src.MapFrom(act => act.Amount ?? 0));
    }
}
