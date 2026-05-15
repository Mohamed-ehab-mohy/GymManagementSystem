using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(GymDbContext context) : base(context)
    {
    }
}
