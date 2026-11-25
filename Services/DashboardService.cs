using EventEase.Data;
using EventEase.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Services;

public class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public DashboardService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<DashboardStats> GetStatsAsync(CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var today = DateTime.Today;

        // A DbContext can only run one query at a time, so these run one after another.
        var totalEvents = await db.Events.CountAsync(ct);
        var totalRegistrations = await db.Registrations.CountAsync(ct);
        var upcomingCount = await db.Events.CountAsync(e => e.EventDate >= today, ct);

        var counts = await db.Registrations.AsNoTracking()
            .GroupBy(r => r.Attendance)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        int Get(AttendanceStatus s) => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0;
        var present = Get(AttendanceStatus.Present);
        var absent = Get(AttendanceStatus.Absent);
        var pending = Get(AttendanceStatus.Pending);

        var recent = await db.Registrations.AsNoTracking()
            .Include(r => r.Event)
            .OrderByDescending(r => r.RegistrationDate)
            .Take(8)
            .ToListAsync(ct);

        var upcoming = await db.Events.AsNoTracking()
            .Where(e => e.EventDate >= today)
            .OrderBy(e => e.EventDate).ThenBy(e => e.StartTime)
            .Take(5)
            .ToListAsync(ct);

        return new DashboardStats(
            totalEvents, totalRegistrations, upcomingCount,
            new AttendanceSummary(present + absent + pending, present, absent, pending),
            recent, upcoming);
    }
}
