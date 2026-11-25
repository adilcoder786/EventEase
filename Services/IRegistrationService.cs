using EventEase.Models;

namespace EventEase.Services;

public interface IRegistrationService
{
    Task<ServiceResult<Registration>> RegisterAsync(Registration model);
    Task<List<Registration>> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<ServiceResult> CancelAsync(int registrationId, string email);
    Task<PagedResult<Registration>> GetForAttendanceAsync(int? eventId, int page, int pageSize, CancellationToken ct = default);
    Task<AttendanceSummary> GetAttendanceSummaryAsync(int? eventId, CancellationToken ct = default);
    Task<ServiceResult> SetAttendanceAsync(int registrationId, AttendanceStatus status);
}
