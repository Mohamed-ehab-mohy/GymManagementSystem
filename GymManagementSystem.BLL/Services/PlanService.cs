using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlanService(IPlanRepository planRepository, IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
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
}
