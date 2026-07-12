using System;

namespace GymManagementSystem.Domain;

public class Payment : BaseEntity
{
    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public int? MembershipId { get; set; }
    public Membership? Membership { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Status { get; set; } = "Pending";
    public string? PaymobOrderId { get; set; }
    public string? PaymobTransactionId { get; set; }

}
