using GymManagementSystem.BLL.Export;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.PL.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers;

[Authorize]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IExportService _exportService;
    private readonly IAttachmentService _attachmentService;

    public MembersController(IMemberService memberService, IExportService exportService, IAttachmentService attachmentService)
    {
        _memberService = memberService;
        _exportService = exportService;
        _attachmentService = attachmentService;
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null, string? sortBy = null, bool ascending = true)
    {
        const int pageSize = 10;
        var pagedMembers = await _memberService.GetPagedMembersAsync(page, pageSize, search, sortBy, ascending);
        var viewModels = pagedMembers.Items.Adapt<IEnumerable<MemberViewModel>>();

        ViewBag.Search = search;
        ViewBag.SortBy = sortBy;
        ViewBag.Ascending = ascending;

        return View(new PagedResultDisplay<MemberViewModel>
        {
            Items = viewModels,
            TotalCount = pagedMembers.TotalCount,
            Page = pagedMembers.Page,
            PageSize = pagedMembers.PageSize
        });
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
                    City = model.City
                },
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
                using var stream = model.PhotoFile.OpenReadStream();
                member.Photo = await _attachmentService.SaveFileAsync("uploads", member.Id, model.PhotoFile.FileName, stream);
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

        var model = new MemberToUpdateViewModel
        {
            Id = member.Id,
            Name = $"{member.FirstName} {member.LastName}",
            Photo = member.Photo,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            BuildingNumber = addressParts.Length > 0 ? addressParts[0] : "",
            Street = addressParts.Length > 1 ? addressParts[1] : (member.Address?.Street ?? ""),
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
                    await _attachmentService.DeleteFileAsync(member.Photo);

                using var stream = model.PhotoFile.OpenReadStream();
                member.Photo = await _attachmentService.SaveFileAsync("uploads", member.Id, model.PhotoFile.FileName, stream);
            }

            member.Address ??= new Address();
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

        var model = member.Adapt<MemberViewModel>();
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

    public async Task<IActionResult> ExportExcel()
    {
        var members = await _memberService.GetAllMembersAsync();
        var viewModels = members.Adapt<IEnumerable<MemberViewModel>>();
        var columns = GetMemberColumnDefinitions();
        var fileBytes = await _exportService.ExportAsync(viewModels, columns, ExportFormat.Excel, "Gym Members Report");
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"members_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }

    public async Task<IActionResult> ExportPdf()
    {
        var members = await _memberService.GetAllMembersAsync();
        var viewModels = members.Adapt<IEnumerable<MemberViewModel>>();
        var columns = GetMemberColumnDefinitions();
        var fileBytes = await _exportService.ExportAsync(viewModels, columns, ExportFormat.Pdf, "Gym Members Report");
        return File(fileBytes, "application/pdf", $"members_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
    }

    private static void SplitName(string fullName, out string firstName, out string lastName)
    {
        var parts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        firstName = parts[0];
        lastName = parts.Length > 1 ? parts[1] : "";
    }

    private static List<ColumnDefinition<MemberViewModel>> GetMemberColumnDefinitions()
    {
        return
        [
            new() { HeaderName = "ID", ValueSelector = m => m.Id },
            new() { HeaderName = "First Name", ValueSelector = m => m.FirstName },
            new() { HeaderName = "Last Name", ValueSelector = m => m.LastName },
            new() { HeaderName = "Email", ValueSelector = m => m.Email },
            new() { HeaderName = "Phone", ValueSelector = m => m.PhoneNumber }
        ];
    }
}
