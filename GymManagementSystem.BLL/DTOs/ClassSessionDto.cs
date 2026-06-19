namespace GymManagementSystem.BLL.DTOs;

public class ClassSessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduleTime { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public int AvailableSlots { get; set; }
    public int TrainerId { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
