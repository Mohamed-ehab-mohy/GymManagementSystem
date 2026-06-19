namespace GymManagementSystem.BLL.DTOs;

public class MemberDashboardDto
{
    public string MemberName { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public DateTime JoinDate { get; set; }

    public bool HasActiveMembership { get; set; }
    public string? PlanName { get; set; }
    public DateTime? MembershipStart { get; set; }
    public DateTime? MembershipEnd { get; set; }
    public int DaysRemaining { get; set; }

    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Bmi { get; set; }
    public string? BmiCategory { get; set; }

    public List<UpcomingSessionDto> UpcomingSessions { get; set; } = [];
    public int TotalBooked { get; set; }
    public int TotalAttended { get; set; }
    public double AttendanceRate { get; set; }
}

public class UpcomingSessionDto
{
    public string SessionName { get; set; } = string.Empty;
    public DateTime ScheduleTime { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}
