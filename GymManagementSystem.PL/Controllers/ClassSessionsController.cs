using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class ClassSessionsController : Controller
{
    private readonly IClassSessionService _classSessionService;
    private readonly ITrainerService _trainerService;
    private readonly ICategoryService _categoryService;

    public ClassSessionsController(IClassSessionService classSessionService, ITrainerService trainerService, ICategoryService categoryService)
    {
        _classSessionService = classSessionService;
        _trainerService = trainerService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var sessions = await _classSessionService.GetAllClassSessionsAsync();

        var viewModels = sessions.Select(cs => new SessionViewModel
        {
            Id = cs.Id,
            Description = cs.Name,
            StartDate = cs.StartTime,
            EndDate = cs.EndTime,
            Capacity = cs.Capacity,
            TrainerName = cs.Trainer?.FirstName + " " + cs.Trainer?.LastName,
            CategoryName = cs.Category?.CategoryName,
            AvailableSlots = cs.Capacity
        }).OrderBy(cs => cs.StartDate).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        var model = new CreateSessionViewModel
        {
            TrainersList = await GetTrainersSelectListAsync(),
            CategoriesList = await GetCategoriesSelectListAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSessionViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End time must be after start time.");
                model.TrainersList = await GetTrainersSelectListAsync();
                model.CategoriesList = await GetCategoriesSelectListAsync();
                return View(model);
            }

            var session = new ClassSession
            {
                Name = model.Description,
                ScheduleTime = model.StartDate.Date,
                StartTime = model.StartDate,
                EndTime = model.EndDate,
                Capacity = model.Capacity,
                TrainerId = model.TrainerId,
                CategoryId = model.CategoryId
            };

            await _classSessionService.AddClassSessionAsync(session);
            return RedirectToAction(nameof(Index));
        }

        model.TrainersList = await GetTrainersSelectListAsync();
        model.CategoriesList = await GetCategoriesSelectListAsync();
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var session = await _classSessionService.GetClassSessionByIdAsync(id);
        if (session == null)
            return NotFound();

        var model = new UpdateSessionViewModel
        {
            Id = session.Id,
            Description = session.Name,
            StartDate = session.StartTime,
            EndDate = session.EndTime,
            Capacity = session.Capacity,
            TrainerId = session.TrainerId,
            TrainersList = await GetTrainersSelectListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End time must be after start time.");
                model.TrainersList = await GetTrainersSelectListAsync();
                return View(model);
            }

            var session = await _classSessionService.GetClassSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            session.Name = model.Description;
            session.StartTime = model.StartDate;
            session.EndTime = model.EndDate;
            session.Capacity = model.Capacity;
            session.TrainerId = model.TrainerId;

            await _classSessionService.UpdateClassSessionAsync(session);
            return RedirectToAction(nameof(Index));
        }

        model.TrainersList = await GetTrainersSelectListAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var session = await _classSessionService.GetClassSessionByIdAsync(id);
        if (session == null)
            return NotFound();

        var model = new SessionViewModel
        {
            Id = session.Id,
            Description = session.Name,
            StartDate = session.StartTime,
            EndDate = session.EndTime,
            Capacity = session.Capacity,
            TrainerName = session.Trainer?.FirstName + " " + session.Trainer?.LastName,
            CategoryName = session.Category?.CategoryName,
            AvailableSlots = session.Capacity
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _classSessionService.DeleteClassSessionAsync(id);
        if (success)
            TempData["SuccessMessage"] = message;
        else
            TempData["ErrorMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    private async Task<SelectList> GetTrainersSelectListAsync()
    {
        var trainers = await _trainerService.GetAllTrainersAsync();
        var trainerOptions = trainers.Select(t => new
        {
            Id = t.Id,
            FullName = $"{t.FirstName} {t.LastName} ({t.Specialty})"
        });
        return new SelectList(trainerOptions, "Id", "FullName");
    }

    private async Task<SelectList> GetCategoriesSelectListAsync()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return new SelectList(categories, "Id", "CategoryName");
    }
}
