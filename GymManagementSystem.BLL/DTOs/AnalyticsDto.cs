namespace GymManagementSystem.BLL.DTOs;

public class AnalyticsDto
{
    public decimal MonthlyRevenue { get; set; }
    public int ActiveMemberCount { get; set; }
    public int TodaySessionsCount { get; set; }
    public int ExpiringSoonCount { get; set; }
    public List<MonthlyCountDto> NewMembersPerMonth { get; set; } = [];
    public List<MonthlyRevenueDto> RevenuePerMonth { get; set; } = [];
    public List<SessionStatDto> TopSessions { get; set; } = [];
    public List<PlanDistributionDto> MembersByPlan { get; set; } = [];
}

public class MonthlyCountDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class SessionStatDto
{
    public string SessionName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public int Capacity { get; set; }
}

public class PlanDistributionDto
{
    public string PlanName { get; set; } = string.Empty;
    public int MemberCount { get; set; }
}
