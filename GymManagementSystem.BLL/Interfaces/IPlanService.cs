using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.Domain;

namespace GymManagementSystem.BLL.Interfaces;

public interface IPlanService
{
    Task<IEnumerable<Plan>> GetAllPlansAsync();
    Task<Plan?> GetPlanByIdAsync(int id);
    Task CreatePlanAsync(Plan plan);
    Task UpdatePlanAsync(Plan plan);
    Task DeletePlanAsync(int id);
    Task<(bool Success, string Message)> ToggleActiveAsync(int id);
}
