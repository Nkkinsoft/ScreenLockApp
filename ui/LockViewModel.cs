using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScreenLockApp.challenges;

namespace ScreenLockApp.ui;

/// <summary>
/// ViewModel for LockActivity
/// </summary>
public class LockViewModel : INotifyPropertyChanged
{
    private IUnlockChallenge? _challenge;
    private string _input = string.Empty;
    private string _errorMessage = string.Empty;
    private string _hintText = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IUnlockChallenge? Challenge
    {
        get => _challenge;
        set
        {
            if (_challenge != value)
            {
                _challenge = value;
                OnPropertyChanged();
                UpdateHintText();
                ClearInput();
            }
        }
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

    public string HintText
    {
        get => _hintText;
        set
        {
            if (_hintText != value)
            {
                _hintText = value;
                OnPropertyChanged();
            }
        }
    }

    public void AppendDigit(string digit)
    {
        if (Challenge == null) return;

        if (Input.Length < Challenge.ExpectedInputLength)
        {
            Input += digit;
            ErrorMessage = string.Empty;
        }
    }

    public void DeleteLastDigit()
    {
        if (Input.Length > 0)
        {
            Input = Input[..^1];
            ErrorMessage = string.Empty;
        }
    }

    public void ClearInput()
    {
        Input = string.Empty;
        ErrorMessage = string.Empty;
    }

    public ValidationResult ValidateInput()
    {
        if (Challenge == null)
            return ValidationResult.Failure("No challenge loaded");

        return Challenge.Validate(Input);
    }

    public void UpdateHintText()
    {
        if (Challenge != null)
        {
            HintText = Challenge.CurrentTargetHint();
        }
    }

    public void SwitchChallenge(IUnlockChallenge newChallenge)
    {
        Challenge = newChallenge;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
