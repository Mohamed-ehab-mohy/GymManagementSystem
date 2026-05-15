using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _memberService.GetAllMembersAsync();
        var viewModels = members.Select(m => new MemberViewModel
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            Email = m.Email,
            PhoneNumber = m.PhoneNumber,
            Street = m.Address?.Street ?? "",
            City = m.Address?.City ?? "",
            State = m.Address?.State ?? "",
            ZipCode = m.Address?.ZipCode ?? "",
            EmergencyContactName = m.EmergencyContactName,
            EmergencyContactPhone = m.EmergencyContactPhone
        }).ToList();

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
        // For Members, "IsActive" might translate to deleting or not.
        // We'll use DeleteMemberAsync as a Soft Delete.
        await _memberService.DeleteMemberAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
