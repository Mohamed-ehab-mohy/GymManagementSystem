using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GymManagementSystem.BLL.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassSessionRepository _sessionRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public BookingService(IBookingRepository repository, IUnitOfWork unitOfWork,
        IClassSessionRepository sessionRepository, IMembershipRepository membershipRepository,
        INotificationService notificationService, ILogger<BookingService>? logger = null)
    {
        _bookingRepository = repository;
        _unitOfWork = unitOfWork;
        _sessionRepository = sessionRepository;
        _membershipRepository = membershipRepository;
        _notificationService = notificationService;
        _logger = logger ?? NullLogger<BookingService>.Instance;
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<PagedResult<Booking>> GetPagedBookingsAsync(int page, int pageSize, string? search = null)
    {
        return await _bookingRepository.GetPagedBookingsAsync(page, pageSize, search);
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task AddBookingAsync(Booking entity)
    {
        await _bookingRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateBookingAsync(Booking entity)
    {
        _bookingRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteBookingAsync(int id)
    {
        var entity = await _bookingRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _bookingRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }

    public async Task<IEnumerable<ClassSession>> GetAvailableSessionsAsync()
    {
        var sessions = await _sessionRepository.Query()
            .Include(s => s.Trainer)
            .Include(s => s.Category)
            .Include(s => s.Bookings)
            .Where(s => s.ScheduleTime > DateTime.Now)
            .OrderBy(s => s.ScheduleTime)
            .ToListAsync();

        return sessions.Where(s => s.Capacity > s.Bookings.Count(b => !b.IsDeleted));
    }

    public async Task<Result> BookSessionAsync(int memberId, int sessionId)
    {
        var session = await _sessionRepository.Query()
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
            return Result.Failure("Session not found.");

        if (session.ScheduleTime <= DateTime.Now)
            return Result.Failure("Cannot book a past session.");

        var bookingCount = session.Bookings.Count(b => !b.IsDeleted);
        if (bookingCount >= session.Capacity)
        {
            _logger.LogWarning("Booking conflict: session {SessionId} ({Name}) is full (capacity {Capacity})", sessionId, session.Name, session.Capacity);
            return Result.Failure("Session is full.");
        }

        var memberships = await _membershipRepository.Query()
            .Where(m => m.MemberId == memberId && m.IsActive && m.EndDate >= DateTime.Today)
            .ToListAsync();

        if (memberships.Count == 0)
            return Result.Failure("You need an active membership to book a session.");

        var existingBookings = await _bookingRepository.Query()
            .AnyAsync(b => b.MemberId == memberId && b.ClassSessionId == sessionId && !b.IsDeleted);

        if (existingBookings)
            return Result.Failure("You are already booked for this session.");

        var booking = new Booking
        {
            MemberId = memberId,
            ClassSessionId = sessionId,
            BookingDate = DateTime.Today,
            IsAttended = false
        };

        await _bookingRepository.AddAsync(booking);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Booking created: member {MemberId} booked session {SessionId} ({Name})", memberId, sessionId, session.Name);

        await _notificationService.SendToUserAsync(memberId, "Your booking has been confirmed.");
        await _notificationService.SendToUserAsync(session.TrainerId, $"A new booking has been made for session: {session.Name ?? session.Id.ToString()}");

        return Result.Success();
    }

    public async Task<Result> CancelBookingAsync(int bookingId, int memberId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null || booking.IsDeleted)
            return Result.Failure("Booking not found.");

        if (booking.MemberId != memberId)
            return Result.Failure("You can only cancel your own bookings.");

        if (booking.IsAttended)
            return Result.Failure("Cannot cancel an attended booking.");

        _bookingRepository.Delete(booking);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Booking cancelled: member {MemberId} cancelled booking {BookingId}", memberId, bookingId);

        await _notificationService.SendToUserAsync(memberId, "Your booking has been cancelled.");

        return Result.Success();
    }

    public async Task<IEnumerable<Booking>> GetMyBookingsAsync(int memberId)
    {
        return await _bookingRepository.Query()
            .Include(b => b.ClassSession).ThenInclude(s => s!.Trainer)
            .Include(b => b.ClassSession).ThenInclude(s => s!.Category)
            .Where(b => b.MemberId == memberId && !b.IsDeleted)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }
}