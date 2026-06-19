using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Attendance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _secretKey;

        public AttendanceService(IBookingRepository bookingRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _secretKey = configuration["AttendanceSettings:SecretKey"] 
                ?? throw new InvalidOperationException("Attendance SecretKey is not configured.");
        }

        public string BuildCheckInPayload(int bookingId)
        {
            return CheckInPayloadHelper.BuildPayload(bookingId, _secretKey);
        }

        public byte[] GenerateQrImage(string payload)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
            using (var pngByteQrCode = new PngByteQRCode(qrCodeData))
            {
                return pngByteQrCode.GetGraphic(20);
            }
        }

        public async Task<(AttendanceResult result, string? memberName, string? sessionName)> CheckInAsync(string payload)
        {
            var (isValidFormat, isValidSignature, bookingId) = CheckInPayloadHelper.ParseAndValidate(payload, _secretKey);

            if (!isValidFormat)
                return (AttendanceResult.InvalidFormat, null, null);

            if (!isValidSignature)
                return (AttendanceResult.InvalidSignature, null, null);

            var booking = await _bookingRepository.GetForCheckInAsync(bookingId);
            if (booking == null)
                return (AttendanceResult.NotFound, null, null);

            string memberName = $"{booking.Member?.FirstName} {booking.Member?.LastName}";
            string sessionName = booking.ClassSession?.Name ?? "Gym Session";

            if (booking.IsAttended)
                return (AttendanceResult.AlreadyAttended, memberName, sessionName);

            if (booking.ClassSession == null || booking.ClassSession.ScheduleTime.Date != DateTime.Today)
                return (AttendanceResult.SessionNotToday, memberName, sessionName);

            booking.IsAttended = true;
            booking.CheckedInAt = DateTime.Now;

            _bookingRepository.Update(booking);
            await _unitOfWork.CompleteAsync();

            return (AttendanceResult.Success, memberName, sessionName);
        }
    }
}
