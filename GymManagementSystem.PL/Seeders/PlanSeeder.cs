using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.Domain;
using GymManagementSystem.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GymManagementSystem.PL.Seeders;

public static class DatabaseSeeder
{
    private static readonly Random Rng = new(42);

    private static readonly string[] MaleFirstNames =
    [
        "Mohamed", "Ahmed", "Mahmoud", "Ali", "Hassan", "Hussein", "Omar", "Khaled", "Youssef", "Amr",
        "Tarek", "Hossam", "Mostafa", "Ibrahim", "Sayed", "Nader", "Sameh", "Ashraf", "Waleed", "Shady",
        "Karim", "Sherif", "Ayman", "Gamal", "Hany", "Maged", "Raouf", "Magdy", "Ziad", "Fady",
        "Mina", "Peter", "George", "John", "Mark", "Andrew", "Bishoy", "Kirollos", "Micheal", "Youssef",
        "Adel", "Ramy", "Emad", "Nagy", "Samy", "Ashour", "Kamel", "Salah", "Hamdy", "Fawzy"
    ];

    private static readonly string[] FemaleFirstNames =
    [
        "Fatma", "Nour", "Mona", "Sara", "Donia", "Heba", "Rania", "Mariam", "Yasmin", "Laila",
        "Shaimaa", "Noha", "Dina", "Reem", "Nada", "Hagar", "Samar", "Esraa", "Asmaa", "Ghada",
        "Amira", "Salma", "Hana", "Nadia", "Sheren", "Wafaa", "Maha", "Mervat", "Safaa", "Faten"
    ];

    private static readonly string[] LastNames =
    [
        "Ehab", "Ashraf", "Ali", "Hassan", "Hussein", "Khalil", "Yousef", "Ahmed", "Sayed", "Omar",
        "Gamal", "Mostafa", "Nagy", "Samy", "Waleed", "Shaker", "Nader", "Fathy", "Samir", "Ezzat",
        "Rashad", "Kamel", "Monir", "Hamdy", "Fawzy", "Shawky", "El-Shafei", "Radwan", "Galal", "Tawfik",
        "Ramadan", "Sadek", "Basyouni", "Ghoneim", "El-Sayed", "Badr", "Hegazy", "Shokry", "Raafat", "Lotfy",
        "El-Gammal", "Zaki", "El-Sharkawy", "Mousa", "Fouad", "El-Desouky", "El-Gohary", "Saad", "El-Husseiny", "Morsi"
    ];

    private static readonly string[] Cities = ["Cairo", "Giza", "Alexandria", "Mansoura", "Tanta", "Ismailia", "Port Said", "Suez", "Luxor", "Aswan"];
    private static readonly string[] Streets =
    [
        "El Tahrir St", "El Nile St", "El Haram St", "El Nozha St", "El Hegaz St",
        "El Abbasia St", "El Safa St", "El Marwa St", "El Faisal St", "El Mohandeseen St",
        "El Zamalek St", "El Maadi St", "El Rehab St", "El Sherouk St", "El Obour St",
        "El Tagamoa St", "El Nasr St", "El Salam St", "El Geish St", "El Horreya St"
    ];

    private static readonly string[] BloodTypes = ["A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"];
    private static readonly string[] Specialties = ["GeneralFitness", "Yoga", "Boxing", "CrossFit"];

    private static readonly string[] SessionNames =
    [
        "Morning CrossFit", "Evening Yoga", "Cardio Blast", "Strength Training", "Boxing",
        "Pilates", "Zumba", "HIIT Circuit", "Body Pump", "Spinning",
        "Kickboxing", "Aerobics", "Aqua Fitness", "TRX", "Kettlebell",
        "Functional Training", "Stretching", "Step Aerobics", "Core Workout", "Tai Chi"
    ];

    private static readonly string[] CategoryNames = ["Cardio", "Strength", "Yoga", "Boxing", "Dance"];

