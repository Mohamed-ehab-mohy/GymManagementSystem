using Microsoft.AspNetCore.Mvc;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.ViewModels;
using GymManagementSystem.DAL.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers;

public class PlansController : Controller
{
    private readonly IPlanService _planService;

    public PlansController(IPlanService planService)
    {
        _planService = planService;
    }

    public async Task<IActionResult> Index()
    {
        var plans = await _planService.GetAllPlansAsync();
        var viewModels = plans.Select(p => new PlanViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            DurationDays = p.DurationDays,
            Price = p.Price,
            IsActive = p.IsActive
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _planService.GetPlanByIdAsync(id.Value);
        if (plan == null) return NotFound();

        var viewModel = new PlanViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            DurationDays = plan.DurationDays,
            Price = plan.Price,
            IsActive = plan.IsActive
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var plan = new Plan
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                DurationDays = viewModel.DurationDays,
                Price = viewModel.Price,
                IsActive = viewModel.IsActive
            };

            await _planService.CreatePlanAsync(plan);
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _planService.GetPlanByIdAsync(id.Value);
        if (plan == null) return NotFound();

        var viewModel = new PlanViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            DurationDays = plan.DurationDays,
            Price = plan.Price,
            IsActive = plan.IsActive
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlanViewModel viewModel)
    {
        if (id != viewModel.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            if (plan == null) return NotFound();

            plan.Name = viewModel.Name;
            plan.Description = viewModel.Description;
            plan.DurationDays = viewModel.DurationDays;
            plan.Price = viewModel.Price;
            plan.IsActive = viewModel.IsActive;

            await _planService.UpdatePlanAsync(plan);
            
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan != null)
        {
            plan.IsActive = !plan.IsActive;
            await _planService.UpdatePlanAsync(plan);
        }
        return RedirectToAction(nameof(Index));
    }
}
