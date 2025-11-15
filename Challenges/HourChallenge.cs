using ScreenLockApp.Utils;

namespace ScreenLockApp.Challenges;

/// <summary>
/// Challenge based on current hour (00-23) with midnight transition support.
/// </summary>
public class HourChallenge : IUnlockChallenge
{
    private readonly ITimeProvider _timeProvider;
    
    public string Id => "hour";
    public string DisplayName => "Current Hour";
    
    public HourChallenge(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public string GetHint()
    {
        var now = _timeProvider.Now;
        return $"Enter current hour (00-23): {now.Hour:D2}";
    }
    
    public bool Validate(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return false;
            
        if (!int.TryParse(userInput, out int inputHour))
            return false;
            
        if (inputHour < 0 || inputHour > 23)
            return false;
            
        var currentHour = _timeProvider.Now.Hour;
        return currentHour == inputHour;
    }
    
    public void Update()
    {
        // Hour-based, no state to update
    }
}
