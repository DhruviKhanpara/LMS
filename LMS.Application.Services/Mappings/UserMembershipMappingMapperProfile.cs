using AutoMapper;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;
using LMS.Core.Entities;
using LMS.Core.Enums;

namespace LMS.Application.Services.Mappings;

public class UserMembershipMappingMapperProfile : Profile
{
    public UserMembershipMappingMapperProfile()
    {
        CreateMap<UserMembershipMapping, GetUserMembershipDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim()));

        CreateMap<UserMembershipMapping, ExportUserMembershipDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim()));

        CreateMap<UserMembershipMapping, GetUserMembersipListDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act =>
                act.EffectiveStartDate <= DateTimeOffset.UtcNow && act.ExpirationDate >= DateTimeOffset.UtcNow ? nameof(UserMembershipStatusEnum.Active) :
                act.EffectiveStartDate > DateTimeOffset.UtcNow ? nameof(UserMembershipStatusEnum.Upcoming) :
                act.ExpirationDate < DateTimeOffset.UtcNow ? nameof(UserMembershipStatusEnum.Expired) :
                nameof(UserMembershipStatusEnum.UnCategorized)))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act =>
                act.EffectiveStartDate <= DateTimeOffset.UtcNow && act.ExpirationDate >= DateTimeOffset.UtcNow ? "#28A745" :
                act.EffectiveStartDate > DateTimeOffset.UtcNow ? "#007BFF" :
                act.ExpirationDate < DateTimeOffset.UtcNow ? "#DC3545" :
                "#6C757D"))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act =>
                act.EffectiveStartDate <= DateTimeOffset.UtcNow && act.ExpirationDate >= DateTimeOffset.UtcNow ? "#28A74529" :
                act.EffectiveStartDate > DateTimeOffset.UtcNow ? "#007BFF29" :
                act.ExpirationDate < DateTimeOffset.UtcNow ? "#DC354529" :
                "#6C757D29"));

        CreateMap<UserMembershipMapping, UpdateUserMembershipDto>()
            .ForMember(dest => dest.UserPassword, opt => opt.Ignore());

        CreateMap<UpdateUserMembershipDto, UserMembershipMapping>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
