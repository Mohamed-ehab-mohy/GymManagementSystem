using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.Tests.Infrastructure;
using GymManagementSystem.Domain;
using NSubstitute;
using Shouldly;

namespace GymManagementSystem.BLL.Tests;

public class BookingServiceTests
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClassSessionRepository _sessionRepo;
    private readonly IMembershipRepository _membershipRepo;
    private readonly INotificationService _notificationService;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepo = Substitute.For<IBookingRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sessionRepo = Substitute.For<IClassSessionRepository>();
        _membershipRepo = Substitute.For<IMembershipRepository>();
        _notificationService = Substitute.For<INotificationService>();
        _bookingService = new BookingService(_bookingRepo, _unitOfWork, _sessionRepo, _membershipRepo, _notificationService);
    }

    [Fact]
    public async Task BookSessionAsync_FullCapacity_ReturnsFailure()
    {
        var sessionId = 1;
        var memberId = 42;
        var futureTime = DateTime.Now.AddDays(1);

        var session = new ClassSession
        {
            Id = sessionId,
            Capacity = 10,
            ScheduleTime = futureTime,
            Bookings = new List<Booking>
            {
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false },
                new() { IsDeleted = false }
            },
            TrainerId = 1
        };

        _sessionRepo.Query().Returns(new TestAsyncEnumerable<ClassSession>([session]).AsQueryable());

        var result = await _bookingService.BookSessionAsync(memberId, sessionId);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe("Session is full.");
    }

    [Fact]
    public async Task BookSessionAsync_NoActiveMembership_ReturnsFailure()
    {
        var sessionId = 1;
        var memberId = 42;
        var futureTime = DateTime.Now.AddDays(1);

        var session = new ClassSession
        {
            Id = sessionId,
            Capacity = 10,
            ScheduleTime = futureTime,
            Bookings = new List<Booking>(),
            TrainerId = 1
        };

        _sessionRepo.Query().Returns(new TestAsyncEnumerable<ClassSession>([session]).AsQueryable());
        _membershipRepo.Query().Returns(new TestAsyncEnumerable<Membership>([]).AsQueryable());

        var result = await _bookingService.BookSessionAsync(memberId, sessionId);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldContain("active membership");
    }

    [Fact]
    public async Task BookSessionAsync_AlreadyBooked_ReturnsFailure()
    {
        var sessionId = 1;
        var memberId = 42;
        var futureTime = DateTime.Now.AddDays(1);

        var session = new ClassSession
        {
            Id = sessionId,
            Capacity = 10,
            ScheduleTime = futureTime,
            Bookings = new List<Booking>(),
            TrainerId = 1
        };

        _sessionRepo.Query().Returns(new TestAsyncEnumerable<ClassSession>([session]).AsQueryable());
        _membershipRepo.Query().Returns(new TestAsyncEnumerable<Membership>([
            new() { MemberId = memberId, IsActive = true, EndDate = DateTime.Today.AddMonths(1) }
        ]).AsQueryable());
        _bookingRepo.Query().Returns(new TestAsyncEnumerable<Booking>([
            new() { MemberId = memberId, ClassSessionId = sessionId, IsDeleted = false }
        ]).AsQueryable());

        var result = await _bookingService.BookSessionAsync(memberId, sessionId);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe("You are already booked for this session.");
    }

    [Fact]
    public async Task BookSessionAsync_ValidRequest_ReturnsSuccess()
    {
        var sessionId = 1;
        var memberId = 42;
        var futureTime = DateTime.Now.AddDays(1);

        var session = new ClassSession
        {
            Id = sessionId,
            Capacity = 10,
            ScheduleTime = futureTime,
            Bookings = new List<Booking>(),
            TrainerId = 1
        };

        _sessionRepo.Query().Returns(new TestAsyncEnumerable<ClassSession>([session]).AsQueryable());
        _membershipRepo.Query().Returns(new TestAsyncEnumerable<Membership>([
            new() { MemberId = memberId, IsActive = true, EndDate = DateTime.Today.AddMonths(1) }
        ]).AsQueryable());
        _bookingRepo.Query().Returns(new TestAsyncEnumerable<Booking>([]).AsQueryable());

        var result = await _bookingService.BookSessionAsync(memberId, sessionId);

        result.IsSuccess.ShouldBeTrue();
        await _bookingRepo.Received(1).AddAsync(Arg.Any<Booking>());
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task CancelBookingAsync_WrongOwner_ReturnsFailure()
    {
        var bookingId = 1;
        var memberId = 42;
        var otherMemberId = 99;

        var booking = new Booking
        {
            Id = bookingId,
            MemberId = otherMemberId,
            IsAttended = false,
            IsDeleted = false
        };

        _bookingRepo.GetByIdAsync(bookingId).Returns(booking);

        var result = await _bookingService.CancelBookingAsync(bookingId, memberId);

        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldContain("own bookings");
    }
}
