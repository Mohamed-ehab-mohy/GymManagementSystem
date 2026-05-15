using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.BLL.Services;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrainerService(ITrainerRepository repository, IUnitOfWork unitOfWork)
    {
        _trainerRepository = repository;
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

    public async Task DeleteTrainerAsync(int id)
    {
        var entity = await _trainerRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _trainerRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
