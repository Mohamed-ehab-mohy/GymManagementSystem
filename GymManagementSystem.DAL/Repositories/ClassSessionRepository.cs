using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories;

public class ClassSessionRepository : Repository<ClassSession>, IClassSessionRepository
{
    public ClassSessionRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ClassSession>> GetByTrainerIdAsync(int trainerId)
    {
        return await _context.ClassSessions
            .Where(cs => cs.TrainerId == trainerId)
            .ToListAsync();
    }
}
