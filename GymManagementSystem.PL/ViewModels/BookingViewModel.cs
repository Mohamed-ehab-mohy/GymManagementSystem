using System;

namespace GymManagementSystem.PL.ViewModels;

public class BookingViewModel
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = "";
    public int ClassSessionId { get; set; }
    public string SessionName { get; set; } = "";
    public DateTime BookingDate { get; set; }
    public DateTime? ScheduleTime { get; set; }
    public bool IsAttended { get; set; }
    public DateTime? CheckedInAt { get; set; }
}
