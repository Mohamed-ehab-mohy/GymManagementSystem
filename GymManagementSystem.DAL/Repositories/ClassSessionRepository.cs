using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public class ClassSessionRepository : Repository<ClassSession>, IClassSessionRepository
{
    public ClassSessionRepository(GymDbContext context) : base(context)
    {
    }
}
