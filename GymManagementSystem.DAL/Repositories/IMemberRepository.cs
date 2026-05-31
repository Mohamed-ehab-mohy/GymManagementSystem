using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    Task<IEnumerable<Member>> GetAllWithDetailsAsync();
    Task<Member?> GetByIdWithDetailsAsync(int id);
}
