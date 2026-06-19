using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions;

namespace GymManagementSystem.BLL.Abstractions.Repositories;

public interface IMembershipRepository : IRepository<Membership>
{
    Task<IEnumerable<Membership>> GetActiveByPlanIdAsync(int planId);
    Task<IEnumerable<Membership>> GetActiveByMemberIdAsync(int memberId);
}
