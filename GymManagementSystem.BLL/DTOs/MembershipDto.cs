namespace GymManagementSystem.BLL.DTOs;

public class MembershipDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsActive { get; set; }
}
