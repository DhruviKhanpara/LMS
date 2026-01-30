using AutoMapper;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Books;
using LMS.Application.Services.Constants;
using LMS.Common.Helpers;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Services.Mappings;

public class BookMapperProfile : Profile
{
    public BookMapperProfile()
    {
        CreateMap<string?, IFormFile?>()
            .ConvertUsing<StringToIFormFileConverter>();

        CreateMap<Books, GetBookDto>()
            .ForMember(dest => dest.GenreName, src => src.MapFrom(act => act.Genre.Name))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"))
            .ForMember(dest => dest.PublishAt, src => src.MapFrom(act => act.PublishAt))
            .ForMember(dest => dest.TotalCopies, src => src.MapFrom(act => act.TotalCopies))
            .ForMember(dest => dest.Price, src => src.MapFrom(act => act.Price))
            .ForMember(dest => dest.BookFiles, src => src.MapFrom(act => act.BookFileMappings.Where(i => i.IsActive)))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Books, ExportBookDto>()
            .ForMember(dest => dest.GenreName, src => src.MapFrom(act => act.Genre.Name))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.PublishAt, src => src.MapFrom(act => act.PublishAt))
            .ForMember(dest => dest.TotalCopies, src => src.MapFrom(act => act.TotalCopies))
            .ForMember(dest => dest.Price, src => src.MapFrom(act => act.Price))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Books, GetBookInfoDto>()
            .ForMember(dest => dest.GenreName, src => src.MapFrom(act => act.Genre.Name))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"))
            .ForMember(dest => dest.PublishAt, src => src.MapFrom(act => act.PublishAt))
            .ForMember(dest => dest.TotalCopies, src => src.MapFrom(act => act.TotalCopies))
            .ForMember(dest => dest.Price, src => src.MapFrom(act => act.Price))
            .ForMember(dest => dest.BookFiles, src => src.MapFrom(act => act.BookFileMappings.Where(i => i.IsActive)))
            .ForMember(dest => dest.TotalBorrowing, src => src.MapFrom(act => act.Transections.LongCount(i => i.IsActive && i.StatusId != (long)TransectionStatusEnum.Cancelled)))
            .ForMember(dest => dest.TotalReservation, src => src.MapFrom(act => act.Reservations.LongCount(i => i.IsActive)))
            .ForMember(dest => dest.ActiveReservation, src => src.MapFrom(act => act.Reservations.LongCount(i => i.IsActive && i.IsAllocated && StatusGroups.Reservation.Active.Contains(i.StatusId))))
            .ForMember(dest => dest.RecentActivitiesByPeople, src => src.MapFrom(act =>
                act.Transections.Where(i => i.IsActive && StatusGroups.Transaction.Active.Contains(i.StatusId))
                .Select(t => new
                {
                    ProfilePhoto = t.User.ProfilePhoto,
                    Username = t.User.FirstName + " " + t.User.LastName,
                    Label = t.Status.Label,
                    ActivityDate = t.BorrowDate
                })
                .Union(act.Reservations.Where(i => i.IsActive && i.IsAllocated && StatusGroups.Reservation.Active.Contains(i.StatusId))
                .Select(r => new
                {
                    ProfilePhoto = r.User.ProfilePhoto,
                    Username = r.User.FirstName + " " + r.User.LastName,
                    Label = r.Status.Label,
                    ActivityDate = r.ReservationDate
                }))
                .OrderByDescending(x => x.ActivityDate)
                .Take(5)
                .Select(x => new RecentActivityByPeople
                {
                    profilePhoto = !string.IsNullOrWhiteSpace(x.ProfilePhoto)
                        ? "/" + x.ProfilePhoto
                        : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s",
                    Username = x.Username,
                    Activity = x.Label,
                    ActivityDate = x.ActivityDate
                })
            ));

        CreateMap<Books, UpdateBookDto>()
            .ForMember(dest => dest.PublishAt, src => src.MapFrom(act => act.PublishAt))
            .ForMember(dest => dest.TotalCopies, src => src.MapFrom(act => act.TotalCopies))
            .ForMember(dest => dest.Price, src => src.MapFrom(act => act.Price))
            .ForMember(dest => dest.BookFiles, src => src.MapFrom(act => act.BookFileMappings.Where(i => i.IsActive)))
            .ForMember(dest => dest.CoverPage, opt => opt.Ignore())
            .ForMember(dest => dest.BookPreview, opt => opt.Ignore());

        CreateMap<UpdateBookDto, Books>()
            .ForMember(dest => dest.TotalCopies, src => src.MapFrom(act => act.TotalCopies ?? 0))
            .ForMember(dest => dest.Price, src => src.MapFrom(act => act.Price ?? 0))
            .ForMember(dest => dest.AvailableCopies, src => src.MapFrom(act => act.AvailableCopies ?? act.TotalCopies ?? 0))
            .ForMember(dest => dest.PublishAt, src => src.MapFrom(act => act.PublishAt ?? DateTimeOffset.UtcNow));

        CreateMap<AddBookDto, Books>()
            .ForMember(dest => dest.AvailableCopies, src => src.MapFrom(act => act.TotalCopies))
            .ForMember(dest => dest.TotalCopies, src => src.MapFrom(act => act.TotalCopies ?? 0))
            .ForMember(dest => dest.Price, src => src.MapFrom(act => act.Price ?? 0))
            .ForMember(dest => dest.PublishAt, src => src.MapFrom(act => act.PublishAt ?? DateTimeOffset.UtcNow));

        CreateMap<Books, SelectListItem>()
            .ForMember(dest => dest.Text, src => src.MapFrom(act => act.Title))
            .ForMember(dest => dest.Value, src => src.MapFrom(act => act.Id));
    }
}
