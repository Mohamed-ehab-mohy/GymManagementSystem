using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories;

namespace GymManagementSystem.BLL.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IBookingRepository repository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<PagedResult<Booking>> GetPagedBookingsAsync(int page, int pageSize, string? search = null)
    {
        return await _bookingRepository.GetPagedBookingsAsync(page, pageSize, search);
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task AddBookingAsync(Booking entity)
    {
        await _bookingRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateBookingAsync(Booking entity)
    {
        _bookingRepository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteBookingAsync(int id)
    {
        var entity = await _bookingRepository.GetByIdAsync(id);
        if (entity != null)
        {
            _bookingRepository.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
