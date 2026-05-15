using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagementSystem.PL.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

        await dbContext.Database.MigrateAsync(); // applies pending migrations

        if (await dbContext.Plans.AnyAsync())
            return;

        var plans = new List<Plan>
        {
            new() {
                Name = "Basic Plan",
                Description = "Access to gym equipment during staffed hours.",
                DurationDays = 30,
                Price = 300,
                IsActive = true
            },
            new() {
                Name = "Standard Plan",
                Description = "Includes gym equipment and 2 group classes per week.",
                DurationDays = 60,
                Price = 500,
                IsActive = true
            },
            new() {
                Name = "Premium Plan",
                Description = "Unlimited access to equipment, classes, and sauna.",
                DurationDays = 90,
                Price = 900,
                IsActive = true
            },
            new() {
                Name = "Annual Plan",
                Description = "Full year access with personal trainer sessions.",
                DurationDays = 365,
                Price = 3000,
                IsActive = true
            }
        };

        await dbContext.Plans.AddRangeAsync(plans);
        await dbContext.SaveChangesAsync();
    }
}