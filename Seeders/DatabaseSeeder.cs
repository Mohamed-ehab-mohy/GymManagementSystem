namespace GymManagementSystem.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync()
        {
            await PlanSeeder.SeedAsync();
            // await MemberSeeder.SeedAsync();
        }
    }
}