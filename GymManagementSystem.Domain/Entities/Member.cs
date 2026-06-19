using System.Collections.Generic;

namespace GymManagementSystem.Domain;

public class Member : GymUser
{
    public string? Photo { get; set; }
    public DateTime JoinDate { get; set; }
    public string EmergencyContactName { get; set; } = null!;
    public string EmergencyContactPhone { get; set; } = null!;

    public HealthRecord? HealthRecord { get; set; }
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
