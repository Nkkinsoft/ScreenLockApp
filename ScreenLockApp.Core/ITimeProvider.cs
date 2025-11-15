namespace ScreenLockApp.Core;

/// <summary>
/// Abstraction for getting the current time, allowing for testability.
/// </summary>
public interface ITimeProvider
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    DateTime Now { get; }
}

/// <summary>
/// System time provider that returns the actual current time.
/// </summary>
public class SystemTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}
