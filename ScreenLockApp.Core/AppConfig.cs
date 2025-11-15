namespace ScreenLockApp.Core;

/// <summary>
/// Application configuration constants.
/// </summary>
public static class AppConfig
{
    /// <summary>
    /// Default tolerance in minutes (Normal mode).
    /// </summary>
    public const int DefaultToleranceMinutes = 1;

    /// <summary>
    /// Number of failures before showing hint with tolerance information.
    /// </summary>
    public const int HintFailureThreshold = 3;

    /// <summary>
    /// Default grace period in minutes after successful unlock.
    /// </summary>
    public const int GracePeriodMinutes = 2;

    /// <summary>
    /// Preference key for tolerance setting.
    /// </summary>
    public const string PrefKeyToleranceMinutes = "tolerance_minutes";

    /// <summary>
    /// Preference key for grace period enabled.
    /// </summary>
    public const string PrefKeyGracePeriodEnabled = "grace_period_enabled";

    /// <summary>
    /// Preference key for grace period duration in minutes.
    /// </summary>
    public const string PrefKeyGracePeriodMinutes = "grace_period_minutes";

    /// <summary>
    /// Preference key for last unlock timestamp (epoch milliseconds).
    /// </summary>
    public const string PrefKeyLastUnlockEpochMs = "last_unlock_epoch_ms";

    /// <summary>
    /// Preference key for lock task mode enabled.
    /// </summary>
    public const string PrefKeyLockTaskModeEnabled = "lock_task_mode_enabled";
}
