using Android.Content;
using Android.OS;

namespace ScreenLockApp.Utils;

/// <summary>
/// Utility class for getting battery information.
/// </summary>
public static class BatteryHelper
{
    /// <summary>
    /// Gets the current battery percentage.
    /// </summary>
    /// <param name="context">Android context</param>
    /// <returns>Battery percentage (0-100), or -1 if unavailable</returns>
    public static int GetBatteryPercent(Context context)
    {
        try
        {
            var batteryStatus = GetBatteryStatus(context);
            if (batteryStatus == null)
            {
                return -1;
            }

            int level = batteryStatus.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = batteryStatus.GetIntExtra(BatteryManager.ExtraScale, -1);

            if (level == -1 || scale == -1)
            {
                return -1;
            }

            return (int)((level / (float)scale) * 100.0f);
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Gets the battery status intent.
    /// </summary>
    private static Intent? GetBatteryStatus(Context context)
    {
        var filter = new IntentFilter(Intent.ActionBatteryChanged);
        return context.RegisterReceiver(null, filter);
    }
}
