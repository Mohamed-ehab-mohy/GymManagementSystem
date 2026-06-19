using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.DTOs;
using GymManagementSystem.PL.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task<IActionResult> Index()
    {
        var dto = await _analyticsService.GetAnalyticsAsync();
        return View(dto);
    }
}
