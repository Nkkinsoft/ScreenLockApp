namespace ScreenLockApp.Core;

/// <summary>
/// Provides access to application preferences/settings.
/// </summary>
public interface IPreferencesProvider
{
    int GetToleranceMinutes();
    void SetToleranceMinutes(int value);
    
    bool GetGracePeriodEnabled();
    void SetGracePeriodEnabled(bool value);
    
    int GetGracePeriodMinutes();
    void SetGracePeriodMinutes(int value);
    
    long GetLastUnlockEpochMs();
    void SetLastUnlockEpochMs(long value);
    
    bool GetLockTaskModeEnabled();
    void SetLockTaskModeEnabled(bool value);
}
