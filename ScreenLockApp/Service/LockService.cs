using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using ScreenLockApp.Data;
using ScreenLockApp.UI;

namespace ScreenLockApp.Service;

/// <summary>
/// Foreground service that monitors and triggers the lock screen.
/// </summary>
[Service(Enabled = true, Exported = false, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeSpecialUse)]
public class LockService : Android.App.Service
{
    private const int NotificationId = 1001;
    private const string ChannelId = "lock_service_channel";
    private Handler? _handler;
    private readonly int _checkInterval = 60000; // Check every minute

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        // Start as foreground service with notification
        var notification = CreateNotification();
        StartForeground(NotificationId, notification);

        // Initialize handler for periodic checks
        _handler = new Handler(Looper.MainLooper!);
        StartPeriodicCheck();

        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent)
    {
        return null; // Not a bound service
    }

    public override void OnDestroy()
    {
        _handler?.RemoveCallbacksAndMessages(null);
        base.OnDestroy();
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                ChannelId,
                GetString(Resource.String.service_channel_name),
                NotificationImportance.Low)
            {
                Description = GetString(Resource.String.service_channel_description)
            };

            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        var notificationIntent = new Intent(this, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(
            this, 
            0, 
            notificationIntent,
            PendingIntentFlags.Immutable);

        var builder = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle(GetString(Resource.String.service_notification_title))
            .SetContentText(GetString(Resource.String.service_notification_text))
            .SetSmallIcon(Resource.Drawable.ic_lock)
            .SetContentIntent(pendingIntent)
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow);

        return builder.Build();
    }

    private void StartPeriodicCheck()
    {
        _handler?.PostDelayed(() =>
        {
            CheckAndTriggerLock();
            StartPeriodicCheck(); // Schedule next check
        }, _checkInterval);
    }

    private void CheckAndTriggerLock()
    {
        var prefs = new PreferencesProvider(this);
        
        // Only trigger if lock is enabled
        if (prefs.IsLockEnabled)
        {
            Android.Util.Log.Debug("ScreenLockApp", "Lock check - lock is enabled");
            // You can add custom logic here to trigger lock at specific times/intervals
            // For now, this service just keeps running to demonstrate the architecture
        }
    }

    /// <summary>
    /// Triggers the lock screen immediately.
    /// </summary>
    public void TriggerLockNow()
    {
        var intent = new Intent(this, typeof(LockActivity));
        intent.AddFlags(ActivityFlags.NewTask);
        intent.AddFlags(ActivityFlags.NoHistory);
        StartActivity(intent);
    }
}
