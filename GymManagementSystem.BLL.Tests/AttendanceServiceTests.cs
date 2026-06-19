using System;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Attendance;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace GymManagementSystem.BLL.Tests;

public class AttendanceServiceTests
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly string _secretKey = "my_super_secret_attendance_key_for_testing";
    private readonly AttendanceService _service;

    public AttendanceServiceTests()
    {
        _bookingRepository = Substitute.For<IBookingRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _configuration = Substitute.For<IConfiguration>();
        _configuration["AttendanceSettings:SecretKey"].Returns(_secretKey);

        _service = new AttendanceService(_bookingRepository, _configuration, _unitOfWork);
    }

    [Fact]
    public async Task CheckInAsync_ValidPayload_ChecksInSuccessfully()
    {
        var bookingId = 123;
        var payload = CheckInPayloadHelper.BuildPayload(bookingId, _secretKey);
        var booking = new Booking
        {
            Id = bookingId,
            IsAttended = false,
            Member = new Member { FirstName = "John", LastName = "Doe" },
            ClassSession = new ClassSession { Name = "Yoga Class", ScheduleTime = DateTime.Today }
        };
        _bookingRepository.GetForCheckInAsync(bookingId).Returns(booking);

        var (result, memberName, sessionName) = await _service.CheckInAsync(payload);

        result.ShouldBe(AttendanceResult.Success);
        memberName.ShouldBe("John Doe");
        sessionName.ShouldBe("Yoga Class");
        booking.IsAttended.ShouldBeTrue();
        booking.CheckedInAt.ShouldNotBeNull();
        _bookingRepository.Received(1).Update(booking);
        await _unitOfWork.Received(1).CompleteAsync();
    }

    [Fact]
    public async Task CheckInAsync_TamperedSignature_ReturnsInvalidSignature()
    {
        var bookingId = 123;
        var payload = CheckInPayloadHelper.BuildPayload(bookingId, _secretKey) + "tampered";

        var (result, memberName, sessionName) = await _service.CheckInAsync(payload);

        result.ShouldBe(AttendanceResult.InvalidSignature);
        memberName.ShouldBeNull();
        sessionName.ShouldBeNull();
        _bookingRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }

    [Fact]
    public async Task CheckInAsync_AlreadyAttended_ReturnsAlreadyAttended()
    {
        var bookingId = 123;
        var payload = CheckInPayloadHelper.BuildPayload(bookingId, _secretKey);
        var booking = new Booking
        {
            Id = bookingId,
            IsAttended = true,
            Member = new Member { FirstName = "John", LastName = "Doe" },
            ClassSession = new ClassSession { Name = "Yoga Class", ScheduleTime = DateTime.Today }
        };
        _bookingRepository.GetForCheckInAsync(bookingId).Returns(booking);

        var (result, memberName, sessionName) = await _service.CheckInAsync(payload);

        result.ShouldBe(AttendanceResult.AlreadyAttended);
        memberName.ShouldBe("John Doe");
        sessionName.ShouldBe("Yoga Class");
        _bookingRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }

    [Fact]
    public async Task CheckInAsync_BookingNotFound_ReturnsNotFound()
    {
        var bookingId = 123;
        var payload = CheckInPayloadHelper.BuildPayload(bookingId, _secretKey);
        _bookingRepository.GetForCheckInAsync(bookingId).Returns((Booking?)null);

        var (result, memberName, sessionName) = await _service.CheckInAsync(payload);

        result.ShouldBe(AttendanceResult.NotFound);
        memberName.ShouldBeNull();
        sessionName.ShouldBeNull();
        _bookingRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }

    [Fact]
    public async Task CheckInAsync_SessionNotToday_ReturnsSessionNotToday()
    {
        var bookingId = 123;
        var payload = CheckInPayloadHelper.BuildPayload(bookingId, _secretKey);
        var booking = new Booking
        {
            Id = bookingId,
            IsAttended = false,
            Member = new Member { FirstName = "John", LastName = "Doe" },
            ClassSession = new ClassSession { Name = "Yoga Class", ScheduleTime = DateTime.Today.AddDays(1) }
        };
        _bookingRepository.GetForCheckInAsync(bookingId).Returns(booking);

        var (result, memberName, sessionName) = await _service.CheckInAsync(payload);

        result.ShouldBe(AttendanceResult.SessionNotToday);
        memberName.ShouldBe("John Doe");
        sessionName.ShouldBe("Yoga Class");
        _bookingRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceive().CompleteAsync();
    }

    [Fact]
    public async Task BuildCheckInPayload_RoundTripAndFormatVerification()
    {
        var bookingId = 456;
        var payload = _service.BuildCheckInPayload(bookingId);

        payload.ShouldStartWith("GYMYCHECKIN:");

        var (isValidFormat, isValidSignature, parsedBookingId) = CheckInPayloadHelper.ParseAndValidate(payload, _secretKey);
        isValidFormat.ShouldBeTrue();
        isValidSignature.ShouldBeTrue();
        parsedBookingId.ShouldBe(bookingId);

        var (invalidFormat, _, _) = CheckInPayloadHelper.ParseAndValidate("INVALIDPAYLOAD", _secretKey);
        invalidFormat.ShouldBeFalse();
    }

    [Fact]
    public void GenerateQrImage_ReturnsNonEmptyByteArray()
    {
        var payload = "GYMYCHECKIN:123:some_signature";
        var bytes = _service.GenerateQrImage(payload);

        bytes.ShouldNotBeNull();
        bytes.Length.ShouldBeGreaterThan(0);
    }
}
