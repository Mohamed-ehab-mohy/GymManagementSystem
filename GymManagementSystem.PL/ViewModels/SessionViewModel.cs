namespace GymManagementSystem.PL.ViewModels;

public class SessionViewModel
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public int Capacity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? TrainerName { get; set; }
    public string? CategoryName { get; set; }
    public int AvailableSlots { get; set; }

    public string DateDisplay => StartDate.ToString("dddd, MMMM dd, yyyy");
    public string TimeRangeDisplay => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
    public TimeSpan Duration => EndDate - StartDate;
    public string Status => DateTime.Now < StartDate ? "Upcoming" : DateTime.Now >= StartDate && DateTime.Now <= EndDate ? "Ongoing" : "Completed";
}
