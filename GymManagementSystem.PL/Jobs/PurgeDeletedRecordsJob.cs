using GymManagementSystem.BLL.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GymManagementSystem.PL.Jobs;

[DisallowConcurrentExecution]
public class PurgeDeletedRecordsJob : IJob
{
    private readonly IPurgeService _purgeService;
    private readonly ILogger<PurgeDeletedRecordsJob> _logger;

    public PurgeDeletedRecordsJob(IPurgeService purgeService, ILogger<PurgeDeletedRecordsJob> logger)
    {
        _purgeService = purgeService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var count = await _purgeService.PurgeAsync(30);
        _logger.LogInformation("Purged {Count} soft-deleted records.", count);
    }
}
