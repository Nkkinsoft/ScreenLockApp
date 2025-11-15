using Android.Content;
using ScreenLockApp.Core;

namespace ScreenLockApp;

/// <summary>
/// Android implementation of preferences provider using SharedPreferences.
/// </summary>
public class PreferencesProvider : IPreferencesProvider
{
    private readonly ISharedPreferences _preferences;
    private const string PrefsName = "ScreenLockAppPrefs";

    public PreferencesProvider(Context context)
    {
        _preferences = context.GetSharedPreferences(PrefsName, FileCreationMode.Private)
            ?? throw new InvalidOperationException("Failed to get shared preferences");
    }

    public int GetToleranceMinutes()
    {
        return _preferences.GetInt(AppConfig.PrefKeyToleranceMinutes, AppConfig.DefaultToleranceMinutes);
    }

    public void SetToleranceMinutes(int value)
    {
        var editor = _preferences.Edit();
        editor?.PutInt(AppConfig.PrefKeyToleranceMinutes, value);
        editor?.Apply();
    }

    public bool GetGracePeriodEnabled()
    {
        return _preferences.GetBoolean(AppConfig.PrefKeyGracePeriodEnabled, true);
    }

    public void SetGracePeriodEnabled(bool value)
    {
        var editor = _preferences.Edit();
        editor?.PutBoolean(AppConfig.PrefKeyGracePeriodEnabled, value);
        editor?.Apply();
    }

    public int GetGracePeriodMinutes()
    {
        return _preferences.GetInt(AppConfig.PrefKeyGracePeriodMinutes, AppConfig.GracePeriodMinutes);
    }

    public void SetGracePeriodMinutes(int value)
    {
        var editor = _preferences.Edit();
        editor?.PutInt(AppConfig.PrefKeyGracePeriodMinutes, value);
        editor?.Apply();
    }

    public long GetLastUnlockEpochMs()
    {
        return _preferences.GetLong(AppConfig.PrefKeyLastUnlockEpochMs, 0);
    }

    public void SetLastUnlockEpochMs(long value)
    {
        var editor = _preferences.Edit();
        editor?.PutLong(AppConfig.PrefKeyLastUnlockEpochMs, value);
        editor?.Apply();
    }

    public bool GetLockTaskModeEnabled()
    {
        return _preferences.GetBoolean(AppConfig.PrefKeyLockTaskModeEnabled, false);
    }

    public void SetLockTaskModeEnabled(bool value)
    {
        var editor = _preferences.Edit();
        editor?.PutBoolean(AppConfig.PrefKeyLockTaskModeEnabled, value);
        editor?.Apply();
    }
}
