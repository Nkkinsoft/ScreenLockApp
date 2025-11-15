using NUnit.Framework;
using ScreenLockApp.Core;

namespace ScreenLockApp.Tests.Core;

[TestFixture]
public class TimeValidatorTests
{
    private TimeValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new TimeValidator();
    }

    [Test]
    public void ValidateTime_ExactMatch_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "1327";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void ValidateTime_OneMinuteBefore_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "1326"; // -1 minute

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_OneMinuteAfter_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "1328"; // +1 minute

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_TwoMinutesBefore_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "1325"; // -2 minutes

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Time does not match current time window"));
    }

    [Test]
    public void ValidateTime_TwoMinutesAfter_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "1329"; // +2 minutes

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Time does not match current time window"));
    }

    [Test]
    public void ValidateTime_MidnightWrapForward_ReturnsSuccess()
    {
        // Arrange - current time is 23:59, input is 00:00 (1 minute ahead)
        var currentTime = new DateTime(2025, 11, 15, 23, 59, 30);
        var input = "0000";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_MidnightWrapBackward_ReturnsSuccess()
    {
        // Arrange - current time is 00:00, input is 23:59 (1 minute before)
        var currentTime = new DateTime(2025, 11, 15, 0, 0, 30);
        var input = "2359";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_MidnightExact_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 0, 0, 30);
        var input = "0000";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_EmptyInput_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Input cannot be empty"));
    }

    [Test]
    public void ValidateTime_NullInput_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        string? input = null;

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input!, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Input cannot be empty"));
    }

    [Test]
    public void ValidateTime_TooShortInput_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "132";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Input must be exactly 4 digits (HHMM)"));
    }

    [Test]
    public void ValidateTime_TooLongInput_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "13277";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Input must be exactly 4 digits (HHMM)"));
    }

    [Test]
    public void ValidateTime_NonNumericInput_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "13ab";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Input must contain only numbers"));
    }

    [Test]
    public void ValidateTime_InvalidHour_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "2527"; // Hour 25 is invalid

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Hour must be between 00 and 23"));
    }

    [Test]
    public void ValidateTime_InvalidMinute_ReturnsFailure()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 13, 27, 30);
        var input = "1360"; // Minute 60 is invalid

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Minute must be between 00 and 59"));
    }

    [Test]
    public void ValidateTime_BoundaryCase_2358to2359_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 23, 58, 30);
        var input = "2359"; // +1 minute

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_BoundaryCase_0001to0000_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 0, 1, 30);
        var input = "0000"; // -1 minute

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_AtSeconds59_StillValidatesCorrectly()
    {
        // Arrange - user starts typing at 12:59:59, submits at 13:00:05
        var currentTime = new DateTime(2025, 11, 15, 13, 0, 5);
        var input = "1259"; // They typed it when it was 12:59

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True); // Should still be valid due to tolerance
    }

    [Test]
    public void ValidateTime_WithLeadingZero_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 9, 5, 30);
        var input = "0905";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTime_EarlyMorning_ReturnsSuccess()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 1, 30, 15);
        var input = "0130";

        // Act
        var result = _validator.ValidateTimeWithCustomCurrent(input, currentTime);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }
}
