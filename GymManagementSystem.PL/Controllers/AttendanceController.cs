using GymManagementSystem.BLL.Attendance;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IBookingRepository _bookingRepository;

        public AttendanceController(IAttendanceService attendanceService, IBookingRepository bookingRepository)
        {
            _attendanceService = attendanceService;
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> MyQr(int bookingId)
        {
            var booking = await _bookingRepository.GetForCheckInAsync(bookingId);
            if (booking == null)
                return NotFound();

            string payload = _attendanceService.BuildCheckInPayload(booking.Id);
            byte[] qrBytes = _attendanceService.GenerateQrImage(payload);
            string base64Image = Convert.ToBase64String(qrBytes);

            var viewModel = new AttendanceQrViewModel
            {
                BookingId = booking.Id,
                MemberName = $"{booking.Member?.FirstName} {booking.Member?.LastName}",
                SessionName = booking.ClassSession?.Name ?? "Gym Session",
                SessionDate = booking.ClassSession?.ScheduleTime ?? DateTime.Now,
                QrCodeBase64 = $"data:image/png;base64,{base64Image}"
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Scan()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                TempData["ErrorMessage"] = "Invalid or empty QR code scanned.";
                return RedirectToAction(nameof(Scan));
            }

            var (result, memberName, sessionName) = await _attendanceService.CheckInAsync(payload);

            switch (result)
            {
                case AttendanceResult.Success:
                    TempData["SuccessMessage"] = $"Check-In Successful! Welcome {memberName} to the session '{sessionName}'.";
                    break;

                case AttendanceResult.NotFound:
                    TempData["ErrorMessage"] = "Error: Booking record not found in the system.";
                    break;

                case AttendanceResult.AlreadyAttended:
                    TempData["WarningMessage"] = $"Attendance Warning: {memberName} has already checked in for '{sessionName}'.";
                    break;

                case AttendanceResult.InvalidSignature:
                    TempData["ErrorMessage"] = "Security Alert: Tampered or invalid QR signature detected!";
                    break;

                case AttendanceResult.InvalidFormat:
                    TempData["ErrorMessage"] = "Error: Scanned QR format is not recognized by Gymy.";
                    break;

                case AttendanceResult.SessionNotToday:
                    TempData["ErrorMessage"] = $"Rejected: This session '{sessionName}' is not scheduled for today!";
                    break;
            }

            return RedirectToAction(nameof(Scan));
        }
    }
}
