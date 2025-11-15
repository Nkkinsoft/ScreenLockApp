namespace ScreenLockApp.Utils;

/// <summary>
/// Validates time-based input with tolerance handling.
/// </summary>
public static class TimeValidator
{
    /// <summary>
    /// Checks if an input time (HHMM format as integer) is within tolerance of the current time.
    /// </summary>
    /// <param name="currentTime">The current date and time.</param>
    /// <param name="inputTime">The input time in HHMM format (e.g., 1430 for 14:30).</param>
    /// <param name="toleranceMinutes">The tolerance in minutes.</param>
    /// <returns>True if input is within tolerance, false otherwise.</returns>
    public static bool IsWithinTolerance(DateTime currentTime, int inputTime, int toleranceMinutes)
    {
        if (inputTime < 0 || inputTime > 2359)
            return false;
            
        int inputHour = inputTime / 100;
        int inputMinute = inputTime % 100;
        
        if (inputHour > 23 || inputMinute > 59)
            return false;
            
        var inputDateTime = new DateTime(
            currentTime.Year,
            currentTime.Month,
            currentTime.Day,
            inputHour,
            inputMinute,
            0
        );
        
        var diff = Math.Abs((currentTime - inputDateTime).TotalMinutes);
        return diff <= toleranceMinutes;
    }
    
    /// <summary>
    /// Converts DateTime to HHMM integer format.
    /// </summary>
    /// <param name="dateTime">The date and time to convert.</param>
    /// <returns>Time in HHMM format as integer.</returns>
    public static int ToHHMM(DateTime dateTime)
    {
        return dateTime.Hour * 100 + dateTime.Minute;
    }
}
