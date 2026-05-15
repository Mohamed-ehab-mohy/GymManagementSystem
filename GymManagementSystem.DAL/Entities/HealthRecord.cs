namespace GymManagementSystem.DAL.Entities;

public class HealthRecord : BaseEntity
{
    public string BloodType { get; set; } = null!;
    public string MedicalConditions { get; set; } = null!;
    public decimal Weight { get; set; }
    public decimal Height { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
}
