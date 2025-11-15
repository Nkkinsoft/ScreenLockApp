using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace ScreenLockApp.Services;

[Service(Enabled = true, Exported = false)]
public class LockScreenService : Service
{
    private const int NotificationId = 1;
    private const string ChannelId = "LockScreenChannel";
    
    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }
    
    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        CreateNotificationChannel();
        
        var notification = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle("ScreenLockApp")
            .SetContentText("Lock screen service is running")
            .SetSmallIcon(Android.Resource.Drawable.IcLockLock)
            .SetOngoing(true)
            .Build();
        
        StartForeground(NotificationId, notification);
        
        return StartCommandResult.Sticky;
    }
    
    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                ChannelId,
                "Lock Screen Service",
                NotificationImportance.Low
            )
            {
                Description = "Keeps the lock screen service running"
            };
            
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.CreateNotificationChannel(channel);
        }
    }
}
