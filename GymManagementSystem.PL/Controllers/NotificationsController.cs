using GymManagementSystem.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(INotificationService notificationService, ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    private int GetUserId() => int.TryParse(_currentUserService.UserId, out var id) ? id : 0;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var notifications = await _notificationService.GetUnreadAsync(GetUserId());
        return View(notifications);
    }

    [HttpGet]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync(GetUserId());
        return Json(new { count });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _notificationService.MarkAllAsReadAsync(GetUserId());
        return Ok();
    }
}
