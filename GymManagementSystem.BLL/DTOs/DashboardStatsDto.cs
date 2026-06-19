namespace GymManagementSystem.BLL.DTOs;

public class DashboardStatsDto
{
    public int TotalMembers { get; set; }
    public int TotalTrainers { get; set; }
    public int ActivePlans { get; set; }
    public int TodaySessions { get; set; }
    public int TodayAttendance { get; set; }
    public int ExpiringMemberships { get; set; }
    public int TotalPlans { get; set; }
}
