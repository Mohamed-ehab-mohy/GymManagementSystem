using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Member>> GetAllWithDetailsAsync()
    {
        return await _context.Members
            .Include(m => m.Address)
            .Include(m => m.HealthRecord)
            .ToListAsync();
    }

    public async Task<Member?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Members
            .Include(m => m.Address)
            .Include(m => m.HealthRecord)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}
