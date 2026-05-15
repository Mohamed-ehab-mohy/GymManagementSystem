using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(GymDbContext context) : base(context)
    {
    }
}
