using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;

namespace GymManagementSystem.BLL.Interfaces;

public interface ITrainerService
{
    Task<IEnumerable<Trainer>> GetAllTrainersAsync();
    Task<Trainer?> GetTrainerByIdAsync(int id);
    Task<PagedResult<Trainer>> GetPagedTrainersAsync(int page, int pageSize, string? search = null, string? sortBy = null, bool ascending = true);
    Task AddTrainerAsync(Trainer trainer);
    Task UpdateTrainerAsync(Trainer trainer);
    Task<(bool Success, string Message)> DeleteTrainerAsync(int id);
}
