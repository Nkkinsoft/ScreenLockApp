using Android.Content;
using AndroidX.AppCompat.App;

namespace ScreenLockApp.data;

/// <summary>
/// Manages app theme (Dark/Light/System)
/// </summary>
public class ThemeManager
{
    private readonly PreferencesProvider _prefs;

    public ThemeManager(Context context)
    {
        _prefs = new PreferencesProvider(context);
    }

    /// <summary>
    /// Applies the currently selected theme
    /// </summary>
    public void ApplyTheme()
    {
        string themeMode = _prefs.GetThemeMode();
        
        int nightMode = themeMode switch
        {
            "dark" => AppCompatDelegate.ModeNightYes,
            "light" => AppCompatDelegate.ModeNightNo,
            _ => AppCompatDelegate.ModeNightFollowSystem
        };

        AppCompatDelegate.DefaultNightMode = nightMode;
    }

    /// <summary>
    /// Sets and applies a new theme mode
    /// </summary>
    public void SetThemeMode(string mode)
    {
        _prefs.SetThemeMode(mode);
        ApplyTheme();
    }
}
