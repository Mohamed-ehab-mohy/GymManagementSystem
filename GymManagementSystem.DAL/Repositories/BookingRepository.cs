using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(GymDbContext context) : base(context)
    {
    }
}
