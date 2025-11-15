namespace ScreenLockApp.Core;

/// <summary>
/// View model for the lock screen, managing input buffer and validation logic.
/// </summary>
public class LockViewModel
{
    private readonly TimeValidator _validator;
    private readonly LockCoordinator _coordinator;
    private string _inputBuffer = string.Empty;

    /// <summary>
    /// Event raised when the input buffer changes.
    /// </summary>
    public event EventHandler<string>? InputChanged;

    /// <summary>
    /// Event raised when validation completes.
    /// </summary>
    public event EventHandler<ValidationResult>? ValidationCompleted;

    /// <summary>
    /// Event raised when a hint should be displayed.
    /// </summary>
    public event EventHandler<string>? HintRequested;

    public LockViewModel(TimeValidator validator, LockCoordinator coordinator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
    }

    /// <summary>
    /// Gets the current input buffer.
    /// </summary>
    public string InputBuffer => _inputBuffer;

    /// <summary>
    /// Gets whether the input is complete (4 digits).
    /// </summary>
    public bool IsInputComplete => _inputBuffer.Length == 4;

    /// <summary>
    /// Appends a digit to the input buffer.
    /// </summary>
    public void AppendDigit(char digit)
    {
        if (_inputBuffer.Length >= 4)
        {
            return; // Already at max length
        }

        if (!char.IsDigit(digit))
        {
            return; // Only digits allowed
        }

        _inputBuffer += digit;
        InputChanged?.Invoke(this, _inputBuffer);
    }

    /// <summary>
    /// Clears the input buffer.
    /// </summary>
    public void ClearInput()
    {
        _inputBuffer = string.Empty;
        InputChanged?.Invoke(this, _inputBuffer);
    }

    /// <summary>
    /// Validates the current input and triggers appropriate events.
    /// </summary>
    public void ValidateInput()
    {
        if (!IsInputComplete)
        {
            return; // Nothing to validate
        }

        var result = _validator.ValidateTime(_inputBuffer);
        
        if (result.IsValid)
        {
            _coordinator.RecordUnlock();
        }
        else
        {
            _coordinator.RecordFailure();
            
            // Show hint after threshold
            if (_coordinator.FailCount >= AppConfig.HintFailureThreshold)
            {
                var hint = GenerateHint();
                HintRequested?.Invoke(this, hint);
            }
        }

        ValidationCompleted?.Invoke(this, result);
        
        // Clear input for next attempt
        ClearInput();
    }

    /// <summary>
    /// Generates a hint message based on current tolerance.
    /// </summary>
    private string GenerateHint()
    {
        int tolerance = _validator.ToleranceMinutes;
        
        if (tolerance == 0)
        {
            return "Hint: Enter the current time (exact match required). Tolerance: Exact only";
        }
        else if (tolerance == 1)
        {
            return $"Hint: Enter the current time (±1 minute tolerance). Tolerance: ±{tolerance} minute";
        }
        else
        {
            return $"Hint: Enter the current time (±{tolerance} minutes tolerance). Tolerance: ±{tolerance} minutes";
        }
    }

    /// <summary>
    /// Deletes the last digit from the input buffer.
    /// </summary>
    public void DeleteLastDigit()
    {
        if (_inputBuffer.Length > 0)
        {
            _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);
            InputChanged?.Invoke(this, _inputBuffer);
        }
    }
}
