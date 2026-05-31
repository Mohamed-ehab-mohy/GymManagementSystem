using System;
using System.Collections.Generic;

namespace GymManagementSystem.DAL.Entities;

public class ClassSession : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime ScheduleTime { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }

    public int TrainerId { get; set; }
    public Trainer Trainer { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