    private static readonly string[] Descriptions =
    [
        "Access to gym equipment during staffed hours.",
        "Includes gym equipment and 2 group classes per week.",
        "Unlimited access to equipment, classes, and sauna.",
        "Full year access with personal trainer sessions.",
        "Weekend warrior pass for Saturday and Sunday access.",
        "Student discount plan with valid ID.",
        "Couple membership for two persons.",
        "Family plan for up to 4 family members.",
        "Corporate wellness program for companies.",
        "VIP plan with dedicated locker and towel service."
    ];

    public static async Task SeedAllAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

        if (dbContext.Database.IsRelational())
            await dbContext.Database.MigrateAsync();
        else
            await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Members.CountAsync() >= 100)
        {
            await SeedAdminAsync(scope);
            return;
        }

        var allCategories = await dbContext.Categories.ToListAsync();
        if (allCategories.Count == 0)
        {
            allCategories = CategoryNames.Select((name, i) => new Category
            {
                CategoryName = name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Seeder"
            }).ToList();
            dbContext.Categories.AddRange(allCategories);
            await dbContext.SaveChangesAsync();

            foreach (var c in allCategories)
                dbContext.Entry(c).State = EntityState.Detached;
        }

        var allPlans = await dbContext.Plans.ToListAsync();
        if (allPlans.Count < 100)
        {
            allPlans = [];
            for (int i = 1; i <= 100; i++)
            {
                var idx = Rng.Next(Descriptions.Length);
                allPlans.Add(new Plan
                {
                    Name = $"{PlanTierName(Rng.Next(4))} Plan #{i}",
                    Description = Descriptions[idx],
                    DurationDays = Rng.Next(1, 12) * 30,
                    Price = Rng.Next(10, 50) * 100,
                    IsActive = Rng.NextDouble() > 0.15
                });
            }
            dbContext.Plans.AddRange(allPlans);
            await dbContext.SaveChangesAsync();

            foreach (var p in allPlans)
                dbContext.Entry(p).State = EntityState.Detached;
        }

        var allTrainers = await dbContext.Trainers.IgnoreQueryFilters().ToListAsync();
        if (allTrainers.Count < 100)
        {
            var existingTrainers = allTrainers.Count;
            allTrainers = [.. allTrainers];
            for (int i = existingTrainers; i < 100; i++)
            {
                var email = $"trainer{i + 1}@gymy.com";
                var firstName = MaleFirstNames[Rng.Next(MaleFirstNames.Length)];
                var lastName = LastNames[Rng.Next(LastNames.Length)];
                allTrainers.Add(new Trainer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = $"01{Rng.Next(100000000, 999999999):D9}",
                    DateOfBirth = new DateTime(Rng.Next(1970, 2000), Rng.Next(1, 13), Rng.Next(1, 28)),
                    Gender = "Male",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Trainer@123", 12),
                    Role = "Trainer",
                    Specialty = (TrainerSpecialty)Rng.Next(4),
                    HireDate = DateTime.Today.AddDays(-Rng.Next(1, 1000)),
                    Address = new Address
                    {
                        Street = Streets[Rng.Next(Streets.Length)],
                        City = Cities[Rng.Next(Cities.Length)],
                        State = Cities[Rng.Next(Cities.Length)],
                        ZipCode = Rng.Next(10000, 99999).ToString()
                    }
                });
            }
            dbContext.Trainers.AddRange(allTrainers.Skip(existingTrainers));
            await dbContext.SaveChangesAsync();

            foreach (var t in allTrainers)
                dbContext.Entry(t).State = EntityState.Detached;
        }

        var allMembers = await dbContext.Members.IgnoreQueryFilters().ToListAsync();
        if (allMembers.Count < 100)
        {
            var existingMembers = allMembers.Count;
            allMembers = [.. allMembers];
            for (int i = existingMembers; i < 100; i++)
            {
                var isMale = Rng.NextDouble() > 0.4;
                var firstName = isMale ? MaleFirstNames[Rng.Next(MaleFirstNames.Length)] : FemaleFirstNames[Rng.Next(FemaleFirstNames.Length)];
                var lastName = LastNames[Rng.Next(LastNames.Length)];
                var gender = isMale ? "Male" : "Female";
                var email = $"member{i + 1}@gmail.com";

                allMembers.Add(new Member
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = $"01{Rng.Next(100000000, 999999999):D9}",
                    DateOfBirth = new DateTime(Rng.Next(1980, 2005), Rng.Next(1, 13), Rng.Next(1, 28)),
                    Gender = gender,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123", 12),
                    Role = "Member",
                    JoinDate = DateTime.Today.AddDays(-Rng.Next(1, 730)),
                    EmergencyContactName = $"{MaleFirstNames[Rng.Next(MaleFirstNames.Length)]} {LastNames[Rng.Next(LastNames.Length)]}",
                    EmergencyContactPhone = $"01{Rng.Next(100000000, 999999999):D9}",
                    Address = new Address
                    {
                        Street = Streets[Rng.Next(Streets.Length)],
                        City = Cities[Rng.Next(Cities.Length)],
                        State = Cities[Rng.Next(Cities.Length)],
                        ZipCode = Rng.Next(10000, 99999).ToString()
                    }
                });
            }
            dbContext.Members.AddRange(allMembers.Skip(existingMembers));
            await dbContext.SaveChangesAsync();

            foreach (var m in allMembers)
                dbContext.Entry(m).State = EntityState.Detached;
        }

        var healthRecords = await dbContext.HealthRecords.IgnoreQueryFilters().ToListAsync();
        if (healthRecords.Count < 100)
        {
            var existingHealth = healthRecords.Count;
            healthRecords = [.. healthRecords];
            for (int i = existingHealth; i < 100 && i < allMembers.Count; i++)
            {
                healthRecords.Add(new HealthRecord
                {
                    MemberId = allMembers[i].Id,
                    BloodType = BloodTypes[Rng.Next(BloodTypes.Length)],
                    Weight = Math.Round((decimal)(Rng.NextDouble() * 100 + 40), 1),
                    Height = Math.Round((decimal)(Rng.NextDouble() * 80 + 140), 1),
                    LastUpdate = DateTime.Today.AddDays(-Rng.Next(1, 90)),
                    Note = Rng.NextDouble() > 0.7 ? "No significant medical history." : null
                });
            }
            dbContext.HealthRecords.AddRange(healthRecords.Skip(existingHealth));
            await dbContext.SaveChangesAsync();
        }

        var allSessions = await dbContext.ClassSessions.IgnoreQueryFilters().ToListAsync();
        if (allSessions.Count < 100)
        {
            var existingSessions = allSessions.Count;
            allSessions = [.. allSessions];
            var today = DateTime.Today;

            for (int i = existingSessions; i < 100; i++)
            {
                var dayOffset = Rng.Next(0, 90);
                var hour = Rng.Next(6, 22);
                var minute = Rng.Next(0, 4) * 15;
                var start = today.AddDays(dayOffset).AddHours(hour).AddMinutes(minute);
                var end = start.AddHours(1);

                allSessions.Add(new ClassSession
                {
                    Name = SessionNames[Rng.Next(SessionNames.Length)],
                    ScheduleTime = start.Date,
                    StartTime = start,
                    EndTime = end,
                    Capacity = Rng.Next(5, 26),
                    CategoryId = allCategories[Rng.Next(allCategories.Count)].Id,
                    TrainerId = allTrainers[Rng.Next(allTrainers.Count)].Id
                });
            }
            dbContext.ClassSessions.AddRange(allSessions.Skip(existingSessions));
            await dbContext.SaveChangesAsync();

            foreach (var s in allSessions)
                dbContext.Entry(s).State = EntityState.Detached;
        }

        var memberships = await dbContext.Memberships.IgnoreQueryFilters().ToListAsync();
        if (memberships.Count < 100)
        {
            var existingMemberships = memberships.Count;
            memberships = [.. memberships];

            for (int i = existingMemberships; i < 100 && i < allMembers.Count; i++)
            {
                var plan = allPlans[Rng.Next(allPlans.Count)];
                var startDate = DateTime.Today.AddDays(-Rng.Next(1, 365));
                memberships.Add(new Membership
                {
                    MemberId = allMembers[i].Id,
                    PlanId = plan.Id,
                    StartDate = startDate,
                    EndDate = startDate.AddDays(plan.DurationDays),
                    IsActive = Rng.NextDouble() > 0.2
                });
            }
            dbContext.Memberships.AddRange(memberships.Skip(existingMemberships));
            await dbContext.SaveChangesAsync();

            foreach (var m in memberships)
                dbContext.Entry(m).State = EntityState.Detached;
        }

        var bookings = await dbContext.Bookings.IgnoreQueryFilters().ToListAsync();
        if (bookings.Count < 100)
        {
            var existingBookings = bookings.Count;
            bookings = [.. bookings];
            var usedPairs = new HashSet<string>();

            bool hasTodayUnattended = false;

            for (int i = existingBookings; i < 100; i++)
            {
                var member = allMembers[Rng.Next(allMembers.Count)];
                var session = allSessions[Rng.Next(allSessions.Count)];
                var key = $"{member.Id}:{session.Id}";

                if (usedPairs.Contains(key))
                    continue;

                usedPairs.Add(key);
                var sessionDate = session.ScheduleTime;
                var isPast = sessionDate < DateTime.Today;
                var isToday = sessionDate == DateTime.Today;

                var isAttended = isPast || (isToday && Rng.NextDouble() > 0.5);

                if (isToday && !isAttended)
                    hasTodayUnattended = true;

                bookings.Add(new Booking
                {
                    MemberId = member.Id,
                    ClassSessionId = session.Id,
                    BookingDate = sessionDate.AddDays(-Rng.Next(0, 5)),
                    IsAttended = isAttended,
                    CheckedInAt = isAttended && (isPast || isToday)
                        ? session.StartTime.AddMinutes(Rng.Next(0, 30))
                        : null
                });
            }

            if (!hasTodayUnattended)
            {
                var todaySession = allSessions.FirstOrDefault(s => s.ScheduleTime == DateTime.Today);
                if (todaySession == null)
                {
                    todaySession = new ClassSession
                    {
                        Name = "Daily Test Session",
                        ScheduleTime = DateTime.Today,
                        StartTime = DateTime.Today.AddHours(10),
                        EndTime = DateTime.Today.AddHours(11),
                        Capacity = 30,
                        CategoryId = allCategories[0].Id,
                        TrainerId = allTrainers[0].Id
                    };
                    dbContext.ClassSessions.Add(todaySession);
                    await dbContext.SaveChangesAsync();
                }

                var anyMember = allMembers[0];
                if (!usedPairs.Contains($"{anyMember.Id}:{todaySession.Id}"))
                {
                    bookings.Add(new Booking
                    {
                        MemberId = anyMember.Id,
                        ClassSessionId = todaySession.Id,
                        BookingDate = DateTime.Today,
                        IsAttended = false,
                        CheckedInAt = null
                    });
                }
            }

            dbContext.Bookings.AddRange(bookings.Skip(existingBookings));
            await dbContext.SaveChangesAsync();
        }

        await SeedAdminAsync(scope);
    }

    private static string PlanTierName(int tier) => tier switch
    {
        0 => "Basic",
        1 => "Standard",
        2 => "Premium",
        _ => "Ultimate"
    };

    private static async Task SeedAdminAsync(IServiceScope scope)
    {
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        var adminResult = await authService.RegisterAdminAsync("admin@gymy.com", "Admin@123", "Admin");
        if (adminResult.IsSuccess)
            Log.Information("Admin user admin@gymy.com created");

        var superResult = await authService.RegisterAdminAsync("superadmin@gymy.com", "Super@123", "SuperAdmin");
        if (superResult.IsSuccess)
            Log.Information("SuperAdmin user superadmin@gymy.com created");
    }
}
