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
}
