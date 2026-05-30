using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetForCheckInAsync(int id);
    Task<Booking?> GetForMemberCheckInAsync(int memberId);
    Task<Booking?> GetOrCreateBookingForMemberAsync(int memberId);
}
