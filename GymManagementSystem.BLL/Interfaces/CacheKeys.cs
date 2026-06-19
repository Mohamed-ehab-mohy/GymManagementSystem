namespace GymManagementSystem.BLL.Interfaces;

public static class CacheKeys
{
    public const string ActivePlans = "plans:active";
    public const string AllCategories = "categories:all";
    public const string AllTrainers = "trainers:all";

    public static string Plan(int id) => $"plan:{id}";
    public static string MemberDashboard(int id) => $"dashboard:{id}";
}
