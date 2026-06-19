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

    public async Task<Booking?> GetOrCreateBookingForMemberAsync(int memberId)
    {
        var existing = await _context.Bookings
            .Include(b => b.Member)
            .Include(b => b.ClassSession)
            .FirstOrDefaultAsync(b => b.MemberId == memberId);

        if (existing != null)
            return existing;

        var member = await _context.Members.FindAsync(memberId);
        var session = await _context.ClassSessions
            .FirstOrDefaultAsync(cs => cs.ScheduleTime.Date == DateTime.Today);

        if (member == null || session == null)
            return null;

        var booking = new Booking
        {
            MemberId = memberId,
            ClassSessionId = session.Id,
            BookingDate = DateTime.Today,
            IsAttended = false,
            Member = member,
            ClassSession = session
        };

        await _context.Bookings.AddAsync(booking);
        return booking;
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
}
