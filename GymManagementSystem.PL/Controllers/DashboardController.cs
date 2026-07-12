using GymManagementSystem.BLL.DTOs;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IMemberService _memberService;

    public DashboardController(IDashboardService dashboardService, IMemberService memberService)
    {
        _dashboardService = dashboardService;
        _memberService = memberService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole(Domain.Roles.Member))
        {
            var memberId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dto = await _memberService.GetMemberDashboardAsync(memberId);
            if (dto == null)
                return Forbid();

            var viewModel = dto.Adapt<MemberDashboardViewModel>();
            return View("MemberDashboard", viewModel);
        }

        var stats = await _dashboardService.GetStatsAsync();
        return View(stats);
    }
}
