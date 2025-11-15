using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using ScreenLockApp.Core;

namespace ScreenLockApp;

/// <summary>
/// Foreground service that monitors lock state and grace period.
/// </summary>
[Service(Enabled = true, Exported = false)]
public class LockService : Service
{
    private const int NotificationId = 1000;
    private const string ChannelId = "lock_service_channel";

    private LockCoordinator? _coordinator;
    private IPreferencesProvider? _preferencesProvider;
    private BroadcastReceiver? _timeTickReceiver;

    public override IBinder? OnBind(Intent? intent)
    {
        return null; // We don't provide binding
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // Get services from DI
        _coordinator = App.TryGetService<LockCoordinator>();
        _preferencesProvider = App.TryGetService<IPreferencesProvider>();

        // Create notification channel
        CreateNotificationChannel();

        // Start as foreground service
        var notification = CreateNotification();
        StartForeground(NotificationId, notification);

        // Register time tick receiver for dynamic updates
        RegisterTimeTickReceiver();

        // Monitor lock state changes
        if (_coordinator != null)
        {
            _coordinator.LockStateChanged += OnLockStateChanged;
        }
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            if (notificationManager == null) return;

            var channel = new NotificationChannel(
                ChannelId,
                GetString(Resource.String.lock_service_channel_name),
                NotificationImportance.Low)
            {
                Description = GetString(Resource.String.lock_service_channel_desc)
            };

            notificationManager.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        var builder = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle(GetString(Resource.String.lock_service_notification_title))
            .SetContentText(GetString(Resource.String.lock_service_notification_text))
            .SetSmallIcon(Android.Resource.Drawable.IcLockLock)
            .SetOngoing(true)
            .SetPriority(NotificationCompat.PriorityLow);

        // Add tap action to return to main activity
        var intent = new Intent(this, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(
            this, 0, intent,
            Build.VERSION.SdkInt >= BuildVersionCodes.M
                ? PendingIntentFlags.Immutable
                : PendingIntentFlags.UpdateCurrent);

        builder.SetContentIntent(pendingIntent);

        return builder.Build();
    }

    private void RegisterTimeTickReceiver()
    {
        _timeTickReceiver = new TimeTickReceiver();
        var filter = new IntentFilter(Intent.ActionTimeTick);
        RegisterReceiver(_timeTickReceiver, filter);
    }

    private void OnLockStateChanged(object? sender, bool isLocked)
    {
        if (!isLocked)
        {
            // Check grace period before allowing re-lock
            CheckGracePeriod();
        }
    }

    private void CheckGracePeriod()
    {
        if (_preferencesProvider == null || _coordinator == null) return;

        if (!_preferencesProvider.GetGracePeriodEnabled()) return;

        long lastUnlockEpochMs = _preferencesProvider.GetLastUnlockEpochMs();
        if (lastUnlockEpochMs == 0) return;

        int gracePeriodMinutes = _preferencesProvider.GetGracePeriodMinutes();
        if (gracePeriodMinutes == 0) return;

        long nowEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long elapsedMs = nowEpochMs - lastUnlockEpochMs;
        long gracePeriodMs = gracePeriodMinutes * 60 * 1000;

        if (elapsedMs < gracePeriodMs)
        {
            // Still within grace period - don't trigger lock
            Android.Util.Log.Info("LockService", 
                $"Within grace period: {elapsedMs / 1000}s elapsed, {gracePeriodMs / 1000}s grace");
        }
    }

    public override void OnDestroy()
    {
        // Unregister receivers
        if (_timeTickReceiver != null)
        {
            UnregisterReceiver(_timeTickReceiver);
            _timeTickReceiver = null;
        }

        // Unsubscribe from coordinator
        if (_coordinator != null)
        {
            _coordinator.LockStateChanged -= OnLockStateChanged;
        }

        base.OnDestroy();
    }

    [Obsolete("Deprecated in API 26")]
    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        return StartCommandResult.Sticky;
    }

    // Inner class for time tick receiver
    private class TimeTickReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Action == Intent.ActionTimeTick)
            {
                Android.Util.Log.Debug("LockService", "Time tick received");
                // Could trigger validation here if needed
            }
        }
    }
}
