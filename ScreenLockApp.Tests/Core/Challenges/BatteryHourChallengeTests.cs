using NUnit.Framework;
using ScreenLockApp.Core;
using ScreenLockApp.Core.Challenges;

namespace ScreenLockApp.Tests.Core.Challenges;

[TestFixture]
public class BatteryHourChallengeTests
{
    private int _mockBatteryPercent;

    private int GetMockBattery() => _mockBatteryPercent;

    [SetUp]
    public void Setup()
    {
        _mockBatteryPercent = 50; // Default battery level
    }

    [Test]
    public void Validate_ExactMatch_ReturnsSuccess()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);
        var input = $"{now.Hour:D2}57"; // e.g., "1357" for 13:00 with 57% battery

        // Act
        var result = challenge.Validate(input);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }

    [Test]
    public void Validate_HourMismatch_ReturnsFailure()
    {
        // Arrange
        var now = DateTime.Now;
        int wrongHour = (now.Hour + 1) % 24;
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);
        var input = $"{wrongHour:D2}57";

        // Act
        var result = challenge.Validate(input);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Hour mismatch"));
    }

    [Test]
    public void Validate_BatteryWithinTolerance_ReturnsSuccess()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery, batteryTolerance: 1);
        
        // Test -1% (within tolerance)
        var input1 = $"{now.Hour:D2}56";
        var result1 = challenge.Validate(input1);
        
        // Test +1% (within tolerance)
        var input2 = $"{now.Hour:D2}58";
        var result2 = challenge.Validate(input2);

        // Assert
        Assert.That(result1.IsValid, Is.True);
        Assert.That(result2.IsValid, Is.True);
    }

    [Test]
    public void Validate_BatteryOutsideTolerance_ReturnsFailure()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery, batteryTolerance: 1);
        
        // Test -2% (outside tolerance)
        var input1 = $"{now.Hour:D2}55";
        var result1 = challenge.Validate(input1);
        
        // Test +2% (outside tolerance)
        var input2 = $"{now.Hour:D2}59";
        var result2 = challenge.Validate(input2);

        // Assert
        Assert.That(result1.IsValid, Is.False);
        Assert.That(result1.ErrorMessage, Does.Contain("Battery percent mismatch"));
        Assert.That(result2.IsValid, Is.False);
        Assert.That(result2.ErrorMessage, Does.Contain("Battery percent mismatch"));
    }

    [Test]
    public void Validate_StrictMode_ZeroTolerance()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery, batteryTolerance: 1, strictMode: true);
        
        // Test exact match (should work)
        var input1 = $"{now.Hour:D2}57";
        var result1 = challenge.Validate(input1);
        
        // Test ±1% (should fail in strict mode)
        var input2 = $"{now.Hour:D2}56";
        var result2 = challenge.Validate(input2);
        
        var input3 = $"{now.Hour:D2}58";
        var result3 = challenge.Validate(input3);

        // Assert
        Assert.That(result1.IsValid, Is.True);
        Assert.That(result2.IsValid, Is.False);
        Assert.That(result3.IsValid, Is.False);
    }

    [Test]
    public void Validate_Battery100_CappedTo99()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = 100; // Battery at 100%
        var challenge = new BatteryHourChallenge(GetMockBattery);
        
        // Input should use 99 when battery is 100
        var input = $"{now.Hour:D2}99";

        // Act
        var result = challenge.Validate(input);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_BatteryUnavailable_ReturnsFailure()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = -1; // Unavailable battery
        var challenge = new BatteryHourChallenge(GetMockBattery);
        var input = $"{now.Hour:D2}50";

        // Act
        var result = challenge.Validate(input);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Battery status unavailable"));
    }

    [Test]
    public void Validate_EmptyInput_ReturnsFailure()
    {
        // Arrange
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var result = challenge.Validate("");

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Input cannot be empty"));
    }

    [Test]
    public void Validate_WrongLength_ReturnsFailure()
    {
        // Arrange
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var result1 = challenge.Validate("123");
        var result2 = challenge.Validate("12345");

        // Assert
        Assert.That(result1.IsValid, Is.False);
        Assert.That(result1.ErrorMessage, Does.Contain("Input must be exactly 4 digits"));
        Assert.That(result2.IsValid, Is.False);
    }

    [Test]
    public void Validate_NonNumeric_ReturnsFailure()
    {
        // Arrange
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var result = challenge.Validate("12AB");

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Input must contain only numbers"));
    }

    [Test]
    public void Validate_InvalidHour_ReturnsFailure()
    {
        // Arrange
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var result1 = challenge.Validate("2457"); // Hour 24 is invalid
        var result2 = challenge.Validate("9957"); // Hour 99 is invalid

        // Assert
        Assert.That(result1.IsValid, Is.False);
        Assert.That(result1.ErrorMessage, Does.Contain("Hour must be between 00 and 23"));
        Assert.That(result2.IsValid, Is.False);
    }

    [Test]
    public void GetHint_ReturnsCorrectFormat()
    {
        // Arrange
        var now = DateTime.Now;
        _mockBatteryPercent = 57;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var hint = challenge.GetHint();

        // Assert
        Assert.That(hint, Does.Contain("HH + PP"));
        Assert.That(hint, Does.Contain($"{now:HH}"));
        Assert.That(hint, Does.Contain("57"));
    }

    [Test]
    public void GetHint_Battery100_ShowsAs99()
    {
        // Arrange
        _mockBatteryPercent = 100;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var hint = challenge.GetHint();

        // Assert
        Assert.That(hint, Does.Contain("99"));
        Assert.That(hint, Does.Not.Contain("100"));
    }

    [Test]
    public void GetHint_BatteryUnavailable_ShowsWarning()
    {
        // Arrange
        _mockBatteryPercent = -1;
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Act
        var hint = challenge.GetHint();

        // Assert
        Assert.That(hint, Does.Contain("Battery status unavailable"));
    }

    [Test]
    public void RequiredLength_Returns4()
    {
        // Arrange
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Assert
        Assert.That(challenge.RequiredLength, Is.EqualTo(4));
    }

    [Test]
    public void ChallengeId_ReturnsBatteryHour()
    {
        // Arrange
        var challenge = new BatteryHourChallenge(GetMockBattery);

        // Assert
        Assert.That(challenge.ChallengeId, Is.EqualTo("battery_hour"));
    }
}
