using System;

namespace GymManagementSystem.Domain;

public class PasswordResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CodeHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
