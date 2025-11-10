using AutoMapper;
using LMS.Application.Contracts.DTOs.Log;
using LMS.Common.Helpers;
using LMS.Core.Entities.LogEntities;

namespace LMS.Application.Services.Mappings;

public class LogMapperProfile : Profile
{
    public LogMapperProfile()
    {
        CreateMap<BooksLog, BooksLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.GenreName, src => src.MapFrom(act => act.Genre != null ? act.Genre.Name : null))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status != null ? act.Status.Label : null))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color : null))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color + "29" : null))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<ConfigsLog, ConfigsLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<GenreLog, GenreLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<MembershipLog, MembershipLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<PenaltyLog, PenaltyLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => act.User != null ? $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim() : null))
            .ForMember(dest => dest.TransectionDueDate, src => src.MapFrom(act => act.Transection != null ? act.Transection.DueDate : (DateTimeOffset?)null))
            .ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status != null ? act.Status.Label : null))
            .ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color : null))
            .ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color + "29" : null))
            .ForMember(dest => dest.PenaltyTypeName, src => src.MapFrom(act => act.PenaltyType != null ? act.PenaltyType.Label : null))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<ReservationLog, ReservationLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
			.ForMember(dest => dest.UserName, src => src.MapFrom(act => act.User != null ? $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim() : null))
			.ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book != null ? act.Book.Title : null))
			.ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status != null ? act.Status.Label : null))
			.ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color : null))
			.ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color + "29" : null))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<TransectionLog, TransectionLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
			.ForMember(dest => dest.UserName, src => src.MapFrom(act => act.User != null ? $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim() : null))
			.ForMember(dest => dest.BookName, src => src.MapFrom(act => act.Book != null ? act.Book.Title : null))
			.ForMember(dest => dest.StatusLabel, src => src.MapFrom(act => act.Status != null ? act.Status.Label : null))
			.ForMember(dest => dest.StatusLabelColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color : null))
			.ForMember(dest => dest.StatusLabelBgColor, src => src.MapFrom(act => act.Status != null ? act.Status.Color + "29" : null))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<UserLog, UserLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.Role, src => src.MapFrom(act => act.Role != null ? act.Role.Label : null))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));

        CreateMap<UserMembershipMappingLog, UserMembershipMappingLogDto>()
            .ForMember(dest => dest.OperationColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeForeColor(act.Operation)))
            .ForMember(dest => dest.OperationBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetOperationTypeBgColor(act.Operation)))
            .ForMember(dest => dest.LogTypeColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeForeColor(act.LogType)))
            .ForMember(dest => dest.LogTypeBgColor, src => src.MapFrom(act => LogTablesColorHelper.GetLogTypeBgColor(act.LogType)))
            .ForMember(dest => dest.UserName, src => src.MapFrom(act => act.User != null ? $"{act.User.FirstName} {act.User.MiddleName ?? ""} {act.User.LastName ?? ""}".Trim() : null))
            .ForMember(dest => dest.MembershipType, src => src.MapFrom(act => act.Membership != null ? act.Membership.Type : null))
            .ForMember(dest => dest.PerformedBy, src => src.MapFrom(act => PerformByHelper.GetPerformBy(act.Operation, act.CreatedByUser != null ? $"{act.CreatedByUser.FirstName} {act.CreatedByUser.MiddleName ?? ""} {act.CreatedByUser.LastName ?? ""}".Trim() : null, act.ModifiedByUser != null ? $"{act.ModifiedByUser.FirstName} {act.ModifiedByUser.MiddleName ?? ""} {act.ModifiedByUser.LastName ?? ""}".Trim() : null, act.DeletedByUser != null ? $"{act.DeletedByUser.FirstName} {act.DeletedByUser.MiddleName ?? ""} {act.DeletedByUser.LastName ?? ""}".Trim() : null)));
    }
}
