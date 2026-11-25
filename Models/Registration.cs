using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class Registration
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\+?[0-9\s\-]{7,15}$",
        ErrorMessage = "Enter a valid phone number: 7-15 characters (digits, spaces, dashes, optional leading +).")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Please select an event.")]
    public int EventId { get; set; }

    public Event? Event { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    public AttendanceStatus Attendance { get; set; } = AttendanceStatus.Pending;
}
