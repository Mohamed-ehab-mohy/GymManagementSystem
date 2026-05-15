using GymManagementSystem.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services;

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

                var entityTypes = context.Model.GetEntityTypes()
                    .Where(e => e.ClrType.GetProperty("IsDeleted") != null);

                int totalDeleted = 0;

                foreach (var entityType in entityTypes)
                {
                    var tableName = entityType.GetTableName();
                    var schema = entityType.GetSchema();
                    var fullTableName = string.IsNullOrEmpty(schema) ? $"[{tableName}]" : $"[{schema}].[{tableName}]";

                    try
                    {
                        var sql = $"DELETE FROM {fullTableName} WHERE IsDeleted = 1 AND UpdatedAt < @p0";
                        var deleted = await context.Database.ExecuteSqlRawAsync(sql, cutoffDate);
                        totalDeleted += deleted;

                        if (deleted > 0)
                            _logger.LogInformation("Permanently removed {count} expired records from {table}", deleted, fullTableName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not cleanup table {table}", fullTableName);
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
}
