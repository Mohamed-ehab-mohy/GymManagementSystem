using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions;

namespace GymManagementSystem.BLL.Abstractions.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    Task<IEnumerable<Member>> GetAllWithDetailsAsync();
    Task<Member?> GetByIdWithDetailsAsync(int id);
}
