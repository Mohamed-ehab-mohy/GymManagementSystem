using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Abstractions.Repositories;

namespace GymManagementSystem.BLL.Services;

public class ClassSessionService : IClassSessionService
{
    private readonly IClassSessionRepository _classSessionRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClassSessionService(IClassSessionRepository repository, IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
    {
        _classSessionRepository = repository;
        _bookingRepository = bookingRepository;
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

    public async Task<(bool Success, string Message)> DeleteClassSessionAsync(int id)
    {
        var entity = await _classSessionRepository.GetByIdAsync(id);
        if (entity == null)
            return (false, "Session not found.");

        var bookings = await _bookingRepository.GetBySessionIdAsync(id);
        if (bookings.Any())
            return (false, "Cannot delete session. Active bookings found.");

        _classSessionRepository.Delete(entity);
        await _unitOfWork.CompleteAsync();
        return (true, "Session deleted successfully.");
    }
}
