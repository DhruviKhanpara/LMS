using AutoMapper;
using LMS.Application.Contracts.DTOs.User;
using LMS.Application.Services.Constants;
using LMS.Common.Helpers;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Services.Mappings;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<string?, IFormFile?>()
            .ConvertUsing<StringToIFormFileConverter>();

        CreateMap<User, GetUserDto>()
            .ForMember(dest => dest.Role, src => src.MapFrom(act => act.Role.Label))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive))
            .ForMember(dest => dest.ProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.ProfilePhoto) ? "/" + act.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.HasPenalty, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.Any(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : false))
            .ForMember(dest => dest.HasOccupiedBook, src => src.MapFrom(act => act.Transections != null ? act.Transections.Any(x => x.IsActive && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)) : false));

        CreateMap<User, ExportUserDto>()
            .ForMember(dest => dest.Role, src => src.MapFrom(act => act.Role.Label))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive))
            .ForMember(dest => dest.HasPenalty, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.Any(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : false))
            .ForMember(dest => dest.HasOccupiedBook, src => src.MapFrom(act => act.Transections != null ? act.Transections.Any(x => x.IsActive && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)) : false));

        CreateMap<User, GetUserDetailDto>()
            .ForMember(dest => dest.Role, src => src.MapFrom(act => act.Role.Label))
            .ForMember(dest => dest.ProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.ProfilePhoto) ? "/" + act.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.HasPenalty, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.Any(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : false))
            .ForMember(dest => dest.HasOccupiedBook, src => src.MapFrom(act => act.Transections != null ? act.Transections.Any(x => x.IsActive && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)) : false))
            .ForMember(dest => dest.HasActiveReservation, src => src.MapFrom(act => act.Reservations != null ? act.Reservations.Any(x => x.IsActive && !StatusGroups.Reservation.Finalized.Contains(x.StatusId)) : false))
            .ForMember(dest => dest.HasActiveMembership, src => src.MapFrom(act => act.UserMemberships != null ? act.UserMemberships.Any(x => x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow) : false));

        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.Role, src => src.MapFrom(act => act.Role.Label))
            .ForMember(dest => dest.ProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.ProfilePhoto) ? "/" + act.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.PenaltyCount, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.LongCount(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : 0))
            .ForMember(dest => dest.OccupiedBookCount, src => src.MapFrom(act => act.Transections != null ? act.Transections.LongCount(x => x.IsActive && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)) : 0))
            .ForMember(dest => dest.ActiveReservationCount, src => src.MapFrom(act => act.Reservations != null ? act.Reservations.LongCount(x => x.IsActive && !StatusGroups.Reservation.Finalized.Contains(x.StatusId)) : 0))
            .ForMember(dest => dest.ActiveMembershipCount, src => src.MapFrom(act => act.UserMemberships != null ? act.UserMemberships.LongCount(x => x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow) : 0))
            .ForMember(dest => dest.TotalPenaltyCount, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.LongCount(x => x.IsActive) : 0))
            .ForMember(dest => dest.TotalOccupiedBookCount, src => src.MapFrom(act => act.Transections != null ? act.Transections.LongCount(x => x.IsActive) : 0))
            .ForMember(dest => dest.TotalReservationCount, src => src.MapFrom(act => act.Reservations != null ? act.Reservations.LongCount(x => x.IsActive) : 0))
            .ForMember(dest => dest.TotalMembershipCount, src => src.MapFrom(act => act.UserMemberships != null ? act.UserMemberships.LongCount(x => x.IsActive) : 0));

        CreateMap<User, ProfilePhotoUpdateDto>()
            .ForMember(dest => dest.ProfilePhoto, opt => opt.Ignore())
            .ForMember(dest => dest.ProfilePhotoPath, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.ProfilePhoto) ? "/" + act.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"));

        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.RoleId, src => src.Condition(act => act.RoleId != 0));

        CreateMap<User, UpdateUserDto>()
            .ForMember(dest => dest.ProfilePhoto, opt => opt.Ignore())
            .ForMember(dest => dest.ProfilePhotoPath, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.ProfilePhoto) ? "/" + act.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"));

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.LibraryCardNumber, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.ProfilePhoto, opt => opt.Ignore());

        CreateMap<User, SelectListItem>()
            .ForMember(dest => dest.Text, src => src.MapFrom(act => $"{act.FirstName} {act.MiddleName ?? ""} {act.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.Value, src => src.MapFrom(act => act.Id));
    }
}
