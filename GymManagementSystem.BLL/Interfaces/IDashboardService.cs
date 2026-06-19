using GymManagementSystem.BLL.DTOs;

namespace GymManagementSystem.BLL.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
