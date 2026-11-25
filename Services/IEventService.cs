using EventEase.Models;

namespace EventEase.Services;

public interface IEventService
{
    Task<PagedResult<Event>> GetEventsAsync(EventQuery query, CancellationToken ct = default);
    Task<List<Event>> GetUpcomingAsync(int count, CancellationToken ct = default);
    Task<List<Event>> GetAllAsync(CancellationToken ct = default);
    Task<Event?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<Event>> CreateAsync(Event model);
    Task<ServiceResult> UpdateAsync(Event model);
    Task<ServiceResult> DeleteAsync(int id);
}
