using Microsoft.EntityFrameworkCore;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Configurations;

namespace GymManagementSystem.DAL.DbContexts;

public class GymDbContext : DbContext
{
    public DbSet<Plan> Plans { get; set; }
    public DbSet<GymUser> GymUsers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }
    public DbSet<ClassSession> ClassSessions { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Booking> Bookings { get; set; }

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
    }
}
