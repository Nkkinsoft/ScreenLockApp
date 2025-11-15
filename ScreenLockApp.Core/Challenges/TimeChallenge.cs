namespace ScreenLockApp.Core.Challenges;

/// <summary>
/// Time-based unlock challenge that requires entering current time in HHMM format.
/// Uses ±1 minute tolerance for user convenience.
/// </summary>
public class TimeChallenge : IUnlockChallenge
{
    private readonly TimeValidator _timeValidator;

    public TimeChallenge()
    {
        _timeValidator = new TimeValidator();
    }

    public string ChallengeId => "time";

    public string DisplayName => "Time (HHMM)";

    public string Description => "Enter current time in 24-hour format (HHMM)";

    public int RequiredLength => 4;

    public ValidationResult Validate(string input)
    {
        return _timeValidator.ValidateTime(input);
    }

    public string GetHint()
    {
        var now = DateTime.Now;
        return $"Enter current 24h time HHMM (e.g., {now:HHmm})";
    }
}
