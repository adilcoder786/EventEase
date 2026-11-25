using EventEase.Data;
using EventEase.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Services;

public class EventService : IEventService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly ILogger<EventService> _logger;

    public EventService(IDbContextFactory<ApplicationDbContext> factory, ILogger<EventService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<PagedResult<Event>> GetEventsAsync(EventQuery query, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var today = DateTime.Today;

        IQueryable<Event> q = db.Events.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            q = q.Where(e => EF.Functions.Like(e.Name, pattern)
                          || EF.Functions.Like(e.Location, pattern)
                          || EF.Functions.Like(e.Description, pattern));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category;
            q = q.Where(e => e.Category == category);
        }

        if (query.UpcomingOnly)
            q = q.Where(e => e.EventDate >= today);

        var total = await q.CountAsync(ct);
        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.PageSize, 1, 50);

        // Upcoming events first (soonest first), then past events.
        var items = await q
            .OrderBy(e => e.EventDate < today ? 1 : 0)
            .ThenBy(e => e.EventDate)
            .ThenBy(e => e.StartTime)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return new PagedResult<Event> { Items = items, TotalCount = total, Page = page, PageSize = size };
    }

    public async Task<List<Event>> GetUpcomingAsync(int count, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var today = DateTime.Today;
        return await db.Events.AsNoTracking()
            .Where(e => e.EventDate >= today)
            .OrderBy(e => e.EventDate).ThenBy(e => e.StartTime)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<List<Event>> GetAllAsync(CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Events.AsNoTracking()
            .OrderBy(e => e.EventDate).ThenBy(e => e.StartTime)
            .ToListAsync(ct);
    }

    public async Task<Event?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<ServiceResult<Event>> CreateAsync(Event model)
    {
        try
        {
            if (model.EventDate.Date < DateTime.Today)
                return ServiceResult<Event>.Fail("Event date cannot be in the past.");

            await using var db = await _factory.CreateDbContextAsync();
            var entity = new Event();
            Apply(entity, model);
            entity.AvailableSeats = entity.MaxCapacity; // nobody registered yet

            db.Events.Add(entity);
            await db.SaveChangesAsync();
            return ServiceResult<Event>.Ok(entity, "Event created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create event {Name}", model.Name);
            return ServiceResult<Event>.Fail("We couldn't create the event. Please try again.");
        }
    }

    public async Task<ServiceResult> UpdateAsync(Event model)
    {
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            var entity = await db.Events.FirstOrDefaultAsync(e => e.Id == model.Id);
            if (entity is null)
                return ServiceResult.Fail("This event no longer exists.");

            var registered = await db.Registrations.CountAsync(r => r.EventId == model.Id);
            if (model.MaxCapacity < registered)
                return ServiceResult.Fail($"Capacity cannot be lower than the {registered} existing registrations.");

            Apply(entity, model);
            entity.AvailableSeats = entity.MaxCapacity - registered; // keep seats consistent

            await db.SaveChangesAsync();
            return ServiceResult.Ok("Event updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update event {Id}", model.Id);
            return ServiceResult.Fail("We couldn't save your changes. Please try again.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            await using var db = await _factory.CreateDbContextAsync();
            var entity = await db.Events.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == id);
            if (entity is null)
                return ServiceResult.Fail("This event no longer exists.");

            db.Registrations.RemoveRange(entity.Registrations);
            db.Events.Remove(entity);
            await db.SaveChangesAsync();
            return ServiceResult.Ok("Event deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete event {Id}", id);
            return ServiceResult.Fail("We couldn't delete the event. Please try again.");
        }
    }

    private static void Apply(Event target, Event source)
    {
        target.Name = source.Name.Trim();
        target.Description = source.Description.Trim();
        target.EventDate = source.EventDate.Date;
        target.StartTime = source.StartTime;
        target.Location = source.Location.Trim();
        target.Category = source.Category;
        target.MaxCapacity = source.MaxCapacity;
        target.ImageUrl = string.IsNullOrWhiteSpace(source.ImageUrl) ? EventCatalog.DefaultImage : source.ImageUrl.Trim();
    }
}
