using System;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using Quartz;

namespace GymManagementSystem.PL.Jobs;

[DisallowConcurrentExecution]
public class PurgeDeletedRecordsJob : IJob
{
    private readonly IPurgeService _purgeService;

    public PurgeDeletedRecordsJob(IPurgeService purgeService)
    {
        _purgeService = purgeService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var count = await _purgeService.PurgeAsync(30);
        Console.WriteLine($"[PurgeJob] Purged {count} soft-deleted records.");
    }
}
