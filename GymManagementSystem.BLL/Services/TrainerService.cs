using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

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
