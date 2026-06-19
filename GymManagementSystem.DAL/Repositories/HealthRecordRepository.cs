using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain; using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.DAL.Repositories;

public class HealthRecordRepository : Repository<HealthRecord>, IHealthRecordRepository
{
    public HealthRecordRepository(GymDbContext context) : base(context)
    {
    }
}
