using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;
using GymManagementSystem.Configurations;

namespace GymManagementSystem.DbContexts;

public class GymDbContext : DbContext
{
    public DbSet<Plan> Plans { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=.;Database=GymSystemTestDb;
             Trusted_Connection = true;
        TrustServerCertificate = true"
        );
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(
            new PlanConfiguration()
        );
    }
}