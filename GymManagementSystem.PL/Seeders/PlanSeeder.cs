using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace GymManagementSystem.PL.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAllAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

        if (dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        var trainer = await dbContext.Trainers.FirstOrDefaultAsync();
        if (trainer == null)
        {
            trainer = new Trainer
            {
                FirstName = "Captain",
                LastName = "Adel",
                Email = "captain.adel@gymy.com",
                PhoneNumber = "01123456789",
                DateOfBirth = new DateTime(1985, 6, 15),
                Gender = "Male",
                Specialty = TrainerSpecialty.GeneralFitness,
                HireDate = DateTime.Today,
                Address = new Address { Street = "Gymy St", City = "Cairo", State = "Cairo", ZipCode = "12345" }
            };
            await dbContext.Trainers.AddAsync(trainer);
            await dbContext.SaveChangesAsync();
        }

        var member = await dbContext.Members.FirstOrDefaultAsync();
        if (member == null)
        {
            member = new Member
            {
                FirstName = "Mohamed",
                LastName = "Ehab",
                Email = "mohamed.ehab@gmail.com",
                PhoneNumber = "01098765432",
                DateOfBirth = new DateTime(1995, 3, 20),
                Gender = "Male",
                JoinDate = DateTime.Today,
                EmergencyContactName = "Parent",
                EmergencyContactPhone = "01234567890",
                Address = new Address { Street = "Home St", City = "Giza", State = "Giza", ZipCode = "54321" }
            };
            await dbContext.Members.AddAsync(member);
            await dbContext.SaveChangesAsync();
        }

        var member2 = await dbContext.Members.Skip(1).FirstOrDefaultAsync();
        if (member2 == null)
        {
            member2 = new Member
            {
                FirstName = "Aly",
                LastName = "Ashraf",
                Email = "aly.ashraf@gmail.com",
                PhoneNumber = "01012345678",
                DateOfBirth = new DateTime(1998, 7, 10),
                Gender = "Male",
                JoinDate = DateTime.Today,
                EmergencyContactName = "Friend",
                EmergencyContactPhone = "01112223334",
                Address = new Address { Street = "Rehab St", City = "Cairo", State = "Cairo", ZipCode = "11223" }
            };
            await dbContext.Members.AddAsync(member2);
            await dbContext.SaveChangesAsync();
        }

        var session = await dbContext.ClassSessions.FirstOrDefaultAsync(cs => cs.ScheduleTime.Date == DateTime.Today);
        if (session == null)
        {
            session = new ClassSession
            {
                Name = "Morning CrossFit",
                ScheduleTime = DateTime.Today,
                StartTime = DateTime.Today.AddHours(9),
                EndTime = DateTime.Today.AddHours(10),
                Capacity = 20,
                CategoryId = 5,
                TrainerId = trainer.Id
            };
            await dbContext.ClassSessions.AddAsync(session);
            await dbContext.SaveChangesAsync();
        }

        var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.MemberId == member.Id);
        if (booking == null)
        {
            booking = new Booking
            {
                MemberId = member.Id,
                ClassSessionId = session.Id,
                BookingDate = DateTime.Today,
                IsAttended = false
            };
            await dbContext.Bookings.AddAsync(booking);
            await dbContext.SaveChangesAsync();
        }

        var booking2 = await dbContext.Bookings.FirstOrDefaultAsync(b => b.MemberId == member2.Id);
        if (booking2 == null)
        {
            booking2 = new Booking
            {
                MemberId = member2.Id,
                ClassSessionId = session.Id,
                BookingDate = DateTime.Today,
                IsAttended = false
            };
            await dbContext.Bookings.AddAsync(booking2);
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Plans.AnyAsync())
        {
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var jsonPath = Path.Combine(env.ContentRootPath, "SeedData", "plans.json");
            if (File.Exists(jsonPath))
            {
                var json = await File.ReadAllTextAsync(jsonPath);
                var seedPlans = System.Text.Json.JsonSerializer.Deserialize<List<PlanSeedDto>>(json);
                if (seedPlans != null && seedPlans.Count > 0)
                {
                    var plans = seedPlans.Select(p => new Plan
                    {
                        Name = p.Name,
                        Description = p.Description,
                        DurationDays = p.DurationDays,
                        Price = p.Price,
                        IsActive = p.IsActive
                    }).ToList();

                    await dbContext.Plans.AddRangeAsync(plans);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        await SeedAdminAsync(scope);
    }

    private static async Task SeedAdminAsync(IServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in new[] { "User", "Admin", "SuperAdmin" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        await EnsureUserAsync(userManager, "admin@gymy.com", "Admin@123", "Admin");
        await EnsureUserAsync(userManager, "superadmin@gymy.com", "Super@123", "SuperAdmin");
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
            Log.Information("User {Email} created with role {Role}", email, role);
        }
    }

    private class PlanSeedDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
