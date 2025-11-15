using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.App;
using AndroidX.Preference;

namespace ScreenLockApp.Themes;

/// <summary>
/// Manages theme selection and high-contrast mode.
/// </summary>
public class ThemeManager
{
    private const string PREF_HIGH_CONTRAST = "high_contrast";
    private const string PREF_THEME_MODE = "theme_mode";
    
    /// <summary>
    /// Applies the current theme to an activity.
    /// </summary>
    /// <param name="context">The activity context.</param>
    public static void ApplyTheme(Context context)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        var highContrast = prefs?.GetBoolean(PREF_HIGH_CONTRAST, false) ?? false;
        var themeMode = prefs?.GetString(PREF_THEME_MODE, "system") ?? "system";
        
        // Apply base theme
        int themeResId = highContrast 
            ? Resource.Style.AppTheme_HighContrast 
            : Resource.Style.AppTheme;
            
        context.SetTheme(themeResId);
        
        // Apply night mode
        int nightMode = themeMode switch
        {
            "light" => AppCompatDelegate.ModeNightNo,
            "dark" => AppCompatDelegate.ModeNightYes,
            _ => AppCompatDelegate.ModeNightFollowSystem
        };
        
        AppCompatDelegate.DefaultNightMode = nightMode;
    }
    
    /// <summary>
    /// Checks if high-contrast mode is enabled.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>True if high-contrast mode is enabled.</returns>
    public static bool IsHighContrastEnabled(Context context)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        return prefs?.GetBoolean(PREF_HIGH_CONTRAST, false) ?? false;
    }
    
    /// <summary>
    /// Gets a theme attribute color value.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="attrId">The attribute resource ID.</param>
    /// <returns>The resolved color value.</returns>
    public static int GetThemeColor(Context context, int attrId)
    {
        var typedValue = new Android.Util.TypedValue();
        context.Theme?.ResolveAttribute(attrId, typedValue, true);
        return typedValue.Data;
    }
}
