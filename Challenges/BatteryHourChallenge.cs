using Android.Content;
using ScreenLockApp.Utils;

namespace ScreenLockApp.Challenges;

/// <summary>
/// Composite challenge requiring both battery percentage and hour to match.
/// Handles battery=100% normalization and midnight transitions.
/// </summary>
public class BatteryHourChallenge : IUnlockChallenge
{
    private readonly BatteryChallenge _batteryChallenge;
    private readonly HourChallenge _hourChallenge;
    
    public string Id => "battery_hour";
    public string DisplayName => "Battery + Hour";
    
    public BatteryHourChallenge(Context context, ITimeProvider timeProvider, Func<int> getBatteryTolerance)
    {
        _batteryChallenge = new BatteryChallenge(context, getBatteryTolerance);
        _hourChallenge = new HourChallenge(timeProvider);
    }
    
    public string GetHint()
    {
        var batteryHint = _batteryChallenge.GetHint();
        var hourHint = _hourChallenge.GetHint();
        return $"{batteryHint} + {hourHint}";
    }
    
    public bool Validate(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput) || userInput.Length < 3)
            return false;
            
        // Expected format: BBHH (battery percentage + hour)
        // Handle both 3-digit (e.g., "523" for 5% battery, 23 hour) 
        // and 4-digit (e.g., "2314" for 23% battery, 14 hour)
        
        string batteryPart;
        string hourPart;
        
        if (userInput.Length == 3)
        {
            batteryPart = userInput.Substring(0, 1);
            hourPart = userInput.Substring(1, 2);
        }
        else if (userInput.Length == 4)
        {
            batteryPart = userInput.Substring(0, 2);
            hourPart = userInput.Substring(2, 2);
        }
        else
        {
            return false;
        }
        
        return _batteryChallenge.Validate(batteryPart) && _hourChallenge.Validate(hourPart);
    }
    
    public void Update()
    {
        _batteryChallenge.Update();
        _hourChallenge.Update();
    }
}
