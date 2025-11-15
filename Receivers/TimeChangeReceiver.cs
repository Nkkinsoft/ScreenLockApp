using Android.Content;
using Android.Util;

namespace ScreenLockApp.Receivers;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(new[] { Intent.ActionTimeTick, Intent.ActionTimeChanged, Intent.ActionTimezoneChanged })]
public class TimeChangeReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null)
            return;
            
        var action = intent.Action;
        Log.Debug("TimeChangeReceiver", $"Received action: {action}");
        
        // Notify any active lock screens to update their hints
        // This could trigger auto-unlock if time advances into tolerance window
        var updateIntent = new Intent("com.nkkinsoft.screenlockapp.TIME_UPDATED");
        context.SendBroadcast(updateIntent);
    }
}
