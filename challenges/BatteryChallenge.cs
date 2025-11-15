using Android.Content;
using ScreenLockApp.providers;

namespace ScreenLockApp.challenges;

/// <summary>
/// Battery percentage unlock challenge
/// </summary>
public class BatteryChallenge : IUnlockChallenge
{
    private readonly Context _context;
    private readonly BatteryStatusProvider _batteryProvider;
    private readonly int _tolerance;

    public string Id => "battery";
    public string DisplayName => "Battery Challenge";
    public string Description => "Enter the current battery percentage as a number. Example: 57 for 57%";
    public int ExpectedInputLength => 3; // Up to 100

    public BatteryChallenge(Context context, int tolerance = 0)
    {
        _context = context;
        _batteryProvider = new BatteryStatusProvider(context);
        _tolerance = tolerance;
    }

    public string CurrentTargetHint()
    {
        int currentBattery = _batteryProvider.GetBatteryPercentage();
        return $"Enter current battery % (no % sign)\nCurrent: {currentBattery}%, Tolerance: ±{_tolerance}%";
    }

    public ValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ValidationResult.Failure("Input cannot be empty");

        if (!int.TryParse(input, out int inputValue))
            return ValidationResult.Failure("Invalid number format");

        if (inputValue < 0 || inputValue > 100)
            return ValidationResult.Failure("Battery percentage must be between 0 and 100");

        int currentBattery = _batteryProvider.GetBatteryPercentage();
        int diff = Math.Abs(currentBattery - inputValue);

        if (diff <= _tolerance)
            return ValidationResult.Success();

        return ValidationResult.Failure($"Battery level does not match (within ±{_tolerance}% tolerance)\nActual: {currentBattery}%");
    }
}
