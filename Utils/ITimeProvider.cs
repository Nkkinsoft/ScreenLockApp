namespace ScreenLockApp.Utils;

/// <summary>
/// Provides current time for challenges. Abstracted for testability.
/// </summary>
public interface ITimeProvider
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    DateTime Now { get; }
}

/// <summary>
/// Default time provider using system time.
/// </summary>
public class SystemTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
}

/// <summary>
/// Test time provider that allows injection of specific times.
/// </summary>
public class TestTimeProvider : ITimeProvider
{
    private DateTime _testTime;
    
    public DateTime Now => _testTime;
    
    public void SetTime(DateTime time)
    {
        _testTime = time;
    }
    
    public void AdvanceMinutes(int minutes)
    {
        _testTime = _testTime.AddMinutes(minutes);
    }
}
