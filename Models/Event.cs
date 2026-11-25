using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be 3-100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Event date is required.")]
    public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);

    [Required(ErrorMessage = "Start time is required.")]
    public TimeOnly StartTime { get; set; } = new TimeOnly(10, 0);

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50)]
    public string Category { get; set; } = "Conference";

    [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000.")]
    public int MaxCapacity { get; set; } = 50;

    [Range(0, 10000)]
    public int AvailableSeats { get; set; } = 50;

    [StringLength(300)]
    public string ImageUrl { get; set; } = EventCatalog.DefaultImage;

    public List<Registration> Registrations { get; set; } = new();

    // ----- Helpers (not stored in the database) -----
    public DateTime StartDateTime => EventDate.Date + StartTime.ToTimeSpan();
    public bool IsUpcoming => EventDate.Date >= DateTime.Today;
    public bool IsFull => AvailableSeats <= 0;
    public int RegisteredCount => Math.Max(0, MaxCapacity - AvailableSeats);
}
