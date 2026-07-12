using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Abstractions;
using GymManagementSystem.BLL.Interfaces;
using GymManagementSystem.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.BLL.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IHubContext<Hub> _hubContext;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IRepository<Notification> notificationRepository,
        IHubContext<Hub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(int userId, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.CompleteAsync();

        await _hubContext.Clients.Group($"user-{userId}")
            .SendAsync("ReceiveNotification", message);
    }

    public async Task<IEnumerable<NotificationDto>> GetUnreadAsync(int userId)
    {
        return await _notificationRepository.Query()
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            })
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _notificationRepository.Query()
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            _notificationRepository.Update(notification);
            await _unitOfWork.CompleteAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var unread = await _notificationRepository.Query()
            .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
            .ToListAsync();

        foreach (var n in unread)
        {
            n.IsRead = true;
            _notificationRepository.Update(n);
        }

        await _unitOfWork.CompleteAsync();
    }
}
