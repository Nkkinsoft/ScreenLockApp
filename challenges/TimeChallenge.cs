using Android.Content;

namespace ScreenLockApp.challenges;

/// <summary>
/// Time-based unlock challenge (HHMM format)
/// </summary>
public class TimeChallenge : IUnlockChallenge
{
    private readonly Context _context;
    private readonly int _tolerance;

    public string Id => "time";
    public string DisplayName => "Time Challenge";
    public string Description => "Enter the current time in HHMM format (24-hour). Example: 1430 for 2:30 PM";
    public int ExpectedInputLength => 4;

    public TimeChallenge(Context context, int tolerance = 0)
    {
        _context = context;
        _tolerance = tolerance;
    }

    public string CurrentTargetHint()
    {
        var now = DateTime.Now;
        return $"Enter current time (HHMM)\nTolerance: ±{_tolerance} minute(s)";
    }

    public ValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ValidationResult.Failure("Input cannot be empty");

        if (input.Length != 4)
            return ValidationResult.Failure("Time must be 4 digits (HHMM)");

        if (!int.TryParse(input, out int timeValue))
            return ValidationResult.Failure("Invalid time format");

        int inputHour = timeValue / 100;
        int inputMinute = timeValue % 100;

        if (inputHour < 0 || inputHour > 23)
            return ValidationResult.Failure("Hour must be between 00 and 23");

        if (inputMinute < 0 || inputMinute > 59)
            return ValidationResult.Failure("Minute must be between 00 and 59");

        var now = DateTime.Now;
        int currentHour = now.Hour;
        int currentMinute = now.Minute;

        // Calculate difference in minutes
        int currentTotalMinutes = currentHour * 60 + currentMinute;
        int inputTotalMinutes = inputHour * 60 + inputMinute;
        int diff = Math.Abs(currentTotalMinutes - inputTotalMinutes);

        // Handle day boundary (e.g., 23:59 vs 00:01)
        if (diff > 720) // More than 12 hours apart
            diff = 1440 - diff; // Wrap around

        if (diff <= _tolerance)
            return ValidationResult.Success();

        return ValidationResult.Failure($"Time does not match (within ±{_tolerance} min tolerance)");
    }
}
