namespace GymManagementSystem.BLL.DTOs;

public class TrainerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public DateOnly? HireDate { get; set; }
}
