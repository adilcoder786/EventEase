namespace EventEase.Services;

public enum ToastType { Success, Error, Info, Warning }

public record Toast(Guid Id, string Message, ToastType Type);

/// <summary>
/// Small scoped service that lets any page show a success/error toast.
/// The ToastContainer component listens to <see cref="OnChange"/> and re-renders.
/// </summary>
public class NotificationService
{
    private readonly List<Toast> _toasts = new();
    private readonly object _lock = new();

    public event Action? OnChange;

    public IReadOnlyList<Toast> Toasts
    {
        get { lock (_lock) return _toasts.ToArray(); }
    }

    public void Show(string message, ToastType type = ToastType.Info, int durationMs = 4500)
    {
        var toast = new Toast(Guid.NewGuid(), message, type);
        lock (_lock) _toasts.Add(toast);
        OnChange?.Invoke();
        _ = RemoveLaterAsync(toast.Id, durationMs);
    }

    public void Success(string message) => Show(message, ToastType.Success);
    public void Error(string message) => Show(message, ToastType.Error, 6000);

    public void Dismiss(Guid id)
    {
        lock (_lock) _toasts.RemoveAll(t => t.Id == id);
        OnChange?.Invoke();
    }

    private async Task RemoveLaterAsync(Guid id, int delayMs)
    {
        await Task.Delay(delayMs);
        Dismiss(id);
    }
}
