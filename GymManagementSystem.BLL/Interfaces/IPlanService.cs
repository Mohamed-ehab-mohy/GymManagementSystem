using GymManagementSystem.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<Plan>> GetAllPlansAsync();
    Task<Plan?> GetPlanByIdAsync(int id);
    Task CreatePlanAsync(Plan plan);
    Task UpdatePlanAsync(Plan plan);
    Task DeletePlanAsync(int id);
}
