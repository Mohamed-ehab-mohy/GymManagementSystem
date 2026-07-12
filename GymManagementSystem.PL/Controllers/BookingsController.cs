using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

[Authorize(Roles = Domain.Roles.Member)]
public class BookingsController : Controller
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Index()
    {
        var sessions = await _bookingService.GetAvailableSessionsAsync();
        var viewModels = sessions.Adapt<IEnumerable<AvailableSessionViewModel>>();
        return View(viewModels);
    }

    public async Task<IActionResult> Mine()
    {
        var memberId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var bookings = await _bookingService.GetMyBookingsAsync(memberId);
        var viewModels = bookings.Adapt<IEnumerable<MyBookingViewModel>>();
        return View(viewModels);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int sessionId)
    {
        var memberId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookingService.BookSessionAsync(memberId, sessionId);

        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = "Booking confirmed!";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int bookingId)
    {
        var memberId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookingService.CancelBookingAsync(bookingId, memberId);

        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = "Booking cancelled.";
        }

        return RedirectToAction(nameof(Mine));
    }
}
