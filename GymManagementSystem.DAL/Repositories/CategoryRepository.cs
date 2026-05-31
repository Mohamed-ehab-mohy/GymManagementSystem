using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(GymDbContext context) : base(context)
    {
    }
}
