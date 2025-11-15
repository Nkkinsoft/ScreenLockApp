using FsCheck;
using FsCheck.NUnit;
using NUnit.Framework;
using ScreenLockApp.Core;

namespace ScreenLockApp.Tests.Core;

[TestFixture]
public class TimeValidatorPropertyTests
{
    private class TestTimeProvider : ITimeProvider
    {
        public DateTime Now { get; set; }
        
        public TestTimeProvider(DateTime now)
        {
            Now = now;
        }
    }

    [FsCheck.NUnit.Property]
    public bool Property_ExactMatch_AlwaysValid(int hour, int minute, int tolerance)
    {
        // Arrange - constrain inputs to valid ranges
        hour = Math.Abs(hour) % 24;
        minute = Math.Abs(minute) % 60;
        tolerance = Math.Abs(tolerance) % 3; // 0, 1, or 2

        var currentTime = new DateTime(2025, 11, 15, hour, minute, 30);
        var timeProvider = new TestTimeProvider(currentTime);
        var validator = new TimeValidator(tolerance, timeProvider);
        var input = $"{hour:D2}{minute:D2}";

        // Act
        var result = validator.ValidateTime(input);

        // Assert - exact match should always be valid regardless of tolerance
        return result.IsValid;
    }

    [FsCheck.NUnit.Property]
    public bool Property_WithinTolerance_IsValid(int hour, int minute, int offsetMinutes, int tolerance)
    {
        // Arrange - constrain inputs to valid ranges
        hour = Math.Abs(hour) % 24;
        minute = Math.Abs(minute) % 60;
        tolerance = Math.Abs(tolerance) % 3; // 0, 1, or 2
        
        if (tolerance == 0)
        {
            // With 0 tolerance, only test exact match
            offsetMinutes = 0;
        }
        else
        {
            // Ensure offset is within tolerance range
            offsetMinutes = (Math.Abs(offsetMinutes) % (tolerance + 1)); // 0 to tolerance
            if (offsetMinutes > tolerance / 2)
            {
                offsetMinutes = -offsetMinutes; // Make some negative
            }
        }

        var currentTime = new DateTime(2025, 11, 15, hour, minute, 30);
        var timeProvider = new TestTimeProvider(currentTime);
        var validator = new TimeValidator(tolerance, timeProvider);

        // Calculate input time with offset
        var inputTime = currentTime.AddMinutes(offsetMinutes);
        var input = $"{inputTime.Hour:D2}{inputTime.Minute:D2}";

        // Act
        var result = validator.ValidateTime(input);

        // Assert - should be valid when within tolerance
        return result.IsValid;
    }

    [FsCheck.NUnit.Property]
    public bool Property_OutsideTolerance_IsInvalid(int hour, int minute, int tolerance, bool isPositiveOffset)
    {
        // Arrange - constrain inputs to valid ranges
        hour = Math.Abs(hour) % 24;
        minute = Math.Abs(minute) % 60;
        tolerance = Math.Abs(tolerance) % 3; // 0, 1, or 2

        // Calculate offset just outside tolerance
        int offsetMinutes = isPositiveOffset ? tolerance + 1 : -(tolerance + 1);

        var currentTime = new DateTime(2025, 11, 15, hour, minute, 30);
        var timeProvider = new TestTimeProvider(currentTime);
        var validator = new TimeValidator(tolerance, timeProvider);

        // Calculate input time with offset
        var inputTime = currentTime.AddMinutes(offsetMinutes);
        var input = $"{inputTime.Hour:D2}{inputTime.Minute:D2}";

        // Act
        var result = validator.ValidateTime(input);

        // Assert - should be invalid when outside tolerance
        return !result.IsValid;
    }

    [FsCheck.NUnit.Property]
    public bool Property_MidnightWrapAround_RespectsToleranceForward(int second, int tolerance)
    {
        // Arrange - test cases near midnight wrapping forward
        second = Math.Abs(second) % 60;
        tolerance = Math.Abs(tolerance) % 3; // 0, 1, or 2

        if (tolerance == 0)
        {
            // With 0 tolerance, 23:59 -> 00:00 should fail
            var currentTime = new DateTime(2025, 11, 15, 23, 59, second);
            var timeProvider = new TestTimeProvider(currentTime);
            var validator = new TimeValidator(0, timeProvider);
            var result = validator.ValidateTime("0000");
            return !result.IsValid; // Should be invalid
        }
        else
        {
            // With tolerance >= 1, 23:59 -> 00:00 should be valid (1 minute ahead)
            var currentTime = new DateTime(2025, 11, 15, 23, 59, second);
            var timeProvider = new TestTimeProvider(currentTime);
            var validator = new TimeValidator(tolerance, timeProvider);
            var result = validator.ValidateTime("0000");
            return result.IsValid; // Should be valid
        }
    }

    [FsCheck.NUnit.Property]
    public bool Property_MidnightWrapAround_RespectsToleranceBackward(int second, int tolerance)
    {
        // Arrange - test cases near midnight wrapping backward
        second = Math.Abs(second) % 60;
        tolerance = Math.Abs(tolerance) % 3; // 0, 1, or 2

        if (tolerance == 0)
        {
            // With 0 tolerance, 00:00 -> 23:59 should fail
            var currentTime = new DateTime(2025, 11, 15, 0, 0, second);
            var timeProvider = new TestTimeProvider(currentTime);
            var validator = new TimeValidator(0, timeProvider);
            var result = validator.ValidateTime("2359");
            return !result.IsValid; // Should be invalid
        }
        else
        {
            // With tolerance >= 1, 00:00 -> 23:59 should be valid (1 minute before)
            var currentTime = new DateTime(2025, 11, 15, 0, 0, second);
            var timeProvider = new TestTimeProvider(currentTime);
            var validator = new TimeValidator(tolerance, timeProvider);
            var result = validator.ValidateTime("2359");
            return result.IsValid; // Should be valid
        }
    }

    [FsCheck.NUnit.Property]
    public bool Property_InvalidInput_ReturnsFailure(string input)
    {
        // Arrange
        var validator = new TimeValidator();

        // Filter out valid 4-digit inputs
        if (!string.IsNullOrWhiteSpace(input) && 
            input.Length == 4 && 
            int.TryParse(input, out int value))
        {
            int hour = value / 100;
            int minute = value % 100;
            
            if (hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59)
            {
                // This is valid input, skip this test case
                return true;
            }
        }

        // Act
        var result = validator.ValidateTime(input);

        // Assert - invalid input should always return failure
        return !result.IsValid;
    }

    [FsCheck.NUnit.Property]
    public bool Property_ToleranceZero_OnlyExactMatchValid(int hour, int minute, int offsetMinutes)
    {
        // Arrange
        hour = Math.Abs(hour) % 24;
        minute = Math.Abs(minute) % 60;
        offsetMinutes = (Math.Abs(offsetMinutes) % 10) - 5; // -5 to +5 minutes

        var currentTime = new DateTime(2025, 11, 15, hour, minute, 30);
        var timeProvider = new TestTimeProvider(currentTime);
        var validator = new TimeValidator(0, timeProvider); // Zero tolerance

        var inputTime = currentTime.AddMinutes(offsetMinutes);
        var input = $"{inputTime.Hour:D2}{inputTime.Minute:D2}";

        // Act
        var result = validator.ValidateTime(input);

        // Assert - only exact match (offset == 0) should be valid
        return offsetMinutes == 0 ? result.IsValid : !result.IsValid;
    }
}
