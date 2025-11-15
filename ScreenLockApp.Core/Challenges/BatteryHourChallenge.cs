namespace ScreenLockApp.Core.Challenges;

/// <summary>
/// Composite unlock challenge that requires Hour + Battery percentage.
/// Format: HHPP (2-digit hour + 2-digit battery percent)
/// - Hour: 00-23 (24-hour format)
/// - Battery: 0-99 (100 is capped to 99 to maintain fixed 4-digit length)
/// - Tolerance: Hour must match exactly, battery allows ±1% tolerance
/// </summary>
public class BatteryHourChallenge : IUnlockChallenge
{
    private readonly Func<int> _getBatteryPercent;
    private readonly bool _strictMode;
    private readonly int _batteryTolerance;

    /// <summary>
    /// Creates a new BatteryHourChallenge with default settings.
    /// </summary>
    /// <param name="getBatteryPercent">Function to get current battery percentage (0-100)</param>
    /// <param name="batteryTolerance">Battery tolerance in percentage (default: 1)</param>
    /// <param name="strictMode">If true, battery tolerance is forced to 0</param>
    public BatteryHourChallenge(
        Func<int> getBatteryPercent,
        int batteryTolerance = 1,
        bool strictMode = false)
    {
        _getBatteryPercent = getBatteryPercent ?? throw new ArgumentNullException(nameof(getBatteryPercent));
        _batteryTolerance = strictMode ? 0 : batteryTolerance;
        _strictMode = strictMode;
    }

    public string ChallengeId => "battery_hour";

    public string DisplayName => "Battery + Hour";

    public string Description => "Enter current hour and battery percentage (HHPP)";

    public int RequiredLength => 4;

    public ValidationResult Validate(string input)
    {
        // Validate input length
        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationResult.Failure("Input cannot be empty");
        }

        if (input.Length != 4)
        {
            return ValidationResult.Failure("Input must be exactly 4 digits (HHPP)");
        }

        // Validate numeric input
        if (!int.TryParse(input, out int value))
        {
            return ValidationResult.Failure("Input must contain only numbers");
        }

        // Parse hour and battery
        int hourCandidate = value / 100;
        int percentCandidate = value % 100;

        // Validate hour range (00-23)
        if (hourCandidate < 0 || hourCandidate > 23)
        {
            return ValidationResult.Failure("Hour must be between 00 and 23");
        }

        // Validate battery range (00-99)
        if (percentCandidate < 0 || percentCandidate > 99)
        {
            return ValidationResult.Failure("Battery percent must be between 00 and 99");
        }

        // Get current hour
        int currentHour = DateTime.Now.Hour;

        // Check hour match (must be exact)
        if (hourCandidate != currentHour)
        {
            return ValidationResult.Failure($"Hour mismatch. Current hour is {currentHour:D2}");
        }

        // Get current battery percentage
        int currentBattery = _getBatteryPercent();

        // Handle unavailable battery
        if (currentBattery < 0)
        {
            return ValidationResult.Failure("Battery status unavailable");
        }

        // Cap battery at 99 as per specification
        if (currentBattery > 99)
        {
            currentBattery = 99;
        }

        // Check battery match within tolerance
        int batteryDifference = Math.Abs(percentCandidate - currentBattery);
        if (batteryDifference > _batteryTolerance)
        {
            return ValidationResult.Failure(
                $"Battery percent mismatch. Current battery is {currentBattery:D2}% " +
                $"(tolerance: ±{_batteryTolerance}%)");
        }

        return ValidationResult.Success();
    }

    public string GetHint()
    {
        var now = DateTime.Now;
        int battery = _getBatteryPercent();
        
        // Handle unavailable battery
        if (battery < 0)
        {
            return $"Enter HH + PP (Hour + Battery%). Battery status unavailable.";
        }
        
        // Cap battery at 99
        if (battery > 99)
        {
            battery = 99;
        }

        return $"Enter HH + PP (Hour + Battery%). Now: {now:HH} + {battery:D2} => {now:HH}{battery:D2}";
    }
}
