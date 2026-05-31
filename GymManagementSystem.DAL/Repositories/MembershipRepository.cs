using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories;

public class MembershipRepository : Repository<Membership>, IMembershipRepository
{
    public MembershipRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Membership>> GetActiveByPlanIdAsync(int planId)
    {
        return await _context.Memberships
            .Where(m => m.PlanId == planId && m.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Membership>> GetActiveByMemberIdAsync(int memberId)
    {
        return await _context.Memberships
            .Where(m => m.MemberId == memberId && m.IsActive)
            .ToListAsync();
    }
}
