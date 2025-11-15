using Android.App.Admin;
using Android.Content;
using Java.Lang;

namespace ScreenLockApp.Admin;

/// <summary>
/// Device Admin Receiver for managing device administration privileges.
/// </summary>
[BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", Exported = true)]
[IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", "android.app.action.DEVICE_ADMIN_DISABLED" })]
[MetaData("android.app.device_admin", Resource = "@xml/device_admin")]
public class TimeDeviceAdminReceiver : DeviceAdminReceiver
{
    /// <summary>
    /// Called when the device admin is enabled.
    /// </summary>
    public override void OnEnabled(Context context, Intent intent)
    {
        base.OnEnabled(context, intent);
        Android.Util.Log.Info("ScreenLockApp", "Device Admin enabled");
    }

    /// <summary>
    /// Called when the device admin is disabled.
    /// </summary>
    public override void OnDisabled(Context context, Intent intent)
    {
        base.OnDisabled(context, intent);
        Android.Util.Log.Info("ScreenLockApp", "Device Admin disabled");
    }

    /// <summary>
    /// Called when the user tries to disable the device admin.
    /// </summary>
    public override ICharSequence? OnDisableRequestedFormatted(Context context, Intent intent)
    {
        return new Java.Lang.String("Disabling device admin will prevent the lock screen from functioning properly.");
    }
}
