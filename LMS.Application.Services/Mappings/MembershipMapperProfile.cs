using AutoMapper;
using LMS.Application.Contracts.DTOs.Membership;
using LMS.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Services.Mappings;

public class MembershipMapperProfile : Profile
{
    public MembershipMapperProfile()
    {
        CreateMap<Membership, GetMembershipDto>()
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Membership, ExportMembershipDto>()
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Membership, SelectListItem>()
            .ForMember(dest => dest.Text, src => src.MapFrom(act => act.Type))
            .ForMember(dest => dest.Value, src => src.MapFrom(act => act.Id));

        CreateMap<AddMembershipDto, Membership>()
            .ForMember(dest => dest.BorrowLimit, src => src.MapFrom(act => act.BorrowLimit ?? 0))
            .ForMember(dest => dest.ReservationLimit, src => src.MapFrom(act => act.ReservationLimit ?? 0))
            .ForMember(dest => dest.Duration, src => src.MapFrom(act => act.Duration ?? 0))
            .ForMember(dest => dest.Cost, src => src.MapFrom(act => act.Cost ?? 0))
            .ForMember(dest => dest.Discount, src => src.MapFrom(act => act.Discount ?? 0));

        CreateMap<UpdateMembershipDto, Membership>()
            .ForMember(dest => dest.BorrowLimit, src => src.MapFrom(act => act.BorrowLimit ?? 0))
            .ForMember(dest => dest.ReservationLimit, src => src.MapFrom(act => act.ReservationLimit ?? 0))
            .ForMember(dest => dest.Duration, src => src.MapFrom(act => act.Duration ?? 0))
            .ForMember(dest => dest.Cost, src => src.MapFrom(act => act.Cost ?? 0))
            .ForMember(dest => dest.Discount, src => src.MapFrom(act => act.Discount ?? 0));

        CreateMap<Membership, UpdateMembershipDto>();
    }
}
