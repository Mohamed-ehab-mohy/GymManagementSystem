using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

[Authorize(Roles = $"{Domain.Roles.Admin},{Domain.Roles.SuperAdmin}")]
public class AdminBookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IMemberService _memberService;
    private readonly IClassSessionService _sessionService;

    public AdminBookingController(IBookingService bookingService, IMemberService memberService, IClassSessionService sessionService)
    {
        _bookingService = bookingService;
        _memberService = memberService;
        _sessionService = sessionService;
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        const int pageSize = 15;
        var paged = await _bookingService.GetPagedBookingsAsync(page, pageSize, search);

        var viewModels = paged.Items.Select(b => new BookingViewModel
        {
            Id = b.Id,
            MemberId = b.MemberId,
            MemberName = $"{b.Member?.FirstName} {b.Member?.LastName}",
            ClassSessionId = b.ClassSessionId,
            SessionName = b.ClassSession?.Name ?? "",
            BookingDate = b.BookingDate,
            ScheduleTime = b.ClassSession?.ScheduleTime,
            IsAttended = b.IsAttended,
            CheckedInAt = b.CheckedInAt
        });

        ViewBag.Search = search;

        return View(new PagedResultDisplay<BookingViewModel>
        {
            Items = viewModels,
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        });
    }

    public async Task<IActionResult> Create()
    {
        var members = await _memberService.GetAllMembersAsync();
        var sessions = await _sessionService.GetAllClassSessionsAsync();

        var model = new CreateBookingViewModel
        {
            Members = members,
            Sessions = sessions
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model)
    {
        if (model.MemberId <= 0 || model.ClassSessionId <= 0)
        {
            ModelState.AddModelError("", "Please select a member and a session.");
        }

        if (ModelState.IsValid)
        {
            var booking = new Booking
            {
                MemberId = model.MemberId,
                ClassSessionId = model.ClassSessionId,
                BookingDate = DateTime.Today,
                IsAttended = false
            };

            await _bookingService.AddBookingAsync(booking);
            TempData["SuccessMessage"] = "Booking created successfully.";
            return RedirectToAction(nameof(Index));
        }

        model.Members = await _memberService.GetAllMembersAsync();
        model.Sessions = await _sessionService.GetAllClassSessionsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _bookingService.DeleteBookingAsync(id);
        TempData["SuccessMessage"] = "Booking deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
