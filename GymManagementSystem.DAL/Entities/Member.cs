using System.Collections.Generic;

namespace GymManagementSystem.DAL.Entities;

public class Member : GymUser
{
    public string EmergencyContactName { get; set; } = null!;
    public string EmergencyContactPhone { get; set; } = null!;

    // Navigation Properties
    public HealthRecord? HealthRecord { get; set; }
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
