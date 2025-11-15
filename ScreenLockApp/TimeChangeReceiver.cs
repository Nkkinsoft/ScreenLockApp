using Android.App;
using Android.Content;
using ScreenLockApp.Core;

namespace ScreenLockApp;

/// <summary>
/// Broadcast receiver that listens for time/timezone changes.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionTimeChanged, Intent.ActionTimezoneChanged })]
public class TimeChangeReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null) return;

        var action = intent.Action;
        if (action == Intent.ActionTimeChanged || action == Intent.ActionTimezoneChanged)
        {
            // Get the lock coordinator
            var coordinator = App.TryGetService<LockCoordinator>();
            
            if (coordinator?.IsLocked == true)
            {
                // Time changed while locked - could auto-unlock if time is now valid
                // This would require re-running validation with current input
                // For now, we just notify via logging
                Android.Util.Log.Info("TimeChangeReceiver", $"Time changed while locked: {action}");
                
                // Optionally show a notification
                ShowTimeChangeNotification(context);
            }
        }
    }

    private void ShowTimeChangeNotification(Context context)
    {
        try
        {
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (notificationManager == null) return;

            // Create notification channel for Android O+
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "time_change_channel",
                    "Time Change Notifications",
                    NotificationImportance.Low)
                {
                    Description = "Notifications when system time changes while locked"
                };
                notificationManager.CreateNotificationChannel(channel);
            }

            // Build notification
            var builder = new Notification.Builder(context);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                builder.SetChannelId("time_change_channel");
            }

            builder.SetContentTitle(context.GetString(Resource.String.app_name))
                   .SetContentText(context.GetString(Resource.String.time_changed_notification))
                   .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                   .SetAutoCancel(true);

            notificationManager.Notify(1001, builder.Build());
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("TimeChangeReceiver", $"Error showing notification: {ex.Message}");
        }
    }
}
