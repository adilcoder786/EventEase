using EventEase.Models;

namespace EventEase.Services;

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync(CancellationToken ct = default);
}
