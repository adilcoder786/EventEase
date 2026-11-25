namespace EventEase.Models;

public record AttendanceSummary(int Total, int Present, int Absent, int Pending)
{
    /// <summary>Attendance percentage = present participants / all registered participants.</summary>
    public double Rate => Total == 0 ? 0 : Math.Round(Present * 100.0 / Total, 1);

    public static AttendanceSummary Empty => new(0, 0, 0, 0);
}

public record DashboardStats(
    int TotalEvents,
    int TotalRegistrations,
    int UpcomingEvents,
    AttendanceSummary Attendance,
    IReadOnlyList<Registration> RecentRegistrations,
    IReadOnlyList<Event> UpcomingList);
