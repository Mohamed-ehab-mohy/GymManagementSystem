using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
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

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DataTableData([FromForm] DataTableRequest request)
    {
        var page = (request.Start / request.Length) + 1;
        var pageSize = request.Length;
        var search = request.Search?.Value;
        string? sortBy = null;
        var ascending = true;

        if (request.Order.Count > 0)
        {
            sortBy = request.Columns[request.Order[0].Column].Name;
            ascending = request.Order[0].Dir != "desc";
        }

        var paged = await _classSessionService.GetPagedSessionsAsync(page, pageSize, search, sortBy, ascending);

        var now = DateTime.Now;
        var data = paged.Items.Select(s => (object)new
        {
            session = $@"<div class=""d-flex align-items-center""><div class=""bg-primary text-white rounded d-flex justify-content-center align-items-center me-3"" style=""width:45px;height:45px;background:linear-gradient(135deg,var(--primary-color),var(--secondary-color))!important;""><i class=""bi bi-stopwatch fs-4""></i></div><h6 class=""mb-0 fw-bold"">{s.Name}</h6></div>",
            category = $"<span class=\"badge bg-info bg-opacity-10 text-info px-3 py-2\"><i class=\"bi bi-tag me-1\"></i> {(s.Category?.CategoryName ?? "")}</span>",
            schedule = $@"<div class=""d-flex flex-column""><span class=""text-primary fw-medium mb-1""><i class=""bi bi-calendar-event me-2 text-info""></i>{s.StartTime:dddd, MMMM dd, yyyy}</span><span class=""text-secondary""><i class=""bi bi-clock me-2""></i>{s.StartTime:hh:mm tt} - {s.EndTime:hh:mm tt}</span></div>",
            capacity = $"<span class=\"badge bg-dark border border-secondary px-3 py-2\"><i class=\"bi bi-people-fill me-2\"></i> {s.Capacity}</span>",
            status = now < s.StartTime ? "<span class=\"badge bg-success px-3 py-2\">Upcoming</span>" : now >= s.StartTime && now <= s.EndTime ? "<span class=\"badge bg-warning text-dark px-3 py-2\">Ongoing</span>" : "<span class=\"badge bg-secondary px-3 py-2\">Completed</span>",
            trainer = $"<span class=\"text-secondary\"><i class=\"bi bi-person-badge me-2\"></i>{s.Trainer?.FirstName} {s.Trainer?.LastName}</span>",
            actions = $@"<div class=""btn-group"" role=""group""><a href=""{Url.Action("Details", "ClassSessions", new { id = s.Id })}"" class=""btn btn-sm btn-outline-info"" title=""Details""><i class=""bi bi-eye""></i></a><a href=""{Url.Action("Edit", "ClassSessions", new { id = s.Id })}"" class=""btn btn-sm btn-outline-warning"" title=""Edit""><i class=""bi bi-pencil""></i></a><form action=""{Url.Action("Delete", "ClassSessions", new { id = s.Id })}"" method=""post"" style=""display:inline;""><button type=""submit"" class=""btn btn-sm btn-outline-danger"" title=""Cancel Session"" onclick=""return confirm('Are you sure you want to cancel this session?');""><i class=""bi bi-trash""></i></button></form></div>"
        }).ToList();

        return Json(new DataTableResponse<object>
        {
            Draw = request.Draw,
            RecordsTotal = paged.TotalCount,
            RecordsFiltered = paged.TotalCount,
            Data = data
        });
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
