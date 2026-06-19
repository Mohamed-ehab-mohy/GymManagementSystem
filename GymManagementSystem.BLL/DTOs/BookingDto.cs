namespace GymManagementSystem.BLL.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int ClassSessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public DateOnly BookingDate { get; set; }
    public bool IsAttended { get; set; }
    public DateTime? CheckedInAt { get; set; }
}
