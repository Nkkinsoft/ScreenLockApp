using Android.Content;

namespace ScreenLockApp.data;

/// <summary>
/// Manages app preferences using SharedPreferences
/// </summary>
public class PreferencesProvider
{
    private const string PrefsName = "ScreenLockAppPrefs";
    private readonly ISharedPreferences _prefs;

    // Preference keys
    private const string KeyActiveChallengeId = "active_challenge_id";
    private const string KeyTimeTolerance = "time_tolerance";
    private const string KeyBatteryTolerance = "battery_tolerance";
    private const string KeyGracePeriod = "grace_period";
    private const string KeyEnableAutoStart = "enable_auto_start";
    private const string KeyEnableHaptics = "enable_haptics";
    private const string KeyEnableKioskMode = "enable_kiosk_mode";
    private const string KeyThemeMode = "theme_mode";
    private const string KeyHighContrast = "high_contrast";
    private const string KeyStrictMode = "strict_mode";
    private const string KeyEnableNtpStub = "enable_ntp_stub";
    private const string KeyEnableBiometricStub = "enable_biometric_stub";

    public PreferencesProvider(Context context)
    {
        _prefs = context.GetSharedPreferences(PrefsName, FileCreationMode.Private)!;
    }

    // Active Challenge
    public string GetActiveChallengeId() => _prefs.GetString(KeyActiveChallengeId, "time") ?? "time";
    public void SetActiveChallengeId(string challengeId)
    {
        _prefs.Edit()!.PutString(KeyActiveChallengeId, challengeId).Apply();
    }

    // Time Tolerance (in minutes)
    public int GetTimeTolerance() => _prefs.GetInt(KeyTimeTolerance, 0);
    public void SetTimeTolerance(int tolerance)
    {
        _prefs.Edit()!.PutInt(KeyTimeTolerance, tolerance).Apply();
    }

    // Battery Tolerance (in percentage points)
    public int GetBatteryTolerance() => _prefs.GetInt(KeyBatteryTolerance, 0);
    public void SetBatteryTolerance(int tolerance)
    {
        _prefs.Edit()!.PutInt(KeyBatteryTolerance, tolerance).Apply();
    }

    // Grace Period (in minutes, 0 = off)
    public int GetGracePeriod() => _prefs.GetInt(KeyGracePeriod, 0);
    public void SetGracePeriod(int minutes)
    {
        _prefs.Edit()!.PutInt(KeyGracePeriod, minutes).Apply();
    }

    // Auto Start on Boot
    public bool GetEnableAutoStart() => _prefs.GetBoolean(KeyEnableAutoStart, false);
    public void SetEnableAutoStart(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyEnableAutoStart, enabled).Apply();
    }

    // Haptic Feedback
    public bool GetEnableHaptics() => _prefs.GetBoolean(KeyEnableHaptics, true);
    public void SetEnableHaptics(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyEnableHaptics, enabled).Apply();
    }

    // Kiosk Mode
    public bool GetEnableKioskMode() => _prefs.GetBoolean(KeyEnableKioskMode, false);
    public void SetEnableKioskMode(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyEnableKioskMode, enabled).Apply();
    }

    // Theme Mode (dark, light, system)
    public string GetThemeMode() => _prefs.GetString(KeyThemeMode, "system") ?? "system";
    public void SetThemeMode(string mode)
    {
        _prefs.Edit()!.PutString(KeyThemeMode, mode).Apply();
    }

    // High Contrast
    public bool GetHighContrast() => _prefs.GetBoolean(KeyHighContrast, false);
    public void SetHighContrast(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyHighContrast, enabled).Apply();
    }

    // Strict Mode (forces tolerance = 0 for time challenge)
    public bool GetStrictMode() => _prefs.GetBoolean(KeyStrictMode, false);
    public void SetStrictMode(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyStrictMode, enabled).Apply();
    }

    // NTP Stub (placeholder for future feature)
    public bool GetEnableNtpStub() => _prefs.GetBoolean(KeyEnableNtpStub, false);
    public void SetEnableNtpStub(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyEnableNtpStub, enabled).Apply();
    }

    // Biometric Stub (placeholder for future feature)
    public bool GetEnableBiometricStub() => _prefs.GetBoolean(KeyEnableBiometricStub, false);
    public void SetEnableBiometricStub(bool enabled)
    {
        _prefs.Edit()!.PutBoolean(KeyEnableBiometricStub, enabled).Apply();
    }
}
