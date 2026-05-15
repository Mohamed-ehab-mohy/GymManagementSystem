using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.BLL.Interfaces;

public interface ITrainerService
{
    Task<IEnumerable<Trainer>> GetAllTrainersAsync();
    Task<Trainer?> GetTrainerByIdAsync(int id);
    Task AddTrainerAsync(Trainer trainer);
    Task UpdateTrainerAsync(Trainer trainer);
    Task DeleteTrainerAsync(int id);
}
