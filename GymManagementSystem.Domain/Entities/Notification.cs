using System;

namespace GymManagementSystem.Domain;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
