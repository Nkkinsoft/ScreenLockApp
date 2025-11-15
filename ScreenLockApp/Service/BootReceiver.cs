using Android.Content;
using ScreenLockApp.Data;

namespace ScreenLockApp.Service;

/// <summary>
/// Broadcast receiver that starts the LockService on device boot.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted, "android.intent.action.QUICKBOOT_POWERON" })]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null)
            return;

        if (intent.Action == Intent.ActionBootCompleted || 
            intent.Action == "android.intent.action.QUICKBOOT_POWERON")
        {
            var prefs = new PreferencesProvider(context);
            
            // Only start service if auto-start is enabled and lock is enabled
            if (prefs.AutoStartOnBoot && prefs.IsLockEnabled)
            {
                Android.Util.Log.Info("ScreenLockApp", "Boot completed - starting LockService");
                
                var serviceIntent = new Intent(context, typeof(LockService));
                
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    context.StartForegroundService(serviceIntent);
                }
                else
                {
                    context.StartService(serviceIntent);
                }
            }
        }
    }
}
