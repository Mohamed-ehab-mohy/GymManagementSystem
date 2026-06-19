using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain; using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.DAL.Repositories;

public class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(GymDbContext context) : base(context)
    {
    }
}
