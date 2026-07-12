namespace GymManagementSystem.Domain;

public abstract class GymUser : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { set; get; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = Roles.Member;

    public Address Address { get; set; } = null!;
}
