using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScreenLockApp.Core;
using ScreenLockApp.Core.Challenges;

namespace ScreenLockApp.UI;

/// <summary>
/// ViewModel for the LockActivity.
/// Handles challenge validation logic and UI state.
/// </summary>
public class LockViewModel : INotifyPropertyChanged
{
    private IUnlockChallenge _challenge;
    private string _input = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isValidating;
    private int _failureCount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public LockViewModel(IUnlockChallenge challenge)
    {
        _challenge = challenge ?? throw new ArgumentNullException(nameof(challenge));
    }

    public string Input
    {
        get => _input;
        set
        {
            if (_input != value)
            {
                _input = value;
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
    /// Gets the required input length from the challenge.
    /// </summary>
    public int RequiredLength => _challenge.RequiredLength;

    /// <summary>
    /// Gets the challenge display name.
    /// </summary>
    public string ChallengeDisplayName => _challenge.DisplayName;

    /// <summary>
    /// Updates the challenge being used.
    /// </summary>
    public void UpdateChallenge(IUnlockChallenge challenge)
    {
        _challenge = challenge ?? throw new ArgumentNullException(nameof(challenge));
        Input = string.Empty;
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(RequiredLength));
        OnPropertyChanged(nameof(ChallengeDisplayName));
    }

    /// <summary>
    /// Validates the current input.
    /// </summary>
    /// <returns>True if validation succeeded, false otherwise.</returns>
    public async Task<bool> ValidateAsync()
    {
        IsValidating = true;
        
        try
        {
            // Simulate async validation (e.g., for network checks in the future)
            await Task.Delay(500);
            
            var result = _challenge.Validate(Input);
            
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
            return _challenge.GetHint();
        }
        return string.Empty;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
