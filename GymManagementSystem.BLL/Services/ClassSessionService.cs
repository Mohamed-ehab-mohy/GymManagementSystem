using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.BLL.Services;

public class ClassSessionService : IClassSessionService
{
    private readonly IClassSessionRepository _classSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClassSessionService(IClassSessionRepository repository, IUnitOfWork unitOfWork)
    {
        _classSessionRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ClassSession>> GetAllClassSessionsAsync()
    {
        return await _classSessionRepository.GetAllAsync();
    }

    public async Task<ClassSession?> GetClassSessionByIdAsync(int id)
    {
        return await _classSessionRepository.GetByIdAsync(id);
    }

    public async Task AddClassSessionAsync(ClassSession entity)
    {
        await _classSessionRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateClassSessionAsync(ClassSession entity)
    {
        _classSessionRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteClassSessionAsync(int id)
    {
        var entity = await _classSessionRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _classSessionRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
