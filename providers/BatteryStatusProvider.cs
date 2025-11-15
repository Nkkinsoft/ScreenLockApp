using Android.Content;

namespace ScreenLockApp.providers;

/// <summary>
/// Provides battery status information
/// </summary>
public class BatteryStatusProvider
{
    private readonly Context _context;
    private int _cachedBatteryPercentage = -1;

    public BatteryStatusProvider(Context context)
    {
        _context = context;
        UpdateBatteryStatus();
    }

    /// <summary>
    /// Gets the current battery percentage (0-100)
    /// </summary>
    public int GetBatteryPercentage()
    {
        UpdateBatteryStatus();
        return _cachedBatteryPercentage;
    }

    private void UpdateBatteryStatus()
    {
        try
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = _context.RegisterReceiver(null, filter);

            if (battery != null)
            {
                int level = battery.GetIntExtra(Android.OS.BatteryManager.ExtraLevel, -1);
                int scale = battery.GetIntExtra(Android.OS.BatteryManager.ExtraScale, -1);

                if (level >= 0 && scale > 0)
                {
                    _cachedBatteryPercentage = (int)((level / (float)scale) * 100);
                }
            }
        }
        catch
        {
            // Fallback to last known value or 0
            if (_cachedBatteryPercentage < 0)
                _cachedBatteryPercentage = 0;
        }
    }
}
