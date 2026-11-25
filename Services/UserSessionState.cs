namespace EventEase.Services;

/// <summary>
/// Holds the current visitor's session state for one browser tab (Blazor circuit):
/// their details (to pre-fill forms), saved events and events they registered for.
/// Components subscribe to <see cref="OnChange"/> so the UI updates automatically.
/// The session "expires" after a period of inactivity and personal data is cleared.
/// This is NOT authentication; it only remembers convenience data.
/// </summary>
public class UserSessionState
{
    private readonly HashSet<int> _savedEventIds = new();
    private readonly HashSet<int> _registeredEventIds = new();

    public UserSessionState(IConfiguration configuration)
    {
        var minutes = configuration.GetValue<int?>("Session:TimeoutMinutes") ?? 30;
        Timeout = TimeSpan.FromMinutes(Math.Max(1, minutes));
        StartedAt = LastActivity = DateTime.Now;
    }

    public event Action? OnChange;

    public TimeSpan Timeout { get; }
    public DateTime StartedAt { get; private set; }
    public DateTime LastActivity { get; private set; }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;

    public bool HasProfile => !string.IsNullOrWhiteSpace(Email);
    public bool WasExpired { get; private set; }
    public int SavedCount => _savedEventIds.Count;
    public int RegisteredCount => _registeredEventIds.Count;

    public bool IsSaved(int eventId) => _savedEventIds.Contains(eventId);
    public bool IsRegistered(int eventId) => _registeredEventIds.Contains(eventId);

    /// <summary>Call on user activity so an active visitor never expires.</summary>
    public void Touch() => LastActivity = DateTime.Now;

    public void SetProfile(string fullName, string email, string phone)
    {
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = phone.Trim();
        WasExpired = false;
        Touch();
        Notify();
    }

    public void ToggleSaved(int eventId, bool saved)
    {
        if (saved) _savedEventIds.Add(eventId); else _savedEventIds.Remove(eventId);
        Touch();
        Notify();
    }

    public void AddRegistration(int eventId)
    {
        _registeredEventIds.Add(eventId);
        Touch();
        Notify();
    }

    public void RemoveRegistration(int eventId)
    {
        _registeredEventIds.Remove(eventId);
        Touch();
        Notify();
    }

    /// <summary>User chose to clear their session details manually.</summary>
    public void ClearProfile()
    {
        FullName = Email = PhoneNumber = string.Empty;
        _registeredEventIds.Clear();
        WasExpired = false;
        Notify();
    }

    /// <summary>Called by a timer in the layout. Clears personal data if the session timed out.</summary>
    public bool ExpireIfNeeded()
    {
        if (!HasProfile || DateTime.Now - LastActivity <= Timeout)
            return false;

        FullName = Email = PhoneNumber = string.Empty;
        _registeredEventIds.Clear();
        WasExpired = true;
        Notify();
        return true;
    }

    public void DismissExpiredNotice()
    {
        WasExpired = false;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}
