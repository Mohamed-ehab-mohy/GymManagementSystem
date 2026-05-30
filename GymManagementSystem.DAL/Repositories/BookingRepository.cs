using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        var booking = await _context.Bookings
            .Include(b => b.Member)
            .Include(b => b.ClassSession)
            .FirstOrDefaultAsync(b => b.MemberId == memberId);

        if (booking == null)
        {
            var member = await _context.Members.FindAsync(memberId);
            var session = await _context.ClassSessions.FirstOrDefaultAsync(cs => cs.ScheduleTime.Date == DateTime.Today);

            if (member != null && session != null)
            {
                booking = new Booking
                {
                    MemberId = memberId,
                    ClassSessionId = session.Id,
                    BookingDate = DateTime.Today,
                    IsAttended = false
                };
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                booking = await _context.Bookings
                    .Include(b => b.Member)
                    .Include(b => b.ClassSession)
                    .FirstOrDefaultAsync(b => b.Id == booking.Id);
            }
        }

        return booking;
    }
}
