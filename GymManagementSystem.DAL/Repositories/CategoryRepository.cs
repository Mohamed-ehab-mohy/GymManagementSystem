using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain; using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.DAL.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(GymDbContext context) : base(context)
    {
    }
}
