using GymManagementSystem.BLL.Export;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IExportService _exportService;

    public MembersController(IMemberService memberService, IExportService exportService)
    {
        _memberService = memberService;
        _exportService = exportService;
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
            EmergencyContactPhone = m.EmergencyContactPhone
        }).ToList();
    }

    public async Task<IActionResult> Index()
    {
        var viewModels = await GetMemberViewModelsAsync();
        return View(viewModels);
    }

    public IActionResult Create()
    {
        return View(new MemberViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MemberViewModel model)
    {
        if (ModelState.IsValid)
        {
            var member = new Member
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Photo = model.Photo,
                JoinDate = model.JoinDate,
                Address = new Address
                {
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode
                },
                EmergencyContactName = model.EmergencyContactName ?? "",
                EmergencyContactPhone = model.EmergencyContactPhone ?? ""
            };

            await _memberService.AddMemberAsync(member);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
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
            EmergencyContactPhone = member.EmergencyContactPhone
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MemberViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();

            member.FirstName = model.FirstName;
            member.LastName = model.LastName;
            member.Email = model.Email;
            member.PhoneNumber = model.PhoneNumber;
            member.DateOfBirth = model.DateOfBirth;
            member.Gender = model.Gender;
            member.Photo = model.Photo;
            member.JoinDate = model.JoinDate;

            if (member.Address == null)
                member.Address = new Address();

            member.Address.Street = model.Street;
            member.Address.City = model.City;
            member.Address.State = model.State;
            member.Address.ZipCode = model.ZipCode;

            member.EmergencyContactName = model.EmergencyContactName ?? "";
            member.EmergencyContactPhone = model.EmergencyContactPhone ?? "";

            await _memberService.UpdateMemberAsync(member);
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
            EmergencyContactPhone = member.EmergencyContactPhone
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        await _memberService.DeleteMemberAsync(id);
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
