using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScreenLockApp.Core;

namespace ScreenLockApp.UI;

/// <summary>
/// ViewModel for the LockActivity.
/// Handles time validation logic and UI state.
/// </summary>
public class LockViewModel : INotifyPropertyChanged
{
    private readonly TimeValidator _timeValidator;
    private string _timeInput = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isValidating;
    private int _failureCount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public LockViewModel()
    {
        _timeValidator = new TimeValidator();
    }

    public string TimeInput
    {
        get => _timeInput;
        set
        {
            if (_timeInput != value)
            {
                _timeInput = value;
                OnPropertyChanged();
                // Clear error when user starts typing
                ErrorMessage = string.Empty;
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsValidating
    {
        get => _isValidating;
        set
        {
            if (_isValidating != value)
            {
                _isValidating = value;
                OnPropertyChanged();
            }
        }
    }

    public int FailureCount
    {
        get => _failureCount;
        set
        {
            if (_failureCount != value)
            {
                _failureCount = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Validates the current time input.
    /// </summary>
    /// <returns>True if validation succeeded, false otherwise.</returns>
    public async Task<bool> ValidateTimeAsync()
    {
        IsValidating = true;
        
        try
        {
            // Simulate async validation (e.g., for network checks in the future)
            await Task.Delay(500);
            
            var result = _timeValidator.ValidateTime(TimeInput);
            
            if (result.IsValid)
            {
                FailureCount = 0;
                return true;
            }
            else
            {
                FailureCount++;
                ErrorMessage = result.ErrorMessage ?? "Unknown error";
                return false;
            }
        }
        finally
        {
            IsValidating = false;
        }
    }

    /// <summary>
    /// Gets a hint message to display after multiple failures.
    /// </summary>
    public string GetHintMessage()
    {
        if (FailureCount >= 3)
        {
            return "Use current 24h time HHMM";
        }
        return string.Empty;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
