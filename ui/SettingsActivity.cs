using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ScreenLockApp.challenges;
using ScreenLockApp.data;

namespace ScreenLockApp.ui;

[Activity(Label = "@string/settings_title", Theme = "@style/AppTheme", ParentActivity = typeof(MainActivity))]
public class SettingsActivity : AppCompatActivity
{
    private PreferencesProvider? _prefs;
    private ChallengeRegistry? _registry;
    private ThemeManager? _themeManager;

    // Challenge selection
    private RadioGroup? _rgChallenge;
    private RadioButton? _rbTime;
    private RadioButton? _rbBattery;
    private RadioButton? _rbHour;
    private RadioButton? _rbDailyCode;
    private TextView? _tvChallengeDesc;

    // Tolerance & Parameters
    private Spinner? _spTimeTolerance;
    private Spinner? _spBatteryTolerance;
    private Spinner? _spGracePeriod;

    // Behavior
    private Switch? _swAutoStart;
    private Switch? _swHaptics;
    private Switch? _swKioskMode;

    // Appearance
    private RadioGroup? _rgTheme;
    private Switch? _swHighContrast;

    // Security
    private Switch? _swStrictMode;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_settings);

        // Enable up navigation
        SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

        _prefs = new PreferencesProvider(this);
        _registry = new ChallengeRegistry(this);
        _themeManager = new ThemeManager(this);

        InitializeViews();
        LoadSettings();
    }

    private void InitializeViews()
    {
        // Challenge selection
        _rgChallenge = FindViewById<RadioGroup>(Resource.Id.rg_challenge);
        _rbTime = FindViewById<RadioButton>(Resource.Id.rb_time);
        _rbBattery = FindViewById<RadioButton>(Resource.Id.rb_battery);
        _rbHour = FindViewById<RadioButton>(Resource.Id.rb_hour);
        _rbDailyCode = FindViewById<RadioButton>(Resource.Id.rb_dailycode);
        _tvChallengeDesc = FindViewById<TextView>(Resource.Id.tv_challenge_desc);

        // Tolerance & Parameters
        _spTimeTolerance = FindViewById<Spinner>(Resource.Id.sp_time_tolerance);
        _spBatteryTolerance = FindViewById<Spinner>(Resource.Id.sp_battery_tolerance);
        _spGracePeriod = FindViewById<Spinner>(Resource.Id.sp_grace_period);

        // Behavior
        _swAutoStart = FindViewById<Switch>(Resource.Id.sw_auto_start);
        _swHaptics = FindViewById<Switch>(Resource.Id.sw_haptics);
        _swKioskMode = FindViewById<Switch>(Resource.Id.sw_kiosk_mode);

        // Appearance
        _rgTheme = FindViewById<RadioGroup>(Resource.Id.rg_theme);
        _swHighContrast = FindViewById<Switch>(Resource.Id.sw_high_contrast);

        // Security
        _swStrictMode = FindViewById<Switch>(Resource.Id.sw_strict_mode);

        SetupEventHandlers();
        SetupSpinners();
    }

    private void SetupEventHandlers()
    {
        if (_rgChallenge != null)
        {
            _rgChallenge.CheckedChange += OnChallengeChanged;
        }

        if (_spTimeTolerance != null)
        {
            _spTimeTolerance.ItemSelected += (s, e) => 
            {
                int tolerance = GetToleranceValue(e.Position);
                _prefs?.SetTimeTolerance(tolerance);
            };
        }

        if (_spBatteryTolerance != null)
        {
            _spBatteryTolerance.ItemSelected += (s, e) => 
            {
                _prefs?.SetBatteryTolerance(e.Position);
            };
        }

        if (_spGracePeriod != null)
        {
            _spGracePeriod.ItemSelected += (s, e) => 
            {
                int minutes = GetGracePeriodValue(e.Position);
                _prefs?.SetGracePeriod(minutes);
            };
        }

        if (_swAutoStart != null)
        {
            _swAutoStart.CheckedChange += (s, e) => _prefs?.SetEnableAutoStart(e.IsChecked);
        }

        if (_swHaptics != null)
        {
            _swHaptics.CheckedChange += (s, e) => _prefs?.SetEnableHaptics(e.IsChecked);
        }

        if (_swKioskMode != null)
        {
            _swKioskMode.CheckedChange += (s, e) => _prefs?.SetEnableKioskMode(e.IsChecked);
        }

        if (_rgTheme != null)
        {
            _rgTheme.CheckedChange += OnThemeChanged;
        }

        if (_swHighContrast != null)
        {
            _swHighContrast.CheckedChange += (s, e) => _prefs?.SetHighContrast(e.IsChecked);
        }

        if (_swStrictMode != null)
        {
            _swStrictMode.CheckedChange += (s, e) => _prefs?.SetStrictMode(e.IsChecked);
        }
    }

    private void SetupSpinners()
    {
        // Time Tolerance options: 0, 1, 2, 5, 10 minutes
        if (_spTimeTolerance != null)
        {
            var adapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.time_tolerance_options, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spTimeTolerance.Adapter = adapter;
        }

        // Battery Tolerance options: 0, 1%
        if (_spBatteryTolerance != null)
        {
            var adapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.battery_tolerance_options, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spBatteryTolerance.Adapter = adapter;
        }

        // Grace Period options: Off, 1, 2, 5 minutes
        if (_spGracePeriod != null)
        {
            var adapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.grace_period_options, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spGracePeriod.Adapter = adapter;
        }
    }

    private void LoadSettings()
    {
        if (_prefs == null) return;

        // Load active challenge
        string activeChallengeId = _prefs.GetActiveChallengeId();
        if (_rgChallenge != null)
        {
            int checkedId = activeChallengeId switch
            {
                "time" => Resource.Id.rb_time,
                "battery" => Resource.Id.rb_battery,
                "hour" => Resource.Id.rb_hour,
                "dailycode" => Resource.Id.rb_dailycode,
                _ => Resource.Id.rb_time
            };
            _rgChallenge.Check(checkedId);
            UpdateChallengeDescription(activeChallengeId);
        }

        // Load tolerance values
        int timeTolerance = _prefs.GetTimeTolerance();
        _spTimeTolerance?.SetSelection(GetTolerancePosition(timeTolerance));

        int batteryTolerance = _prefs.GetBatteryTolerance();
        _spBatteryTolerance?.SetSelection(batteryTolerance);

        int gracePeriod = _prefs.GetGracePeriod();
        _spGracePeriod?.SetSelection(GetGracePeriodPosition(gracePeriod));

        // Load behavior settings
        if (_swAutoStart != null)
            _swAutoStart.Checked = _prefs.GetEnableAutoStart();
        if (_swHaptics != null)
            _swHaptics.Checked = _prefs.GetEnableHaptics();
        if (_swKioskMode != null)
            _swKioskMode.Checked = _prefs.GetEnableKioskMode();

        // Load theme
        string themeMode = _prefs.GetThemeMode();
        if (_rgTheme != null)
        {
            int themeId = themeMode switch
            {
                "dark" => Resource.Id.rb_theme_dark,
                "light" => Resource.Id.rb_theme_light,
                _ => Resource.Id.rb_theme_system
            };
            _rgTheme.Check(themeId);
        }

        if (_swHighContrast != null)
            _swHighContrast.Checked = _prefs.GetHighContrast();
        if (_swStrictMode != null)
            _swStrictMode.Checked = _prefs.GetStrictMode();
    }

    private void OnChallengeChanged(object? sender, RadioGroup.CheckedChangeEventArgs e)
    {
        string challengeId = e.CheckedId switch
        {
            Resource.Id.rb_time => "time",
            Resource.Id.rb_battery => "battery",
            Resource.Id.rb_hour => "hour",
            Resource.Id.rb_dailycode => "dailycode",
            _ => "time"
        };

        _prefs?.SetActiveChallengeId(challengeId);
        UpdateChallengeDescription(challengeId);
    }

    private void UpdateChallengeDescription(string challengeId)
    {
        if (_tvChallengeDesc == null || _registry == null) return;

        var challenge = _registry.GetChallengeById(challengeId);
        _tvChallengeDesc.Text = challenge.Description;
    }

    private string GetChallengeDisplayName(string challengeId)
    {
        return challengeId switch
        {
            "time" => "Time Challenge",
            "battery" => "Battery Challenge",
            "hour" => "Hour Challenge",
            "dailycode" => "Daily Code Challenge",
            _ => "Unknown"
        };
    }

    private void OnThemeChanged(object? sender, RadioGroup.CheckedChangeEventArgs e)
    {
        string themeMode = e.CheckedId switch
        {
            Resource.Id.rb_theme_dark => "dark",
            Resource.Id.rb_theme_light => "light",
            _ => "system"
        };

        _themeManager?.SetThemeMode(themeMode);
    }

    private int GetToleranceValue(int position)
    {
        return position switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 5,
            4 => 10,
            _ => 0
        };
    }

    private int GetTolerancePosition(int tolerance)
    {
        return tolerance switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            5 => 3,
            10 => 4,
            _ => 0
        };
    }

    private int GetGracePeriodValue(int position)
    {
        return position switch
        {
            0 => 0,  // Off
            1 => 1,
            2 => 2,
            3 => 5,
            _ => 0
        };
    }

    private int GetGracePeriodPosition(int minutes)
    {
        return minutes switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            5 => 3,
            _ => 0
        };
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
        if (item.ItemId == Android.Resource.Id.Home)
        {
            Finish();
            return true;
        }
        return base.OnOptionsItemSelected(item);
    }
}
