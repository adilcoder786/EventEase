using EventEase.Data;
using EventEase.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(IDbContextFactory<ApplicationDbContext> factory, ILogger<RegistrationService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    /// <summary>
    /// Registers a participant. Checks (inside one transaction): event exists, not over,
    /// no duplicate (same email + event) and a free seat. Then reduces AvailableSeats.
    /// </summary>
    public async Task<ServiceResult<Registration>> RegisterAsync(Registration model)
    {
        var email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();

        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();

            var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == model.EventId);
            if (ev is null)
                return ServiceResult<Registration>.Fail("The selected event no longer exists.");

            if (ev.EventDate.Date < DateTime.Today)
                return ServiceResult<Registration>.Fail("This event has already taken place.");

            if (await db.Registrations.AnyAsync(r => r.EventId == ev.Id && r.Email == email))
                return ServiceResult<Registration>.Fail("This email address is already registered for this event.");

            if (ev.AvailableSeats <= 0)
                return ServiceResult<Registration>.Fail("Sorry, this event is fully booked.");

            var registration = new Registration
            {
                FullName = model.FullName.Trim(),
                Email = email,
                PhoneNumber = model.PhoneNumber.Trim(),
                EventId = ev.Id,
                RegistrationDate = DateTime.Now,
                Attendance = AttendanceStatus.Pending
            };

            db.Registrations.Add(registration);
            ev.AvailableSeats--;

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            return ServiceResult<Registration>.Ok(registration, $"You're registered for {ev.Name}!");
        }
        catch (DbUpdateException ex)
        {
            // Unique index fired: two identical requests raced each other.
            _logger.LogWarning(ex, "Registration conflict for {Email} on event {EventId}", email, model.EventId);
            return ServiceResult<Registration>.Fail("This email may already be registered, or the event just filled up. Please refresh and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for {Email} on event {EventId}", email, model.EventId);
            return ServiceResult<Registration>.Fail("Something went wrong while saving your registration. Please try again.");
        }
    }

    public async Task<List<Registration>> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Registrations.AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.Email == normalized)
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync(ct);
    }

    public async Task<ServiceResult> CancelAsync(int registrationId, string email)
    {
        var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            var reg = await db.Registrations.Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == registrationId && r.Email == normalized);

            if (reg is null)
                return ServiceResult.Fail("Registration not found.");

            if (reg.Event is not null)
            {
                if (reg.Event.EventDate.Date < DateTime.Today)
                    return ServiceResult.Fail("Registrations for past events cannot be cancelled.");

                reg.Event.AvailableSeats = Math.Min(reg.Event.MaxCapacity, reg.Event.AvailableSeats + 1);
            }

            db.Registrations.Remove(reg);
            await db.SaveChangesAsync();
            return ServiceResult.Ok("Your registration has been cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel registration {Id}", registrationId);
            return ServiceResult.Fail("We couldn't cancel the registration. Please try again.");
        }
    }

    public async Task<PagedResult<Registration>> GetForAttendanceAsync(int? eventId, int page, int pageSize, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        IQueryable<Registration> q = db.Registrations.AsNoTracking().Include(r => r.Event);

        if (eventId.HasValue && eventId.Value > 0)
        {
            var id = eventId.Value;
            q = q.Where(r => r.EventId == id);
        }

        var total = await q.CountAsync(ct);
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var items = await q
            .OrderBy(r => r.Event!.Name).ThenBy(r => r.FullName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Registration> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<AttendanceSummary> GetAttendanceSummaryAsync(int? eventId, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        IQueryable<Registration> q = db.Registrations.AsNoTracking();

        if (eventId.HasValue && eventId.Value > 0)
        {
            var id = eventId.Value;
            q = q.Where(r => r.EventId == id);
        }

        var counts = await q.GroupBy(r => r.Attendance)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        int Get(AttendanceStatus s) => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0;
        var present = Get(AttendanceStatus.Present);
        var absent = Get(AttendanceStatus.Absent);
        var pending = Get(AttendanceStatus.Pending);
        return new AttendanceSummary(present + absent + pending, present, absent, pending);
    }

    public async Task<ServiceResult> SetAttendanceAsync(int registrationId, AttendanceStatus status)
    {
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            var reg = await db.Registrations.FirstOrDefaultAsync(r => r.Id == registrationId);
            if (reg is null)
                return ServiceResult.Fail("Registration not found.");

            reg.Attendance = status;
            await db.SaveChangesAsync();
            return ServiceResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update attendance for {Id}", registrationId);
            return ServiceResult.Fail("We couldn't update attendance. Please try again.");
        }
    }
}
