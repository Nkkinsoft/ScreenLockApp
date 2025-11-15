using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using ScreenLockApp.Themes;

namespace ScreenLockApp.Activities;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    private Button? _btnLockScreen;
    private Button? _btnSettings;
    private TextView? _statusText;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Apply theme before setContentView
        ThemeManager.ApplyTheme(this);
        
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
        
        _btnLockScreen = FindViewById<Button>(Resource.Id.btnLockScreen);
        _btnSettings = FindViewById<Button>(Resource.Id.btnSettings);
        _statusText = FindViewById<TextView>(Resource.Id.statusText);
        
        if (_btnLockScreen != null)
        {
            _btnLockScreen.Click += BtnLockScreen_Click;
        }
        
        if (_btnSettings != null)
        {
            _btnSettings.Click += BtnSettings_Click;
        }
        
        UpdateStatus();
    }
    
    private void BtnLockScreen_Click(object? sender, EventArgs e)
    {
        var intent = new Intent(this, typeof(LockActivity));
        StartActivity(intent);
    }
    
    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        var intent = new Intent(this, typeof(SettingsActivity));
        StartActivity(intent);
    }
    
    protected override void OnResume()
    {
        base.OnResume();
        UpdateStatus();
    }
    
    private void UpdateStatus()
    {
        if (_statusText != null)
        {
            _statusText.Text = GetString(Resource.String.status_unlocked);
            
            // Apply theme color
            var color = ThemeManager.GetThemeColor(this, Android.Resource.Attribute.ColorPrimary);
            _statusText.SetTextColor(new Android.Graphics.Color(color));
        }
    }
}
