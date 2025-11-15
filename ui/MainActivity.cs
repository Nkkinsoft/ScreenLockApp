using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using ScreenLockApp.challenges;
using ScreenLockApp.data;

namespace ScreenLockApp.ui;

[Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
public class MainActivity : AppCompatActivity
{
    private Button? _btnStartLock;
    private Button? _btnSettings;
    private Button? _btnStopLock;
    private TextView? _tvStatus;
    private PreferencesProvider? _prefs;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Apply theme
        var themeManager = new ThemeManager(this);
        themeManager.ApplyTheme();

        SetContentView(Resource.Layout.activity_main);

        _prefs = new PreferencesProvider(this);

        _btnStartLock = FindViewById<Button>(Resource.Id.btn_start_lock);
        _btnSettings = FindViewById<Button>(Resource.Id.btn_settings);
        _btnStopLock = FindViewById<Button>(Resource.Id.btn_stop_lock);
        _tvStatus = FindViewById<TextView>(Resource.Id.tv_status);

        if (_btnStartLock != null)
            _btnStartLock.Click += OnStartLockClicked;

        if (_btnSettings != null)
            _btnSettings.Click += OnSettingsClicked;

        if (_btnStopLock != null)
            _btnStopLock.Click += OnStopLockClicked;

        UpdateStatus();
    }

    private void OnStartLockClicked(object? sender, EventArgs e)
    {
        // Start lock activity
        var intent = new Android.Content.Intent(this, typeof(LockActivity));
        StartActivity(intent);
        UpdateStatus();
    }

    private void OnSettingsClicked(object? sender, EventArgs e)
    {
        // Open settings
        var intent = new Android.Content.Intent(this, typeof(SettingsActivity));
        StartActivity(intent);
    }

    private void OnStopLockClicked(object? sender, EventArgs e)
    {
        // Stop lock service (if running)
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (_tvStatus != null && _prefs != null)
        {
            var registry = new ChallengeRegistry(this);
            var activeChallenge = registry.GetActiveChallenge();
            _tvStatus.Text = $"Active Challenge: {activeChallenge.DisplayName}\n" +
                           $"Auto-start: {(_prefs.GetEnableAutoStart() ? "Enabled" : "Disabled")}\n" +
                           $"Theme: {_prefs.GetThemeMode()}";
        }
    }

    protected override void OnResume()
    {
        base.OnResume();
        UpdateStatus();
    }
}
