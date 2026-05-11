using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.DbContexts;
using GymManagementSystem.Models;

namespace GymManagementSystem.Controllers;

public class PlansController : Controller
{
    private readonly GymDbContext _context;

    public PlansController(GymDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var plans = await _context.Plans.ToListAsync();
        return View(plans);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var plan = await _context.Plans.FirstOrDefaultAsync(m => m.Id == id);
        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }

    // GET: Plans/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Plans/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,DurationDays,Price,IsActive")] Plan plan)
    {
        if (ModelState.IsValid)
        {
            plan.CreatedAt = DateTime.UtcNow;
            _context.Add(plan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }

    // GET: Plans/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
        {
            return NotFound();
        }
        return View(plan);
    }

    // POST: Plans/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DurationDays,Price,IsActive,CreatedAt")] Plan plan)
    {
        if (id != plan.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                plan.UpdatedAt = DateTime.UtcNow;
                _context.Update(plan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(plan.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }

    // POST: Plans/ToggleActive/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan != null)
        {
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool PlanExists(int id)
    {
        return _context.Plans.Any(e => e.Id == id);
    }
}
