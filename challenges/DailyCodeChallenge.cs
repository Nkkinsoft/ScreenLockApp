using Android.Content;

namespace ScreenLockApp.challenges;

/// <summary>
/// Daily code challenge based on current date (BETA/STUB)
/// Code format: DDWD (DD = day of month, W = day of week 0-6)
/// Example: For June 15th (Wednesday), code would be "153" (15 = day, 3 = Wednesday)
/// </summary>
public class DailyCodeChallenge : IUnlockChallenge
{
    private readonly Context _context;

    public string Id => "dailycode";
    public string DisplayName => "Daily Code (Beta)";
    public string Description => "Enter today's code derived from the date.\nFormat: DDWD where DD=day of month, W=day of week (0=Sun)\nExample: 15th Wednesday = 153";
    public int ExpectedInputLength => 3; // DD + W

    public DailyCodeChallenge(Context context)
    {
        _context = context;
    }

    public string CurrentTargetHint()
    {
        var now = DateTime.Now;
        int dayOfMonth = now.Day;
        int dayOfWeek = (int)now.DayOfWeek; // 0=Sunday, 6=Saturday
        string expectedCode = $"{dayOfMonth:D2}{dayOfWeek}";
        
        return $"Enter today's code (DDWD format)\nToday: {now:dddd, MMMM dd}\nExpected format: DD (day) + W (weekday 0-6)";
    }

    public ValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ValidationResult.Failure("Input cannot be empty");

        if (input.Length != 3)
            return ValidationResult.Failure("Daily code must be 3 digits (DDWD)");

        if (!int.TryParse(input, out int codeValue))
            return ValidationResult.Failure("Invalid code format");

        var now = DateTime.Now;
        int dayOfMonth = now.Day;
        int dayOfWeek = (int)now.DayOfWeek;
        string expectedCode = $"{dayOfMonth:D2}{dayOfWeek}";

        if (input == expectedCode)
            return ValidationResult.Success();

        return ValidationResult.Failure($"Daily code does not match\nHint: Check day and weekday number");
    }
}
