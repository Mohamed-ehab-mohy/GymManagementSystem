using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions;

namespace GymManagementSystem.BLL.Abstractions.Repositories;

public interface IClassSessionRepository : IRepository<ClassSession>
{
    Task<IEnumerable<ClassSession>> GetByTrainerIdAsync(int trainerId);
}
