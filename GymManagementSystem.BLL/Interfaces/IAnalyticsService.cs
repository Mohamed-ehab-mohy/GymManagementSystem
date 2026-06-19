using System.Threading.Tasks;
using GymManagementSystem.BLL.DTOs;

namespace GymManagementSystem.BLL.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsDto> GetAnalyticsAsync(int months = 12);
}
