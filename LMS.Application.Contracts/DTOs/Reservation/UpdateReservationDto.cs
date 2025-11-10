namespace LMS.Application.Contracts.DTOs.Reservation;

public class UpdateReservationDto : BaseReservationDto
{
    public long Id { get; set; }
    public bool IsAllocateAfterIsDefault { get; set; } = false;
    public bool IsCountTransferAllocation { get; set; } = true;
}
