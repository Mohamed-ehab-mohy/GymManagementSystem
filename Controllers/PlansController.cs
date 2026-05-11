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
}
