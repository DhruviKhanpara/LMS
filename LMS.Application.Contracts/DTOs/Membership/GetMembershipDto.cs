using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Membership;

public class GetMembershipDto
{
    [JsonProperty("id")]
    public long Id { get; set; }
    [JsonProperty("type")]
    [Display(Name = "Membership type")]
    public string Type { get; set; } = null!;
    [JsonProperty("description")]
    public string Description { get; set; } = null!;
    [JsonProperty("borrowLimit")]
    [Display(Name = "Borrow limit")]
    public long BorrowLimit { get; set; }
    [JsonProperty("reservationLimit")]
    [Display(Name = "Reservation limit")]
    public long ReservationLimit { get; set; }
    [JsonProperty("duration")]
    [Display(Name = "Duration in Days")]
    public long Duration { get; set; }
    [JsonProperty("cost")]
    public decimal Cost { get; set; }
    [JsonProperty("discount")]
    public decimal Discount { get; set; }
    public bool IsRemoved { get; set; }
}
