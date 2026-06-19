namespace GymManagementSystem.Domain;

public class HealthRecord : BaseEntity
{
    public string BloodType { get; set; } = null!;
    public string? MedicalConditions { get; set; }
    public string? Note { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public DateTime LastUpdate { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
}
