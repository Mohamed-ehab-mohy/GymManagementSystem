namespace GymManagementSystem.BLL.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public DateOnly JoinDate { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public double Height { get; set; }
    public double Weight { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string? Note { get; set; }
}
