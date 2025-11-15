using Android.Content;

namespace ScreenLockApp.Data;

/// <summary>
/// Provides access to SharedPreferences for storing app settings.
/// </summary>
public class PreferencesProvider
{
    private const string PrefsName = "ScreenLockPrefs";
    private const string KeyLockEnabled = "lock_enabled";
    private const string KeyAutoStartOnBoot = "auto_start_on_boot";
    private const string KeyFailureCount = "failure_count";
    private const string KeyLastFailureTime = "last_failure_time";
    private const string KeySelectedChallenge = "selected_challenge";
    private const string KeyBatteryTolerance = "battery_tolerance";
    private const string KeyStrictMode = "strict_mode";

    private readonly ISharedPreferences _prefs;
    private readonly ISharedPreferencesEditor _editor;

    public PreferencesProvider(Context context)
    {
        _prefs = context.GetSharedPreferences(PrefsName, FileCreationMode.Private)!;
        _editor = _prefs.Edit()!;
    }

    /// <summary>
    /// Gets or sets whether the lock is enabled.
    /// </summary>
    public bool IsLockEnabled
    {
        get => _prefs.GetBoolean(KeyLockEnabled, false);
        set
        {
            _editor.PutBoolean(KeyLockEnabled, value);
            _editor.Apply();
        }
    }

    /// <summary>
    /// Gets or sets whether the service should auto-start on boot.
    /// </summary>
    public bool AutoStartOnBoot
    {
        get => _prefs.GetBoolean(KeyAutoStartOnBoot, false);
        set
        {
            _editor.PutBoolean(KeyAutoStartOnBoot, value);
            _editor.Apply();
        }
    }

    /// <summary>
    /// Gets or sets the count of consecutive failures.
    /// </summary>
    public int FailureCount
    {
        get => _prefs.GetInt(KeyFailureCount, 0);
        set
        {
            _editor.PutInt(KeyFailureCount, value);
            _editor.Apply();
        }
    }

    /// <summary>
    /// Gets or sets the timestamp of the last failure (in milliseconds).
    /// </summary>
    public long LastFailureTime
    {
        get => _prefs.GetLong(KeyLastFailureTime, 0);
        set
        {
            _editor.PutLong(KeyLastFailureTime, value);
            _editor.Apply();
        }
    }

    /// <summary>
    /// Resets the failure count.
    /// </summary>
    public void ResetFailureCount()
    {
        FailureCount = 0;
        LastFailureTime = 0;
    }

    /// <summary>
    /// Increments the failure count.
    /// </summary>
    public void IncrementFailureCount()
    {
        FailureCount++;
        LastFailureTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Gets or sets the selected challenge ID.
    /// </summary>
    public string SelectedChallenge
    {
        get => _prefs.GetString(KeySelectedChallenge, "time") ?? "time";
        set
        {
            _editor.PutString(KeySelectedChallenge, value);
            _editor.Apply();
        }
    }

    /// <summary>
    /// Gets or sets the battery tolerance (for battery-based challenges).
    /// </summary>
    public int BatteryTolerance
    {
        get => _prefs.GetInt(KeyBatteryTolerance, 1);
        set
        {
            _editor.PutInt(KeyBatteryTolerance, value);
            _editor.Apply();
        }
    }

    /// <summary>
    /// Gets or sets whether strict mode is enabled.
    /// </summary>
    public bool StrictMode
    {
        get => _prefs.GetBoolean(KeyStrictMode, false);
        set
        {
            _editor.PutBoolean(KeyStrictMode, value);
            _editor.Apply();
        }
    }
}
