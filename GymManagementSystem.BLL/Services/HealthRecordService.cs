using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.BLL.Services;

public class HealthRecordService : IHealthRecordService
{
    private readonly IHealthRecordRepository _healthRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HealthRecordService(IHealthRecordRepository repository, IUnitOfWork unitOfWork)
    {
        _healthRecordRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<HealthRecord>> GetAllHealthRecordsAsync()
    {
        return await _healthRecordRepository.GetAllAsync();
    }

    public async Task<HealthRecord?> GetHealthRecordByIdAsync(int id)
    {
        return await _healthRecordRepository.GetByIdAsync(id);
    }

    public async Task AddHealthRecordAsync(HealthRecord entity)
    {
        await _healthRecordRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateHealthRecordAsync(HealthRecord entity)
    {
        _healthRecordRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteHealthRecordAsync(int id)
    {
        var entity = await _healthRecordRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _healthRecordRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
