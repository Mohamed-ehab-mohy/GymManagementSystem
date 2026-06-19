using System.Linq.Expressions;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(GymDbContext context) : base(context)
    {
    }

    public async Task<Booking?> GetForCheckInAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Member)
            .Include(b => b.ClassSession)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking?> GetForMemberCheckInAsync(int memberId)
    {
        return await _context.Bookings
            .Include(b => b.Member)
            .Include(b => b.ClassSession)
            .FirstOrDefaultAsync(b => b.MemberId == memberId);
    }

    public async Task<IEnumerable<Booking>> GetBySessionIdAsync(int sessionId)
    {
        return await _context.Bookings
            .Where(b => b.ClassSessionId == sessionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByMemberIdAsync(int memberId)
    {
        return await _context.Bookings
            .Where(b => b.MemberId == memberId)
            .ToListAsync();
    }

    public async Task<PagedResult<Booking>> GetPagedBookingsAsync(int page, int pageSize, string? search = null)
    {
        var query = _context.Bookings
            .Include(b => b.Member)
            .Include(b => b.ClassSession)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b =>
                (b.Member.FirstName + " " + b.Member.LastName).Contains(search));
        }

        query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.Id);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Booking>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
