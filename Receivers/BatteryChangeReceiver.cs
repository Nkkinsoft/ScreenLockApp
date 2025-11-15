using Android.Content;
using Android.OS;
using Android.Util;

namespace ScreenLockApp.Receivers;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(new[] { Intent.ActionBatteryChanged })]
public class BatteryChangeReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null)
            return;
            
        int level = intent.GetIntExtra(BatteryManager.ExtraLevel, -1);
        int scale = intent.GetIntExtra(BatteryManager.ExtraScale, -1);
        
        if (level == -1 || scale == -1)
            return;
            
        float batteryPct = (level / (float)scale) * 100;
        
        Log.Debug("BatteryChangeReceiver", $"Battery changed: {batteryPct}%");
        
        // Notify any active lock screens to update their hints
        var updateIntent = new Intent("com.nkkinsoft.screenlockapp.BATTERY_UPDATED");
        updateIntent.PutExtra("battery_percent", (int)batteryPct);
        context.SendBroadcast(updateIntent);
    }
}
