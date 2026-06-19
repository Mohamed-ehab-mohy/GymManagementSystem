using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Interfaces;

public interface IPurgeService
{
    Task<int> PurgeAsync(int olderThanDays = 30);
}
