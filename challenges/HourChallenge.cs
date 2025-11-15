using Android.Content;

namespace ScreenLockApp.challenges;

/// <summary>
/// Hour-only unlock challenge (HH format, 00-23)
/// </summary>
public class HourChallenge : IUnlockChallenge
{
    private readonly Context _context;

    public string Id => "hour";
    public string DisplayName => "Hour Challenge";
    public string Description => "Enter only the current hour (00-23). Example: 14 for 2 PM";
    public int ExpectedInputLength => 2;

    public HourChallenge(Context context)
    {
        _context = context;
    }

    public string CurrentTargetHint()
    {
        var now = DateTime.Now;
        return $"Enter current hour (00-23)\nCurrent hour: {now.Hour:D2}";
    }

    public ValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ValidationResult.Failure("Input cannot be empty");

        if (input.Length != 2)
            return ValidationResult.Failure("Hour must be 2 digits (00-23)");

        if (!int.TryParse(input, out int hourValue))
            return ValidationResult.Failure("Invalid hour format");

        if (hourValue < 0 || hourValue > 23)
            return ValidationResult.Failure("Hour must be between 00 and 23");

        var now = DateTime.Now;
        int currentHour = now.Hour;

        if (hourValue == currentHour)
            return ValidationResult.Success();

        return ValidationResult.Failure($"Hour does not match\nExpected: {currentHour:D2}");
    }
}
