using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetForCheckInAsync(int id);
    Task<Booking?> GetForMemberCheckInAsync(int memberId);
    Task<IEnumerable<Booking>> GetBySessionIdAsync(int sessionId);
    Task<IEnumerable<Booking>> GetByMemberIdAsync(int memberId);
    Task<PagedResult<Booking>> GetPagedBookingsAsync(int page, int pageSize, string? search = null);
}
