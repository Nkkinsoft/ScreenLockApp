namespace ScreenLockApp.Core;

/// <summary>
/// Coordinates lock/unlock state across the application.
/// Thread-safe singleton pattern.
/// </summary>
public class LockCoordinator
{
    private readonly object _lockObject = new object();
    private bool _isLocked;
    private int _failCount;
    private int _activeTolerance;

    /// <summary>
    /// Gets whether the screen is currently locked.
    /// </summary>
    public bool IsLocked
    {
        get { lock (_lockObject) { return _isLocked; } }
        private set { lock (_lockObject) { _isLocked = value; } }
    }

    /// <summary>
    /// Gets the current failure count.
    /// </summary>
    public int FailCount
    {
        get { lock (_lockObject) { return _failCount; } }
        private set { lock (_lockObject) { _failCount = value; } }
    }

    /// <summary>
    /// Gets the active tolerance in minutes.
    /// </summary>
    public int ActiveTolerance
    {
        get { lock (_lockObject) { return _activeTolerance; } }
        private set { lock (_lockObject) { _activeTolerance = value; } }
    }

    /// <summary>
    /// Event raised when lock state changes.
    /// </summary>
    public event EventHandler<bool>? LockStateChanged;

    /// <summary>
    /// Event raised when failure count changes.
    /// </summary>
    public event EventHandler<int>? FailCountChanged;

    public LockCoordinator()
    {
        _isLocked = false;
        _failCount = 0;
        _activeTolerance = AppConfig.DefaultToleranceMinutes;
    }

    /// <summary>
    /// Triggers the lock screen.
    /// </summary>
    public void TriggerLock(int tolerance)
    {
        lock (_lockObject)
        {
            _isLocked = true;
            _activeTolerance = tolerance;
        }
        LockStateChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Records a validation failure.
    /// </summary>
    public void RecordFailure()
    {
        int newCount;
        lock (_lockObject)
        {
            _failCount++;
            newCount = _failCount;
        }
        FailCountChanged?.Invoke(this, newCount);
    }

    /// <summary>
    /// Records a successful unlock and resets state.
    /// </summary>
    public void RecordUnlock()
    {
        lock (_lockObject)
        {
            _isLocked = false;
            _failCount = 0;
        }
        LockStateChanged?.Invoke(this, false);
        FailCountChanged?.Invoke(this, 0);
    }

    /// <summary>
    /// Resets the failure count without unlocking.
    /// </summary>
    public void ResetFailCount()
    {
        lock (_lockObject)
        {
            _failCount = 0;
        }
        FailCountChanged?.Invoke(this, 0);
    }
}
