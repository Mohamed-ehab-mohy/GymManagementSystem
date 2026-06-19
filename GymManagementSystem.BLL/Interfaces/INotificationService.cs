using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Interfaces;

public interface INotificationService
{
    Task SendToUserAsync(int userId, string message);
    Task<IEnumerable<NotificationDto>> GetUnreadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
