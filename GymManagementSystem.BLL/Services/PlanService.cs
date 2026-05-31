using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.BLL.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlanService(IPlanRepository planRepository, IMembershipRepository membershipRepository, IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _membershipRepository = membershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
        return await _planRepository.GetAllAsync();
    }

    public async Task<Plan?> GetPlanByIdAsync(int id)
    {
        return await _planRepository.GetByIdAsync(id);
    }

    public async Task CreatePlanAsync(Plan plan)
    {
        await _planRepository.AddAsync(plan);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdatePlanAsync(Plan plan)
    {
        _planRepository.Update(plan);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeletePlanAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan != null)
        {
            _planRepository.Delete(plan);
            await _unitOfWork.CompleteAsync();
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

        return (true, plan.IsActive ? "Plan activated successfully." : "Plan deactivated successfully.");
    }
}
