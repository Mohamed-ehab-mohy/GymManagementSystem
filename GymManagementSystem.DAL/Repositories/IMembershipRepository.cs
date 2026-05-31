using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public interface IMembershipRepository : IRepository<Membership>
{
    Task<IEnumerable<Membership>> GetActiveByPlanIdAsync(int planId);
    Task<IEnumerable<Membership>> GetActiveByMemberIdAsync(int memberId);
}
