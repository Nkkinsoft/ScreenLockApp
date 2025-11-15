namespace ScreenLockApp.Core;

/// <summary>
/// Represents the result of time validation.
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
}

/// <summary>
/// Validates time-based authentication with ±1 minute tolerance.
/// Handles midnight wrap-around and edge cases.
/// </summary>
public class TimeValidator
{
    private const int ToleranceMinutes = 1;

    /// <summary>
    /// Validates the input time against the current system time.
    /// </summary>
    /// <param name="input">User input in HHMM format (e.g., "1327")</param>
    /// <returns>ValidationResult indicating success or failure with error message</returns>
    public ValidationResult ValidateTime(string input)
    {
        // Validate input length
        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationResult.Failure("Input cannot be empty");
        }

        if (input.Length != 4)
        {
            return ValidationResult.Failure("Input must be exactly 4 digits (HHMM)");
        }

        // Validate numeric input
        if (!int.TryParse(input, out int timeValue))
        {
            return ValidationResult.Failure("Input must contain only numbers");
        }

        // Parse hours and minutes
        int inputHour = timeValue / 100;
        int inputMinute = timeValue % 100;

        // Validate hour range (00-23)
        if (inputHour < 0 || inputHour > 23)
        {
            return ValidationResult.Failure("Hour must be between 00 and 23");
        }

        // Validate minute range (00-59)
        if (inputMinute < 0 || inputMinute > 59)
        {
            return ValidationResult.Failure("Minute must be between 00 and 59");
        }

        // Get current system time
        DateTime now = DateTime.Now;
        DateTime inputTime = new DateTime(now.Year, now.Month, now.Day, inputHour, inputMinute, 0);

        // Check if input time is within tolerance (±1 minute)
        if (IsWithinTolerance(now, inputTime))
        {
            return ValidationResult.Success();
        }

        return ValidationResult.Failure("Time does not match current time window");
    }

    /// <summary>
    /// Checks if the input time is within ±1 minute tolerance of the current time.
    /// Handles midnight wrap-around correctly.
    /// </summary>
    private bool IsWithinTolerance(DateTime current, DateTime input)
    {
        // Extract just the time (hours and minutes) by zeroing out seconds
        int currentMinutesFromMidnight = current.Hour * 60 + current.Minute;
        int inputMinutesFromMidnight = input.Hour * 60 + input.Minute;
        
        // Calculate the difference in minutes
        int difference = inputMinutesFromMidnight - currentMinutesFromMidnight;

        // Handle midnight wrap-around
        // If the difference is large, it means we wrapped around midnight
        if (difference > 720) // More than 12 hours ahead (wrapped backward)
        {
            difference -= 1440; // Subtract 24 hours in minutes
        }
        else if (difference < -720) // More than 12 hours behind (wrapped forward)
        {
            difference += 1440; // Add 24 hours in minutes
        }

        // Check if within ±1 minute tolerance
        return Math.Abs(difference) <= ToleranceMinutes;
    }

    /// <summary>
    /// Validates time with a custom current time (for testing purposes).
    /// </summary>
    public ValidationResult ValidateTimeWithCustomCurrent(string input, DateTime currentTime)
    {
        // Validate input length
        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationResult.Failure("Input cannot be empty");
        }

        if (input.Length != 4)
        {
            return ValidationResult.Failure("Input must be exactly 4 digits (HHMM)");
        }

        // Validate numeric input
        if (!int.TryParse(input, out int timeValue))
        {
            return ValidationResult.Failure("Input must contain only numbers");
        }

        // Parse hours and minutes
        int inputHour = timeValue / 100;
        int inputMinute = timeValue % 100;

        // Validate hour range (00-23)
        if (inputHour < 0 || inputHour > 23)
        {
            return ValidationResult.Failure("Hour must be between 00 and 23");
        }

        // Validate minute range (00-59)
        if (inputMinute < 0 || inputMinute > 59)
        {
            return ValidationResult.Failure("Minute must be between 00 and 59");
        }

        // Create input time using current date
        DateTime inputTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, inputHour, inputMinute, 0);

        // Check if input time is within tolerance (±1 minute)
        if (IsWithinTolerance(currentTime, inputTime))
        {
            return ValidationResult.Success();
        }

        return ValidationResult.Failure("Time does not match current time window");
    }
}
