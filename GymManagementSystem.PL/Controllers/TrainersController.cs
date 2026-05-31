using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

public class TrainersController : Controller
{
    private readonly ITrainerService _trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
    }

    public async Task<IActionResult> Index()
    {
        var trainers = await _trainerService.GetAllTrainersAsync();
        var viewModels = trainers.Select(t => new TrainerViewModel
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            PhoneNumber = t.PhoneNumber,
            DateOfBirth = t.DateOfBirth,
            Gender = t.Gender,
            Street = t.Address?.Street ?? "",
            City = t.Address?.City ?? "",
            State = t.Address?.State ?? "",
            ZipCode = t.Address?.ZipCode ?? "",
            Specialty = t.Specialty,
            HireDate = t.HireDate.GetValueOrDefault(DateTime.Today)
        }).ToList();

        return View(viewModels);
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
    public async Task<IActionResult> ToggleActive(int id)
    {
        await _trainerService.DeleteTrainerAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
