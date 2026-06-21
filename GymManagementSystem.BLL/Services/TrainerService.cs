using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.BLL.Services;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IClassSessionRepository _classSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrainerService(ITrainerRepository repository, IClassSessionRepository classSessionRepository, IUnitOfWork unitOfWork)
    {
        _trainerRepository = repository;
        _classSessionRepository = classSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Trainer>> GetAllTrainersAsync()
    {
        return await _trainerRepository.GetAllAsync();
    }

    public async Task<PagedResult<Trainer>> GetPagedTrainersAsync(int page, int pageSize, string? search = null, string? sortBy = null, bool ascending = true)
    {
        Expression<Func<Trainer, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(search))
        {
            filter = t => t.FirstName.Contains(search) || t.LastName.Contains(search) || t.Email.Contains(search) || t.PhoneNumber.Contains(search) || t.Specialty.ToString().Contains(search);
        }
        return await _trainerRepository.GetPagedAsync(page, pageSize, filter, sortBy, ascending);
    }

    public async Task<Trainer?> GetTrainerByIdAsync(int id)
    {
        return await _trainerRepository.GetByIdAsync(id);
    }

    public async Task AddTrainerAsync(Trainer entity)
    {
        await _trainerRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateTrainerAsync(Trainer entity)
    {
        _trainerRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<(bool Success, string Message)> DeleteTrainerAsync(int id)
    {
        var entity = await _trainerRepository.GetByIdAsync(id);
        if (entity == null)
            return (false, "Trainer not found.");

        var activeSessions = await _classSessionRepository.GetByTrainerIdAsync(id);
        if (activeSessions.Any())
            return (false, "Cannot delete trainer. Active class sessions found.");

        _trainerRepository.Delete(entity);
        await _unitOfWork.CompleteAsync();
        return (true, "Trainer deleted successfully.");
    }
}
