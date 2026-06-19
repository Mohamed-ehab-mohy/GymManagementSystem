using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Domain;
using GymManagementSystem.DAL.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GymManagementSystem.DAL.DbContexts;

public class GymDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Plan> Plans { get; set; }
    public DbSet<GymUser> GymUsers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }
    public DbSet<ClassSession> ClassSessions { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Category> Categories { get; set; }

    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PlanConfiguration());
        modelBuilder.ApplyConfiguration(new GymUserConfiguration());
        modelBuilder.ApplyConfiguration(new HealthRecordConfiguration());
        modelBuilder.ApplyConfiguration(new ClassSessionConfiguration());
        modelBuilder.ApplyConfiguration(new MembershipConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new MemberConfiguration());
        modelBuilder.ApplyConfiguration(new TrainerConfiguration());

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName != null && tableName.StartsWith("AspNet"))
                entity.SetTableName(tableName[6..]);
        }
    }
}
