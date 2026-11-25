namespace EventEase.Models;

/// <summary>Simple success/failure result so services never need to throw for normal problems.</summary>
public record ServiceResult(bool Success, string Message)
{
    public static ServiceResult Ok(string message = "") => new(true, message);
    public static ServiceResult Fail(string message) => new(false, message);
}

public record ServiceResult<T>(bool Success, string Message, T? Data = default)
{
    public static ServiceResult<T> Ok(T data, string message = "") => new(true, message, data);
    public static ServiceResult<T> Fail(string message) => new(false, message);
}
