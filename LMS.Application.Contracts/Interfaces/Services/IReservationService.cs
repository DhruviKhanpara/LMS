using LMS.Application.Contracts.DTOs.Reservation;
using LMS.Application.Contracts.DTOs;
using LMS.Core.Enums;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IReservationService
{
    Task<PaginatedResponseDto<T>> GetAllReservationAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null, long? bookId = null) where T : class;
    Task<PaginatedResponseDto<GetUserReservationDto>> GetUserReservationAsync(int? pageSize = null, int? pageNumber = null);
    Task<T> GetReservationByIdAsync<T>(long id) where T : class;
    Task<byte[]> ExportReservationData();
    Task ReserveBookforLoginUserAsync(long bookId);
    Task AddReservationAsync(AddReservationDto reservation);
    Task UpdateReservationAsync(UpdateReservationDto reservation);
    Task RemoveBookReservationAsync(long bookId);
    Task RemoveUserReservationAsync(long userId);
    Task ReservationActionsAsync(long id, ReservationActionEnum reservationAction);
    Task PermanentDeleteReservationAsync(long id);
    Task ReallocateExpiredAllocationToReservationAsync(bool forLogin = false, bool notifyUser = false);
    Task AllocateBookToReservation(bool forLogin = false, bool notifyUser = false);
    Task DispatchEmailForReservationAllocation(List<long>? ids = null);
}
