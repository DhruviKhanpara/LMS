using AutoMapper;
using LMS.Application.Contracts.DTOs.Home;
using LMS.Application.Contracts.DTOs.Transection;
using LMS.Core.Entities;
using LMS.Core.Enums;

namespace LMS.Application.Services.Mappings;

public class TransectionMapperProfile : Profile
{
    public TransectionMapperProfile()
    {
        CreateMap<Transection, GetTransectionDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}"))
            .ForMember(dest => dest.UserProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.User.ProfilePhoto) ? "/" + act.User.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"))
            .ForMember(dest => dest.HasPenalty, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.Any(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : false))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Transection, ExportTransectionDto>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}"))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.HasPenalty, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.Any(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : false))
            .ForMember(dest => dest.IsRemoved, src => src.MapFrom(act => !act.IsActive));

        CreateMap<Transection, GetUserTransectionDto>()
            .ForMember(dest => dest.BookCoverPage, opt => opt.MapFrom(act =>
                act.Book.BookFileMappings
                    .Where(m => m.IsActive && m.Label.ToLower() == nameof(BookFileTypeEnum.CoverPage).ToLower())
                    .Select(m => !string.IsNullOrWhiteSpace(m.fileLocation)
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
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"))
            .ForMember(dest => dest.HasPenalty, src => src.MapFrom(act => act.Penalties != null ? act.Penalties.Any(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid) : false));

        CreateMap<Transection, RecentCheckOuts>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}"))
            .ForMember(dest => dest.UserProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.User.ProfilePhoto) ? "/" + act.User.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.BookAuthor, src => src.MapFrom(act => act.Book.Author))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"));

        CreateMap<Transection, OverdueCheckOuts>()
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => $"{act.User.FirstName} {act.User.MiddleName ?? ""}".Trim() + $" {act.User.LastName ?? ""}"))
            .ForMember(dest => dest.UserProfilePhoto, src => src.MapFrom(act => !string.IsNullOrWhiteSpace(act.User.ProfilePhoto) ? "/" + act.User.ProfilePhoto : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s"))
            .ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book.Title))
            .ForMember(dest => dest.OverdueDays, src => src.MapFrom(act => (DateTimeOffset.UtcNow - act.DueDate).Days))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status.Label))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status.Color))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status.Color + "29"));

        CreateMap<AddTransectionDto, Transection>()
            .ForMember(dest => dest.DueDate, src => src.MapFrom(act => act.DueDate ?? act.BorrowDate));

        CreateMap<Transection, UpdateTransectionDto>()
            .ForMember(dest => dest.DueDate, src => src.MapFrom(act => act.DueDate));

        CreateMap<UpdateTransectionDto, Transection>()
            .ForMember(dest => dest.DueDate, src => src.MapFrom(act => act.DueDate ?? act.BorrowDate))
            .ForMember(dest => dest.UserId, src => src.Ignore())
            .ForMember(dest => dest.BookId, src => src.Ignore());
    }
}
