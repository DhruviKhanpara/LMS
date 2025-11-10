using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.UserMembershipMapping;

public class ExportUserMembershipDto
{
    public long Id { get; set; }
    [Display(Name = "User Name")]
    public string UserName { get; set; } = null!;
    [Display(Name = "Membership type")]
    public string MembershipType { get; set; } = null!;
    [Display(Name = "Membership description")]
    public string MembershipDescription { get; set; } = null!;
    [Display(Name = "Effective start date")]
    public DateTimeOffset EffectiveStartDate { get; set; }
    [Display(Name = "Expiration date")]
    public DateTimeOffset ExpirationDate { get; set; }
    [Display(Name = "Borrow limit")]
    public long BorrowLimit { get; set; }
    [Display(Name = "Reservation limit")]
    public long ReservationLimit { get; set; }
    [Display(Name = "Membership cost")]
    public decimal MembershipCost { get; set; }
    public decimal Discount { get; set; }
    [Display(Name = "Paid amount")]
    public decimal? PaidAmount { get; set; }
}
