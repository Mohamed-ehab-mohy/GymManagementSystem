using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GymManagementSystem.BLL.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);
    private readonly ILogger _logger;

    public PlanService(IPlanRepository planRepository, IMembershipRepository membershipRepository, IUnitOfWork unitOfWork, ICacheService cache, ILogger<PlanService>? logger = null)
    {
        _planRepository = planRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger ?? NullLogger<PlanService>.Instance;
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
        if (_cache.TryGet<IEnumerable<Plan>>(CacheKeys.ActivePlans, out var cached))
            return cached!;

        var plans = await _planRepository.GetAllAsync();
        _cache.Set(CacheKeys.ActivePlans, plans, CacheDuration);
        return plans;
    }

    public async Task<Plan?> GetPlanByIdAsync(int id)
    {
        var key = CacheKeys.Plan(id);
        if (_cache.TryGet<Plan>(key, out var cached))
            return cached;

        var plan = await _planRepository.GetByIdAsync(id);
        if (plan != null)
            _cache.Set(key, plan, CacheDuration);
        return plan;
    }

    public async Task CreatePlanAsync(Plan plan)
    {
        await _planRepository.AddAsync(plan);
        await _unitOfWork.CompleteAsync();
        _cache.Remove(CacheKeys.ActivePlans);
    }

    public async Task UpdatePlanAsync(Plan plan)
    {
        _planRepository.Update(plan);
        await _unitOfWork.CompleteAsync();
        _cache.Remove(CacheKeys.ActivePlans);
        _cache.Remove(CacheKeys.Plan(plan.Id));
    }

    public async Task DeletePlanAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan != null)
        {
            _planRepository.Delete(plan);
            await _unitOfWork.CompleteAsync();
            _cache.Remove(CacheKeys.ActivePlans);
            _cache.Remove(CacheKeys.Plan(id));
        }
    }

    public async Task<(bool Success, string Message)> ToggleActiveAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
            return (false, "Plan not found.");

        if (plan.IsActive)
        {
            var activeMemberships = await _membershipRepository.GetActiveByPlanIdAsync(id);
            if (activeMemberships.Any())
                return (false, "Cannot deactivate plan. Active memberships found.");
        }

        plan.IsActive = !plan.IsActive;
        _planRepository.Update(plan);
        await _unitOfWork.CompleteAsync();

        _cache.Remove(CacheKeys.ActivePlans);
        _cache.Remove(CacheKeys.Plan(id));

        var action = plan.IsActive ? "activated" : "deactivated";
        _logger.LogInformation("Plan {PlanId} ({Name}) {Action}", id, plan.Name, action);

        return (true, plan.IsActive ? "Plan activated successfully." : "Plan deactivated successfully.");
    }
}
