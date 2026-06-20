using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Domain;
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
    public DbSet<Category> Categories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

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
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new MemberConfiguration());
        modelBuilder.ApplyConfiguration(new TrainerConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());
    }
}
