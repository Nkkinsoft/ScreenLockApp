using Android.App;
using Android.Runtime;
using ScreenLockApp.data;

namespace ScreenLockApp;

[Application]
public class ScreenLockApplication : Application
{
    public ScreenLockApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // Apply theme on app start
        var themeManager = new ThemeManager(this);
        themeManager.ApplyTheme();
    }
}
