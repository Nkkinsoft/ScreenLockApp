using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using ScreenLockApp.Themes;

namespace ScreenLockApp.Activities;

[Activity(Label = "@string/settings_title")]
public class SettingsActivity : AppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Apply theme before setContentView
        ThemeManager.ApplyTheme(this);
        
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_settings);
        
        // Load preferences fragment
        if (savedInstanceState == null)
        {
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.settings_container, new SettingsFragment())
                .Commit();
        }
        
        // Enable back button
        SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
    }
    
    public override bool OnSupportNavigateUp()
    {
        Finish();
        return true;
    }
}

public class SettingsFragment : PreferenceFragmentCompat
{
    public override void OnCreatePreferences(Bundle? savedInstanceState, string? rootKey)
    {
        SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
        
        // Set up strict mode listener
        var strictMode = FindPreference("strict_mode");
        if (strictMode != null)
        {
            strictMode.PreferenceChange += StrictMode_PreferenceChange;
        }
    }
    
    private void StrictMode_PreferenceChange(object? sender, Preference.PreferenceChangeEventArgs e)
    {
        if (e.NewValue is bool strictEnabled && strictEnabled)
        {
            // Override tolerances to 0
            var prefs = PreferenceManager.GetDefaultSharedPreferences(RequireContext());
            prefs?.Edit()
                ?.PutInt("tolerance_minutes", 0)
                ?.PutInt("battery_tolerance", 0)
                ?.Apply();
        }
    }
}
