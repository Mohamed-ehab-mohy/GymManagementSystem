using Microsoft.AspNetCore.Mvc;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.PL.ViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.BLL.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers;

public class PlansController : Controller
{
    private readonly IPlanService _planService;
    private readonly IExportService _exportService;

    public PlansController(IPlanService planService, IExportService exportService)
    {
        _planService = planService;
        _exportService = exportService;
    }

    private async Task<IEnumerable<PlanViewModel>> GetPlanViewModelsAsync()
    {
        var plans = await _planService.GetAllPlansAsync();
        return plans.Select(p => new PlanViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            DurationDays = p.DurationDays,
            Price = p.Price,
            IsActive = p.IsActive
        }).ToList();
    }

    public async Task<IActionResult> Index()
    {
        var viewModels = await GetPlanViewModelsAsync();
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
            TempData["SuccessMessage"] = "Plan created successfully.";
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
            TempData["SuccessMessage"] = "Plan updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var (success, message) = await _planService.ToggleActiveAsync(id);
        if (success)
            TempData["SuccessMessage"] = message;
        else
            TempData["ErrorMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var plans = await GetPlanViewModelsAsync();
        var columns = GetPlanColumnDefinitions();
        var fileBytes = await _exportService.ExportAsync(plans, columns, ExportFormat.Excel, "Gym Membership Plans Report");
        var fileName = $"plans_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf()
    {
        var plans = await GetPlanViewModelsAsync();
        var columns = GetPlanColumnDefinitions();
        var fileBytes = await _exportService.ExportAsync(plans, columns, ExportFormat.Pdf, "Gym Membership Plans Report");
        var fileName = $"plans_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
        return File(fileBytes, "application/pdf", fileName);
    }

    private List<ColumnDefinition<PlanViewModel>> GetPlanColumnDefinitions()
    {
        return new List<ColumnDefinition<PlanViewModel>>
        {
            new() { HeaderName = "Plan ID", ValueSelector = p => p.Id },
            new() { HeaderName = "Plan Name", ValueSelector = p => p.Name },
            new() { HeaderName = "Duration (Days)", ValueSelector = p => p.DurationDays },
            new() { HeaderName = "Price (EGP)", ValueSelector = p => p.Price },
            new() { HeaderName = "Status", ValueSelector = p => p.IsActive ? "Active" : "Inactive" }
        };
    }
}
