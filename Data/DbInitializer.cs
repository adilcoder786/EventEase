using EventEase.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Data;

/// <summary>
/// Creates/updates the database on startup and inserts sample data on the first run.
/// Sample dates are relative to "today", so the demo always has upcoming events.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await using var db = await factory.CreateDbContextAsync();

            // If you generated migrations (dotnet ef migrations add InitialCreate) apply them;
            // otherwise create the schema directly so the app still runs.
            if (db.Database.GetMigrations().Any())
                await db.Database.MigrateAsync();
            else
                await db.Database.EnsureCreatedAsync();

            if (await db.Events.AnyAsync())
                return;

            await SeedAsync(db);
            logger.LogInformation("Sample data inserted.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialisation failed.");
            throw;
        }
    }

    private static async Task SeedAsync(ApplicationDbContext db)
    {
        var today = DateTime.Today;

        var events = new List<Event>
        {
            new() { Name = "Tech Innovation Summit", Category = "Conference", ImageUrl = "/images/conference.svg",
                Description = "A full day of talks on AI, cloud and modern web development with industry speakers.",
                EventDate = today.AddDays(5), StartTime = new TimeOnly(9, 0), Location = "Patna Convention Centre", MaxCapacity = 200 },
            new() { Name = "Blazor Hands-On Workshop", Category = "Workshop", ImageUrl = "/images/workshop.svg",
                Description = "Build a Blazor app from scratch. Bring your laptop and learn by doing.",
                EventDate = today.AddDays(9), StartTime = new TimeOnly(10, 30), Location = "Innovation Lab, Room 204", MaxCapacity = 40 },
            new() { Name = "Startup Networking Evening", Category = "Networking", ImageUrl = "/images/networking.svg",
                Description = "Meet founders, mentors and investors over an informal evening of conversations.",
                EventDate = today.AddDays(12), StartTime = new TimeOnly(18, 0), Location = "Riverside Cafe Hall", MaxCapacity = 80 },
            new() { Name = "Data Science Bootcamp", Category = "Education", ImageUrl = "/images/workshop.svg",
                Description = "Intro to data analysis, visualisation and machine learning basics.",
                EventDate = today.AddDays(15), StartTime = new TimeOnly(9, 30), Location = "City University Auditorium", MaxCapacity = 60 },
            new() { Name = "Community Music Festival", Category = "Entertainment", ImageUrl = "/images/entertainment.svg",
                Description = "Live local bands, food stalls and a relaxed weekend for the whole family.",
                EventDate = today.AddDays(20), StartTime = new TimeOnly(16, 0), Location = "Gandhi Maidan Grounds", MaxCapacity = 500 },
            new() { Name = "Career Fair 2026", Category = "Career", ImageUrl = "/images/career.svg",
                Description = "Meet recruiters from 40+ companies. Bring printed copies of your resume.",
                EventDate = today.AddDays(25), StartTime = new TimeOnly(11, 0), Location = "Exhibition Hall B", MaxCapacity = 300 },
            new() { Name = "Open Source Meetup", Category = "Networking", ImageUrl = "/images/networking.svg",
                Description = "Lightning talks and pair-programming on open source projects. Beginners welcome.",
                EventDate = today.AddDays(3), StartTime = new TimeOnly(17, 30), Location = "Co-working Hub, Boring Road", MaxCapacity = 30 },
            new() { Name = "Cloud Fundamentals Day (Past)", Category = "Workshop", ImageUrl = "/images/workshop.svg",
                Description = "A beginner-friendly day of cloud basics. This event has already taken place.",
                EventDate = today.AddDays(-10), StartTime = new TimeOnly(10, 0), Location = "Tech Park, Hall 1", MaxCapacity = 25 }
        };

        foreach (var e in events) e.AvailableSeats = e.MaxCapacity;
        db.Events.AddRange(events);
        await db.SaveChangesAsync();

        // A few sample registrations so Dashboard and Attendance have data.
        var people = new[]
        {
            ("Aarav Sharma", "aarav@example.com", "+91 9000000001"),
            ("Diya Verma", "diya@example.com", "+91 9000000002"),
            ("Rohan Singh", "rohan@example.com", "+91 9000000003"),
            ("Ananya Gupta", "ananya@example.com", "+91 9000000004"),
            ("Kabir Mehta", "kabir@example.com", "+91 9000000005"),
            ("Ishita Rao", "ishita@example.com", "+91 9000000006")
        };

        var regs = new List<Registration>();
        var now = DateTime.Now;
        int minutes = 0;

        // Past event: attendance already recorded.
        var past = events[^1];
        for (int i = 0; i < people.Length; i++)
        {
            regs.Add(new Registration
            {
                EventId = past.Id, FullName = people[i].Item1, Email = people[i].Item2, PhoneNumber = people[i].Item3,
                RegistrationDate = now.AddDays(-14).AddMinutes(minutes += 37),
                Attendance = i < 4 ? AttendanceStatus.Present : AttendanceStatus.Absent
            });
        }

        // Upcoming events: pending attendance.
        for (int i = 0; i < 4; i++)
            regs.Add(new Registration { EventId = events[0].Id, FullName = people[i].Item1, Email = people[i].Item2,
                PhoneNumber = people[i].Item3, RegistrationDate = now.AddHours(-(30 - i * 5)) });
        for (int i = 0; i < 3; i++)
            regs.Add(new Registration { EventId = events[1].Id, FullName = people[i + 1].Item1, Email = people[i + 1].Item2,
                PhoneNumber = people[i + 1].Item3, RegistrationDate = now.AddHours(-(12 - i * 3)) });
        regs.Add(new Registration { EventId = events[6].Id, FullName = people[4].Item1, Email = people[4].Item2,
            PhoneNumber = people[4].Item3, RegistrationDate = now.AddHours(-2) });

        db.Registrations.AddRange(regs);

        foreach (var group in regs.GroupBy(r => r.EventId))
            events.First(e => e.Id == group.Key).AvailableSeats -= group.Count();

        await db.SaveChangesAsync();
    }
}
