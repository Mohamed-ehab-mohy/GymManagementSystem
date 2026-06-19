namespace GymManagementSystem.PL.ViewModels;

public class AvailableSessionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ScheduleTime { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int AvailableSlots { get; set; }
}

public class MyBookingViewModel
{
    public int Id { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public DateTime ScheduleTime { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public bool IsAttended { get; set; }
}
