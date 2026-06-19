using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.PL.Services;

public class RenewalReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RenewalReminderService> _logger;

    public RenewalReminderService(IServiceScopeFactory scopeFactory, ILogger<RenewalReminderService> logger)
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
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var membershipRepo = scope.ServiceProvider.GetRequiredService<IRepository<Membership>>();

                var cutoff = DateTime.Today.AddDays(7);
                var expiringSoon = await membershipRepo.Query()
                    .Where(m => m.IsActive && !m.IsDeleted && m.EndDate <= cutoff && m.EndDate >= DateTime.Today)
                    .ToListAsync(stoppingToken);

                foreach (var membership in expiringSoon)
                {
                    var daysLeft = (membership.EndDate - DateTime.Today).Days;
                    await notificationService.SendToUserAsync(
                        membership.MemberId,
                        $"Your membership expires in {daysLeft} day(s). Renew now to keep enjoying Power Fitness!");
                }

                if (expiringSoon.Count > 0)
                    _logger.LogInformation("Sent {Count} renewal reminders.", expiringSoon.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending renewal reminders.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
