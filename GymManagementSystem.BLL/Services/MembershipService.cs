using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.BLL.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MembershipService(IMembershipRepository repository, IUnitOfWork unitOfWork)
    {
        _membershipRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
    {
        return await _membershipRepository.GetAllAsync();
    }

    public async Task<Membership?> GetMembershipByIdAsync(int id)
    {
        return await _membershipRepository.GetByIdAsync(id);
    }

    public async Task AddMembershipAsync(Membership entity)
    {
        await _membershipRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateMembershipAsync(Membership entity)
    {
        _membershipRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteMembershipAsync(int id)
    {
        var entity = await _membershipRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _membershipRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
