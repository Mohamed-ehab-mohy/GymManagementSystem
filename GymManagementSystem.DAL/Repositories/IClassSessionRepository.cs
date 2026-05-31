using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public interface IClassSessionRepository : IRepository<ClassSession>
{
    Task<IEnumerable<ClassSession>> GetByTrainerIdAsync(int trainerId);
}
