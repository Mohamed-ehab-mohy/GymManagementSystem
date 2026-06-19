using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain; using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.DAL.Repositories;

public class TrainerRepository : Repository<Trainer>, ITrainerRepository
{
    public TrainerRepository(GymDbContext context) : base(context)
    {
    }
}
