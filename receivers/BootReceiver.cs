using Android.Content;
using ScreenLockApp.data;

namespace ScreenLockApp.receivers;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted })]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null) return;

        if (intent.Action == Intent.ActionBootCompleted)
        {
            var prefs = new PreferencesProvider(context);
            
            if (prefs.GetEnableAutoStart())
            {
                // Start lock service on boot
                var serviceIntent = new Intent(context, typeof(services.LockService));
                context.StartService(serviceIntent);
            }
        }
    }
}
