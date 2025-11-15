using Android.Content;
using AndroidX.Preference;
using ScreenLockApp.Challenges;
using ScreenLockApp.Utils;

namespace ScreenLockApp.ViewModels;

/// <summary>
/// Notifies observers when the active challenge changes.
/// </summary>
public delegate void ChallengeChangedEventHandler(object sender, ChallengeChangedEventArgs e);

/// <summary>
/// Event arguments for challenge change events.
/// </summary>
public class ChallengeChangedEventArgs : EventArgs
{
    public string OldChallengeId { get; set; } = string.Empty;
    public string NewChallengeId { get; set; } = string.Empty;
    public IUnlockChallenge? NewChallenge { get; set; }
}

/// <summary>
/// ViewModel for lock screen, manages active challenge and validation.
/// </summary>
public class LockViewModel
{
    private readonly Context _context;
    private readonly ChallengeRegistry _registry;
    private readonly ITimeProvider _timeProvider;
    private IUnlockChallenge? _activeChallenge;
    private string _activeChallengeId = "time";
    private DateTime _lastUnlockTime = DateTime.MinValue;
    private string _inputBuffer = string.Empty;
    
    public event ChallengeChangedEventHandler? ChallengeChanged;
    
    public LockViewModel(Context context, ChallengeRegistry registry, ITimeProvider timeProvider)
    {
        _context = context;
        _registry = registry;
        _timeProvider = timeProvider;
        
        InitializeFromPreferences();
    }
    
    /// <summary>
    /// Gets the current hint text for the active challenge.
    /// </summary>
    public string CurrentHint => _activeChallenge?.GetHint() ?? "No challenge active";
    
    /// <summary>
    /// Gets the current input buffer.
    /// </summary>
    public string InputBuffer => _inputBuffer;
    
    /// <summary>
    /// Gets the active challenge ID.
    /// </summary>
    public string ActiveChallengeId => _activeChallengeId;
    
    /// <summary>
    /// Appends a digit to the input buffer.
    /// </summary>
    /// <param name="digit">The digit to append (0-9).</param>
    public void AppendDigit(char digit)
    {
        if (char.IsDigit(digit))
        {
            _inputBuffer += digit;
        }
    }
    
    /// <summary>
    /// Clears the input buffer.
    /// </summary>
    public void ClearBuffer()
    {
        _inputBuffer = string.Empty;
    }
    
    /// <summary>
    /// Validates the current input buffer.
    /// </summary>
    /// <returns>True if validation succeeds, false otherwise.</returns>
    public bool ValidateInput()
    {
        if (_activeChallenge == null)
            return false;
            
        // Check grace period
        if (IsInGracePeriod())
            return true;
            
        var result = _activeChallenge.Validate(_inputBuffer);
        
        if (result)
        {
            _lastUnlockTime = _timeProvider.Now;
            LogDebug($"Validation succeeded for challenge {_activeChallengeId}");
        }
        else
        {
            LogDebug($"Validation failed for challenge {_activeChallengeId}: input={_inputBuffer}");
        }
        
        return result;
    }
    
    /// <summary>
    /// Switches to a different challenge.
    /// </summary>
    /// <param name="challengeId">The ID of the challenge to switch to.</param>
    public void SwitchChallenge(string challengeId)
    {
        var oldId = _activeChallengeId;
        var newChallenge = _registry.Get(challengeId);
        
        if (newChallenge == null)
            return;
            
        _activeChallengeId = challengeId;
        _activeChallenge = newChallenge;
        ClearBuffer();
        
        // Save to preferences
        var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
        prefs?.Edit()?.PutString("active_challenge_id", challengeId)?.Apply();
        
        // Notify observers
        ChallengeChanged?.Invoke(this, new ChallengeChangedEventArgs
        {
            OldChallengeId = oldId,
            NewChallengeId = challengeId,
            NewChallenge = newChallenge
        });
        
        LogDebug($"Challenge switched from {oldId} to {challengeId}");
    }
    
    /// <summary>
    /// Updates the active challenge state.
    /// </summary>
    public void UpdateChallenge()
    {
        _activeChallenge?.Update();
    }
    
    private void InitializeFromPreferences()
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
        _activeChallengeId = prefs?.GetString("active_challenge_id", "time") ?? "time";
        _activeChallenge = _registry.Get(_activeChallengeId);
    }
    
    private bool IsInGracePeriod()
    {
        if (_lastUnlockTime == DateTime.MinValue)
            return false;
            
        var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
        var gracePeriodMinutes = prefs?.GetInt("grace_period_minutes", 0) ?? 0;
        
        if (gracePeriodMinutes == 0)
            return false;
            
        var elapsed = (_timeProvider.Now - _lastUnlockTime).TotalMinutes;
        return elapsed < gracePeriodMinutes;
    }
    
    private void LogDebug(string message)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(_context);
        var debugMode = prefs?.GetBoolean("debug_mode", false) ?? false;
        
        if (debugMode)
        {
            Android.Util.Log.Debug("LockViewModel", message);
        }
    }
}
