using ScreenLockApp.Utils;

namespace ScreenLockApp.Challenges;

/// <summary>
/// Challenge based on day of week with rotating codes, handles week boundary transitions.
/// </summary>
public class DailyCodeChallenge : IUnlockChallenge
{
    private readonly ITimeProvider _timeProvider;
    private readonly string[] _dailyCodes = { "1234", "2345", "3456", "4567", "5678", "6789", "7890" };
    
    public string Id => "daily_code";
    public string DisplayName => "Daily Code";
    
    public DailyCodeChallenge(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public string GetHint()
    {
        var dayOfWeek = _timeProvider.Now.DayOfWeek;
        return $"Enter today's code ({dayOfWeek})";
    }
    
    public bool Validate(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return false;
            
        var dayOfWeek = (int)_timeProvider.Now.DayOfWeek;
        var expectedCode = _dailyCodes[dayOfWeek];
        
        return userInput == expectedCode;
    }
    
    public void Update()
    {
        // Day-based, no state to update
    }
}
