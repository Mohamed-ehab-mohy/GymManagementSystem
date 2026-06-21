using System;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class TrainersController : Controller
{
    private readonly ITrainerService _trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
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

        var paged = await _trainerService.GetPagedTrainersAsync(page, pageSize, search, sortBy, ascending);
        var data = paged.Items.Select(t => (object)new
        {
            name = $@"<div class=""d-flex align-items-center""><div class=""bg-primary text-white rounded-circle d-flex justify-content-center align-items-center me-3"" style=""width:40px;height:40px;font-weight:bold;background:linear-gradient(135deg,var(--primary-color),var(--secondary-color))!important;"">{t.FirstName[0]}{t.LastName[0]}</div><div><h6 class=""mb-0 fw-bold"">{t.FirstName} {t.LastName}</h6><small class=""text-secondary"">Hired: {t.HireDate?.ToString("MMM dd, yyyy")}</small></div></div>",
            specialty = $"<span class=\"badge bg-secondary\">{t.Specialty}</span>",
            contact = $@"<div class=""d-flex flex-column""><span class=""text-secondary mb-1""><i class=""bi bi-envelope me-2""></i>{t.Email}</span><span class=""text-secondary""><i class=""bi bi-telephone me-2""></i>{t.PhoneNumber}</span></div>",
            actions = $@"<div class=""btn-group"" role=""group""><a href=""{Url.Action("Details", "Trainers", new { id = t.Id })}"" class=""btn btn-sm btn-outline-info"" title=""Details""><i class=""bi bi-eye""></i></a><a href=""{Url.Action("Edit", "Trainers", new { id = t.Id })}"" class=""btn btn-sm btn-outline-warning"" title=""Edit""><i class=""bi bi-pencil""></i></a><form action=""{Url.Action("Delete", "Trainers", new { id = t.Id })}"" method=""post"" style=""display:inline;""><button type=""submit"" class=""btn btn-sm btn-outline-danger"" title=""Delete"" onclick=""return confirm('Are you sure you want to delete this trainer?');""><i class=""bi bi-trash""></i></button></form></div>"
        }).ToList();

        return Json(new DataTableResponse<object>
        {
            Draw = request.Draw,
            RecordsTotal = paged.TotalCount,
            RecordsFiltered = paged.TotalCount,
            Data = data
        });
    }

    public IActionResult Create()
    {
        return View(new TrainerViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerViewModel model)
    {
        if (ModelState.IsValid)
        {
            var trainer = new Trainer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode
                },
                Specialty = model.Specialty,
                HireDate = model.HireDate
            };

            await _trainerService.AddTrainerAsync(trainer);
            TempData["SuccessMessage"] = "Trainer created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var trainer = await _trainerService.GetTrainerByIdAsync(id);
        if (trainer == null)
            return NotFound();

        var model = new TrainerViewModel
        {
            Id = trainer.Id,
            FirstName = trainer.FirstName,
            LastName = trainer.LastName,
            Email = trainer.Email,
            PhoneNumber = trainer.PhoneNumber,
            DateOfBirth = trainer.DateOfBirth,
            Gender = trainer.Gender,
            Street = trainer.Address?.Street ?? "",
            City = trainer.Address?.City ?? "",
            State = trainer.Address?.State ?? "",
            ZipCode = trainer.Address?.ZipCode ?? "",
            Specialty = trainer.Specialty,
            HireDate = trainer.HireDate.GetValueOrDefault(DateTime.Today)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainerViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null)
                return NotFound();

            trainer.FirstName = model.FirstName;
            trainer.LastName = model.LastName;
            trainer.Email = model.Email;
            trainer.PhoneNumber = model.PhoneNumber;
            trainer.DateOfBirth = model.DateOfBirth;
            trainer.Gender = model.Gender;

            if (trainer.Address == null)
                trainer.Address = new Address();

            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Address.State = model.State;
            trainer.Address.ZipCode = model.ZipCode;

            trainer.Specialty = model.Specialty;
            trainer.HireDate = model.HireDate;

            await _trainerService.UpdateTrainerAsync(trainer);
            TempData["SuccessMessage"] = "Trainer updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var trainer = await _trainerService.GetTrainerByIdAsync(id);
        if (trainer == null)
            return NotFound();

        var model = new TrainerViewModel
        {
            Id = trainer.Id,
            FirstName = trainer.FirstName,
            LastName = trainer.LastName,
            Email = trainer.Email,
            PhoneNumber = trainer.PhoneNumber,
            DateOfBirth = trainer.DateOfBirth,
            Gender = trainer.Gender,
            Street = trainer.Address?.Street ?? "",
            City = trainer.Address?.City ?? "",
            State = trainer.Address?.State ?? "",
            ZipCode = trainer.Address?.ZipCode ?? "",
            Specialty = trainer.Specialty,
            HireDate = trainer.HireDate.GetValueOrDefault(DateTime.Today)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _trainerService.DeleteTrainerAsync(id);
        if (success)
            TempData["SuccessMessage"] = message;
        else
            TempData["ErrorMessage"] = message;
        return RedirectToAction(nameof(Index));
    }
}
