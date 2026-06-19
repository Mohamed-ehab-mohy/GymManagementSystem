namespace GymManagementSystem.Domain;

public class Category : BaseEntity
{
    public string CategoryName { get; set; } = null!;

    public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
}
