using ScreenLockApp.Utils;

namespace ScreenLockApp.Challenges;

/// <summary>
/// Challenge based on current time in HHMM format with configurable tolerance.
/// </summary>
public class TimeChallenge : IUnlockChallenge
{
    private readonly ITimeProvider _timeProvider;
    private readonly Func<int> _getToleranceMinutes;
    
    public string Id => "time";
    public string DisplayName => "Current Time";
    
    public TimeChallenge(ITimeProvider timeProvider, Func<int> getToleranceMinutes)
    {
        _timeProvider = timeProvider;
        _getToleranceMinutes = getToleranceMinutes;
    }
    
    public string GetHint()
    {
        var now = _timeProvider.Now;
        var tolerance = _getToleranceMinutes();
        if (tolerance == 0)
        {
            return $"Enter current time: {now:HHmm}";
        }
        return $"Enter current time (±{tolerance} min)";
    }
    
    public bool Validate(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput) || userInput.Length != 4)
            return false;
            
        if (!int.TryParse(userInput, out int inputTime))
            return false;
            
        return TimeValidator.IsWithinTolerance(
            _timeProvider.Now,
            inputTime,
            _getToleranceMinutes()
        );
    }
    
    public void Update()
    {
        // Time-based, no state to update
    }
}
