using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Services;

public class CleanupBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanupBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public CleanupBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CleanupBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();
                var cutoffDate = DateTime.UtcNow.AddDays(-30);

                var softDeletableTypes = context.Model.GetEntityTypes()
                    .Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType))
                    .Select(e => e.ClrType)
                    .ToList();

                int totalDeleted = 0;

                foreach (var entityType in softDeletableTypes)
                {
                    try
                    {
                        var method = typeof(CleanupBackgroundService)
                            .GetMethod(nameof(PurgeEntityTypeAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            ?.MakeGenericMethod(entityType);

                        if (method != null)
                        {
                            var deleted = await (Task<int>)method.Invoke(this, new object[] { context, cutoffDate })!;
                            totalDeleted += deleted;

                            if (deleted > 0)
                                _logger.LogInformation("Permanently removed {count} expired records from {table}", deleted, entityType.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not cleanup entity {entity}", entityType.Name);
                    }
                }

                if (totalDeleted > 0)
                    _logger.LogInformation("Maintenance cleanup completed. Total records removed: {total}", totalDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background maintenance task failed");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task<int> PurgeEntityTypeAsync<T>(GymDbContext context, DateTime cutoff) where T : class, ISoftDeletable
    {
        var dbSet = context.Set<T>();
        var toDelete = await dbSet
            .Where(e => e.IsDeleted && e.DeletedAt != null && e.DeletedAt < cutoff)
            .IgnoreQueryFilters()
            .ToListAsync();

        if (toDelete.Count == 0) return 0;

        dbSet.RemoveRange(toDelete);
        return await context.SaveChangesAsync();
    }
}
