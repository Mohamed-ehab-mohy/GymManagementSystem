using System;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.PL.Services;

public class PurgeService : IPurgeService
{
    private readonly GymDbContext _context;

    public PurgeService(GymDbContext context)
    {
        _context = context;
    }

    public async Task<int> PurgeAsync(int olderThanDays = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-olderThanDays);
        var total = 0;

        var softDeletableSets = _context.Model.GetEntityTypes()
            .Where(et => typeof(ISoftDeletable).IsAssignableFrom(et.ClrType))
            .Select(et => et.ClrType)
            .ToList();

        foreach (var type in softDeletableSets)
        {
            var method = typeof(PurgeService)
                .GetMethod(nameof(PurgeEntityAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.MakeGenericMethod(type);

            if (method != null)
            {
                var result = await (Task<int>)method.Invoke(this, new object[] { cutoff })!;
                total += result;
            }
        }

        return total;
    }

    private async Task<int> PurgeEntityAsync<T>(DateTime cutoff) where T : class, ISoftDeletable
    {
        var dbSet = _context.Set<T>();
        var toDelete = await dbSet
            .Where(e => e.IsDeleted && e.DeletedAt != null && e.DeletedAt < cutoff)
            .IgnoreQueryFilters()
            .ToListAsync();

        if (toDelete.Count == 0) return 0;

        dbSet.RemoveRange(toDelete);
        return await _context.SaveChangesAsync();
    }
}
