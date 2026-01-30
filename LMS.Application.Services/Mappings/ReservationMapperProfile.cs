using AutoMapper;
using LMS.Application.Contracts.DTOs.Reservation;
using LMS.Core.Entities;
using LMS.Core.Enums;

namespace LMS.Application.Services.Mappings;

public class ReservationMapperProfile : Profile
{
    public ReservationMapperProfile()
    {
        CreateMap<Reservation, GetReservationDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.UserProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.User.ProfilePhoto) ? "/" + act.User.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Reservation, ExportReservationDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}".Trim()))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Reservation, GetUserReservationDto>()
            .ForMember(dest => dest.BookCoverPage, opt => opt.MapFrom(act =>
                act.Book.BookFileMappings
                    .Where(m => m.IsActive && m.Label.ToLower() == nameof(BookFileTypeEnum.CoverPage).ToLower())
                    .Select(m => string.IsNullOrWhiteSpace(m.fileLocation)
                        ? "/" + m.fileLocation
                        : null)
                    .FirstOrDefault()
                    ?? "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"
            ))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.BookAuthor, src => src.MapFrom(act => act.Book.Author))
            .ForMember(dest => dest.BookAvailableCopies, src => src.MapFrom(act => act.Book.AvailableCopies))
            .ForMember(dest => dest.BookTotalCopies, src => src.MapFrom(act => act.Book.TotalCopies))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"));

        CreateMap<AddReservationDto, Reservation>()
            .ForMember(dest => dest.AllocateAfter, src => src.MapFrom(act => act.AllocateAfter ?? act.ReservationDate));

        CreateMap<Reservation, UpdateReservationDto>()
            .ForMember(dest => dest.AllocateAfter, src => src.MapFrom(act => act.AllocateAfter));

        CreateMap<UpdateReservationDto, Reservation>()
            .ForMember(dest => dest.AllocateAfter, src => src.MapFrom(act => act.AllocateAfter ?? act.ReservationDate))
            .ForMember(dest => dest.UserId, src => src.Ignore())
            .ForMember(dest => dest.BookId, src => src.Ignore());
    }
}
