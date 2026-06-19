using System;

namespace GymManagementSystem.Domain;

public class Booking : BaseEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int ClassSessionId { get; set; }
    public ClassSession ClassSession { get; set; } = null!;

    public DateTime BookingDate { get; set; }
    public bool IsAttended { get; set; }
    public DateTime? CheckedInAt { get; set; }
}
