using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;

namespace GymManagementSystem.BLL.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<PagedResult<Booking>> GetPagedBookingsAsync(int page, int pageSize, string? search = null);
    Task AddBookingAsync(Booking booking);
    Task UpdateBookingAsync(Booking booking);
    Task DeleteBookingAsync(int id);

    Task<IEnumerable<ClassSession>> GetAvailableSessionsAsync();
    Task<Result> BookSessionAsync(int memberId, int sessionId);
    Task<Result> CancelBookingAsync(int bookingId, int memberId);
    Task<IEnumerable<Booking>> GetMyBookingsAsync(int memberId);
}
