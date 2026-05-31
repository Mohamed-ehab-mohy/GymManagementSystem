using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

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
        
        var viewModels = sessions.Select(cs => new ClassSessionViewModel
        {
            Id = cs.Id,
            Name = cs.Name,
            StartTime = cs.StartTime,
            EndTime = cs.EndTime,
            Capacity = cs.Capacity,
            TrainerId = cs.TrainerId,
            TrainerName = cs.Trainer?.FirstName + " " + cs.Trainer?.LastName,
            CategoryId = cs.CategoryId,
            CategoryName = cs.Category?.CategoryName
        }).OrderBy(cs => cs.StartTime).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        var model = new ClassSessionViewModel
        {
            TrainersList = await GetTrainersSelectListAsync(),
            CategoriesList = await GetCategoriesSelectListAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassSessionViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
                model.TrainersList = await GetTrainersSelectListAsync();
                model.CategoriesList = await GetCategoriesSelectListAsync();
                return View(model);
            }

            var session = new ClassSession
            {
                Name = model.Name,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Capacity = model.Capacity,
                TrainerId = model.TrainerId,
                CategoryId = model.CategoryId
            };

            await _classSessionService.AddClassSessionAsync(session);
            return RedirectToAction(nameof(Index));
        }

        model.TrainersList = await GetTrainersSelectListAsync();
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var session = await _classSessionService.GetClassSessionByIdAsync(id);
        if (session == null)
            return NotFound();

        var model = new ClassSessionViewModel
        {
            Id = session.Id,
            Name = session.Name,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Capacity = session.Capacity,
            TrainerId = session.TrainerId,
            CategoryId = session.CategoryId,
            TrainersList = await GetTrainersSelectListAsync(),
            CategoriesList = await GetCategoriesSelectListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClassSessionViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
                model.TrainersList = await GetTrainersSelectListAsync();
                model.CategoriesList = await GetCategoriesSelectListAsync();
                return View(model);
            }

            var session = await _classSessionService.GetClassSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            session.Name = model.Name;
            session.StartTime = model.StartTime;
            session.EndTime = model.EndTime;
            session.Capacity = model.Capacity;
            session.TrainerId = model.TrainerId;
            session.CategoryId = model.CategoryId;

            await _classSessionService.UpdateClassSessionAsync(session);
            return RedirectToAction(nameof(Index));
        }

        model.TrainersList = await GetTrainersSelectListAsync();
        model.CategoriesList = await GetCategoriesSelectListAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var session = await _classSessionService.GetClassSessionByIdAsync(id);
        if (session == null)
            return NotFound();

        var model = new ClassSessionViewModel
        {
            Id = session.Id,
            Name = session.Name,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Capacity = session.Capacity,
            TrainerId = session.TrainerId,
            TrainerName = session.Trainer?.FirstName + " " + session.Trainer?.LastName,
            CategoryId = session.CategoryId,
            CategoryName = session.Category?.CategoryName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _classSessionService.DeleteClassSessionAsync(id);
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
