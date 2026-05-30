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

        await dbContext.Database.MigrateAsync();

        var trainer = await dbContext.Trainers.FirstOrDefaultAsync();
        if (trainer == null)
        {
            trainer = new Trainer
            {
                FirstName = "Captain",
                LastName = "Adel",
                Email = "captain.adel@gymy.com",
                PhoneNumber = "01123456789",
                Specialization = "Bodybuilding",
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