using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Abstractions.Repositories;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Services;
using GymManagementSystem.BLL.Tests.Infrastructure;
using GymManagementSystem.Domain;
using NSubstitute;
using Shouldly;

namespace GymManagementSystem.BLL.Tests;

public class PlanServiceTests
{
    private readonly IPlanRepository _planRepo;
    private readonly IMembershipRepository _membershipRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly PlanService _planService;

    public PlanServiceTests()
    {
        _planRepo = Substitute.For<IPlanRepository>();
        _membershipRepo = Substitute.For<IMembershipRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _cache = Substitute.For<ICacheService>();
        _planService = new PlanService(_planRepo, _membershipRepo, _unitOfWork, _cache);
    }

    [Fact]
    public async Task ToggleActiveAsync_DeactivateWithActiveMemberships_ReturnsFailure()
    {
        var planId = 1;
        var plan = new Plan { Id = planId, Name = "Gold", IsActive = true };

        _planRepo.GetByIdAsync(planId).Returns(plan);
        _membershipRepo.GetActiveByPlanIdAsync(planId).Returns([
            new() { PlanId = planId, IsActive = true }
        ]);

        var (success, message) = await _planService.ToggleActiveAsync(planId);

        success.ShouldBeFalse();
        message.ShouldContain("Active memberships");
        plan.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task ToggleActiveAsync_DeactivateWithoutActiveMemberships_Succeeds()
    {
        var planId = 1;
        var plan = new Plan { Id = planId, Name = "Gold", IsActive = true };

        _planRepo.GetByIdAsync(planId).Returns(plan);
        _membershipRepo.GetActiveByPlanIdAsync(planId).Returns([]);

        var (success, message) = await _planService.ToggleActiveAsync(planId);

        success.ShouldBeTrue();
        plan.IsActive.ShouldBeFalse();
        _planRepo.Received(1).Update(plan);
        await _unitOfWork.Received(1).CompleteAsync();
        _cache.Received(1).Remove(CacheKeys.ActivePlans);
    }

    [Fact]
    public async Task ToggleActiveAsync_NonExistentPlan_ReturnsFailure()
    {
        _planRepo.GetByIdAsync(99).Returns((Plan?)null);

        var (success, message) = await _planService.ToggleActiveAsync(99);

        success.ShouldBeFalse();
        message.ShouldBe("Plan not found.");
    }

    [Fact]
    public async Task GetAllPlansAsync_CacheHit_ReturnsFromCache()
    {
        var cachedPlans = new List<Plan> { new() { Id = 1, Name = "Gold" } };
        _cache.TryGet<IEnumerable<Plan>>(CacheKeys.ActivePlans, out var _)
            .Returns(x =>
            {
                x[1] = cachedPlans;
                return true;
            });

        var result = await _planService.GetAllPlansAsync();

        result.ShouldBe(cachedPlans);
        await _planRepo.DidNotReceive().GetAllAsync();
    }

    [Fact]
    public async Task GetAllPlansAsync_CacheMiss_FetchesFromDbAndCaches()
    {
        var plans = new List<Plan> { new() { Id = 1, Name = "Gold" } };
        _cache.TryGet<IEnumerable<Plan>>(CacheKeys.ActivePlans, out var _).Returns(false);
        _planRepo.GetAllAsync().Returns(plans);

        var result = await _planService.GetAllPlansAsync();

        result.ShouldBe(plans);
        _cache.Received(1).Set(CacheKeys.ActivePlans, plans, Arg.Any<TimeSpan>());
    }
}
