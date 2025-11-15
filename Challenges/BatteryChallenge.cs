using Android.Content;
using Android.OS;
using ScreenLockApp.Utils;

namespace ScreenLockApp.Challenges;

/// <summary>
/// Challenge based on current battery percentage with configurable tolerance.
/// </summary>
public class BatteryChallenge : IUnlockChallenge
{
    private readonly Context _context;
    private readonly Func<int> _getBatteryTolerance;
    
    public string Id => "battery";
    public string DisplayName => "Battery Level";
    
    public BatteryChallenge(Context context, Func<int> getBatteryTolerance)
    {
        _context = context;
        _getBatteryTolerance = getBatteryTolerance;
    }
    
    public string GetHint()
    {
        var batteryPercent = GetCurrentBatteryPercent();
        var tolerance = _getBatteryTolerance();
        if (tolerance == 0)
        {
            return $"Enter battery level: {batteryPercent:D2}";
        }
        return $"Enter battery level (±{tolerance}%)";
    }
    
    public bool Validate(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return false;
            
        if (!int.TryParse(userInput, out int inputPercent))
            return false;
            
        var currentPercent = GetCurrentBatteryPercent();
        var tolerance = _getBatteryTolerance();
        
        return Math.Abs(currentPercent - inputPercent) <= tolerance;
    }
    
    public void Update()
    {
        // Battery-based, no state to update
    }
    
    private int GetCurrentBatteryPercent()
    {
        var filter = new IntentFilter(Intent.ActionBatteryChanged);
        var battery = _context.RegisterReceiver(null, filter);
        
        if (battery == null)
            return 0;
            
        int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
        int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);
        
        if (level == -1 || scale == -1)
            return 0;
            
        float percent = (level / (float)scale) * 100;
        int batteryPercent = (int)Math.Floor(percent);
        
        // Normalize 100% to 99% as per requirements
        if (batteryPercent == 100)
            batteryPercent = 99;
            
        return batteryPercent;
    }
}
