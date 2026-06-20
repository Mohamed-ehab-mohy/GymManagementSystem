using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace GymManagementSystem.PL.Jobs;

[DisallowConcurrentExecution]
public class RenewalReminderJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RenewalReminderJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var membershipRepo = scope.ServiceProvider.GetRequiredService<IRepository<Membership>>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RenewalReminderJob>>();

        var now = DateTime.UtcNow.Date;

        var expiring = await membershipRepo.Query()
            .Include(m => m.Member)
            .Include(m => m.Plan)
            .Where(m => m.IsActive && !m.IsDeleted && m.EndDate >= now)
            .ToListAsync();

        int sentCount = 0;

        foreach (var m in expiring)
        {
            int daysLeft = (m.EndDate - now).Days;

            if (daysLeft == 7 && m.ReminderDaysSent != 7)
            {
                await emailService.SendRenewalReminderAsync(m.Member.Email, 7, m.Plan.Name);
                await notificationService.SendToUserAsync(m.MemberId, $"Your {m.Plan.Name} membership expires in 7 days. Renew now!");
                m.ReminderDaysSent = 7;
                m.ReminderSentAt = DateTime.UtcNow;
                sentCount++;
            }
            else if (daysLeft == 1 && m.ReminderDaysSent != 1)
            {
                await emailService.SendRenewalReminderAsync(m.Member.Email, 1, m.Plan.Name);
                await notificationService.SendToUserAsync(m.MemberId, $"Your {m.Plan.Name} membership expires TOMORROW! Renew now!");
                m.ReminderDaysSent = 1;
                m.ReminderSentAt = DateTime.UtcNow;
                sentCount++;
            }
        }

        if (sentCount > 0)
        {
            await uow.CompleteAsync();
            logger.LogInformation("Sent {Count} renewal reminders.", sentCount);
        }
    }
}
