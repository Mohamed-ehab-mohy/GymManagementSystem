using GymManagementSystem.BLL.Export;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.PL.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IExportService _exportService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;

    public MembersController(IMemberService memberService, IExportService exportService, IUnitOfWork unitOfWork, IWebHostEnvironment env)
    {
        _memberService = memberService;
        _exportService = exportService;
        _unitOfWork = unitOfWork;
        _env = env;
    }

    private static void SplitName(string fullName, out string firstName, out string lastName)
    {
        var parts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        firstName = parts[0];
        lastName = parts.Length > 1 ? parts[1] : "";
    }

    private async Task<IEnumerable<MemberViewModel>> GetMemberViewModelsAsync()
    {
        var members = await _memberService.GetAllMembersAsync();
        return members.Select(m => new MemberViewModel
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            Email = m.Email,
            PhoneNumber = m.PhoneNumber,
            DateOfBirth = m.DateOfBirth,
            Gender = m.Gender,
            Photo = m.Photo,
            JoinDate = m.JoinDate,
            Street = m.Address?.Street ?? "",
            City = m.Address?.City ?? "",
            State = m.Address?.State ?? "",
            ZipCode = m.Address?.ZipCode ?? "",
            EmergencyContactName = m.EmergencyContactName,
            EmergencyContactPhone = m.EmergencyContactPhone,
            Height = m.HealthRecord?.Height ?? 0,
            Weight = m.HealthRecord?.Weight ?? 0,
            BloodType = m.HealthRecord?.BloodType ?? "",
            Note = m.HealthRecord?.Note
        }).ToList();
    }

    private async Task<string?> SavePhotoAsync(int memberId, Microsoft.AspNetCore.Http.IFormFile? photoFile)
    {
        if (photoFile == null || photoFile.Length == 0)
            return null;

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(photoFile.FileName);
        var fileName = $"member_{memberId}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photoFile.CopyToAsync(stream);
        }

        return $"/uploads/{fileName}";
    }

    public async Task<IActionResult> Index()
    {
        var viewModels = await GetMemberViewModelsAsync();
        return View(viewModels);
    }

    public IActionResult Create()
    {
        return View(new CreateMemberViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMemberViewModel model)
    {
        if (ModelState.IsValid)
        {
            SplitName(model.Name, out var firstName, out var lastName);

            var member = new Member
            {
                FirstName = firstName,
                LastName = lastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                JoinDate = DateTime.Today,
                Address = new Address
                {
                    Street = $"{model.BuildingNumber} {model.Street}",
                    City = model.City,
                    State = "",
                    ZipCode = ""
                },
                EmergencyContactName = "",
                EmergencyContactPhone = "",
                HealthRecord = new HealthRecord
                {
                    Height = model.HealthRecord.Height,
                    Weight = model.HealthRecord.Weight,
                    BloodType = model.HealthRecord.BloodType,
                    Note = model.HealthRecord.Note,
                    LastUpdate = DateTime.Now
                }
            };

            await _memberService.AddMemberAsync(member);

            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                member.Photo = await SavePhotoAsync(member.Id, model.PhotoFile);
                await _memberService.UpdateMemberAsync(member);
            }

            TempData["SuccessMessage"] = "Member created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        var addressParts = (member.Address?.Street ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var buildingNumber = addressParts.Length > 0 ? addressParts[0] : "";
        var street = addressParts.Length > 1 ? addressParts[1] : (member.Address?.Street ?? "");

        var model = new MemberToUpdateViewModel
        {
            Id = member.Id,
            Name = $"{member.FirstName} {member.LastName}",
            Photo = member.Photo,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            BuildingNumber = buildingNumber,
            Street = street,
            City = member.Address?.City ?? ""
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MemberToUpdateViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();

            SplitName(model.Name, out var firstName, out var lastName);
            member.FirstName = firstName;
            member.LastName = lastName;
            member.Email = model.Email;
            member.PhoneNumber = model.PhoneNumber;

            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(member.Photo))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, member.Photo.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
                member.Photo = await SavePhotoAsync(member.Id, model.PhotoFile);
            }

            if (member.Address == null)
                member.Address = new Address();

            member.Address.Street = $"{model.BuildingNumber} {model.Street}";
            member.Address.City = model.City;

            await _memberService.UpdateMemberAsync(member);
            TempData["SuccessMessage"] = "Member updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();

        var model = new MemberViewModel
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            DateOfBirth = member.DateOfBirth,
            Gender = member.Gender,
            Photo = member.Photo,
            JoinDate = member.JoinDate,
            Street = member.Address?.Street ?? "",
            City = member.Address?.City ?? "",
            State = member.Address?.State ?? "",
            ZipCode = member.Address?.ZipCode ?? "",
            EmergencyContactName = member.EmergencyContactName,
            EmergencyContactPhone = member.EmergencyContactPhone,
            Height = member.HealthRecord?.Height ?? 0,
            Weight = member.HealthRecord?.Weight ?? 0,
            BloodType = member.HealthRecord?.BloodType ?? "",
            Note = member.HealthRecord?.Note
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _memberService.DeleteMemberAsync(id);
        if (success)
            TempData["SuccessMessage"] = message;
        else
            TempData["ErrorMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var members = await GetMemberViewModelsAsync();
        var columns = GetMemberColumnDefinitions();
        var fileBytes = await _exportService.ExportAsync(members, columns, ExportFormat.Excel, "Gym Members Report");
        var fileName = $"members_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> ExportPdf()
    {
        var members = await GetMemberViewModelsAsync();
        var columns = GetMemberColumnDefinitions();
        var fileBytes = await _exportService.ExportAsync(members, columns, ExportFormat.Pdf, "Gym Members Report");
        var fileName = $"members_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
        return File(fileBytes, "application/pdf", fileName);
    }

    private List<ColumnDefinition<MemberViewModel>> GetMemberColumnDefinitions()
    {
        return new List<ColumnDefinition<MemberViewModel>>
        {
            new() { HeaderName = "ID", ValueSelector = m => m.Id },
            new() { HeaderName = "First Name", ValueSelector = m => m.FirstName },
            new() { HeaderName = "Last Name", ValueSelector = m => m.LastName },
            new() { HeaderName = "Email", ValueSelector = m => m.Email },
            new() { HeaderName = "Phone Number", ValueSelector = m => m.PhoneNumber }
        };
    }
}
