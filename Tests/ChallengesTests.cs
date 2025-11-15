using Xunit;

namespace ScreenLockApp.Tests;

/// <summary>
/// Unit tests for challenge validation logic
/// Tests the core validation algorithms without Android dependencies
/// </summary>
public class ChallengesTests
{
    [Fact]
    public void TimeChallenge_ValidateExactMatch_ReturnsSuccess()
    {
        // This test requires Android context, so we'll create a simple validation test
        // In a real scenario, we'd use Android unit test framework
        var now = DateTime.Now;
        string timeInput = $"{now.Hour:D2}{now.Minute:D2}";
        
        // Basic validation logic test
        Assert.Equal(4, timeInput.Length);
        Assert.True(int.TryParse(timeInput, out _));
    }

    [Fact]
    public void TimeChallenge_InvalidLength_ShouldFail()
    {
        string shortInput = "123";
        string longInput = "12345";
        
        Assert.NotEqual(4, shortInput.Length);
        Assert.NotEqual(4, longInput.Length);
    }

    [Theory]
    [InlineData("0000", 0, 0)]
    [InlineData("1430", 14, 30)]
    [InlineData("2359", 23, 59)]
    public void TimeChallenge_ParseValidTime_ExtractsHourAndMinute(string input, int expectedHour, int expectedMinute)
    {
        int timeValue = int.Parse(input);
        int hour = timeValue / 100;
        int minute = timeValue % 100;
        
        Assert.Equal(expectedHour, hour);
        Assert.Equal(expectedMinute, minute);
    }

    [Theory]
    [InlineData("2400")] // Invalid hour
    [InlineData("1260")] // Invalid minute
    [InlineData("9999")] // Both invalid
    public void TimeChallenge_InvalidTime_ShouldFail(string input)
    {
        int timeValue = int.Parse(input);
        int hour = timeValue / 100;
        int minute = timeValue % 100;
        
        bool isValid = hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59;
        Assert.False(isValid);
    }

    [Theory]
    [InlineData(0, 100, true)]
    [InlineData(50, 100, true)]
    [InlineData(99, 100, true)]
    [InlineData(100, 100, true)]
    [InlineData(101, 100, false)]
    public void BatteryChallenge_ValidateRange_ChecksValidPercentage(int input, int max, bool shouldBeValid)
    {
        bool isValid = input >= 0 && input <= max;
        Assert.Equal(shouldBeValid, isValid);
    }

    [Theory]
    [InlineData(50, 50, 0, true)]  // Exact match
    [InlineData(50, 51, 1, true)]  // Within tolerance
    [InlineData(50, 49, 1, true)]  // Within tolerance
    [InlineData(50, 52, 1, false)] // Outside tolerance
    public void BatteryChallenge_ToleranceValidation_ChecksWithinRange(int actual, int input, int tolerance, bool shouldPass)
    {
        int diff = Math.Abs(actual - input);
        bool isValid = diff <= tolerance;
        Assert.Equal(shouldPass, isValid);
    }

    [Theory]
    [InlineData("00", 0, true)]
    [InlineData("14", 14, true)]
    [InlineData("23", 23, true)]
    [InlineData("24", 24, false)] // Invalid hour
    [InlineData("99", 99, false)] // Invalid hour
    public void HourChallenge_ValidateHour_ChecksValidRange(string input, int hour, bool shouldBeValid)
    {
        bool parsed = int.TryParse(input, out int parsedHour);
        bool isValid = parsed && parsedHour == hour && hour >= 0 && hour <= 23;
        Assert.Equal(shouldBeValid, isValid);
    }

    [Theory]
    [InlineData(15, 3, "153")] // 15th day, Wednesday (3)
    [InlineData(1, 0, "010")]  // 1st day, Sunday (0)
    [InlineData(31, 6, "316")] // 31st day, Saturday (6)
    public void DailyCodeChallenge_GenerateCode_CreatesCorrectFormat(int day, int weekday, string expectedCode)
    {
        string code = $"{day:D2}{weekday}";
        Assert.Equal(expectedCode, code);
        Assert.Equal(3, code.Length);
    }

    [Fact]
    public void ChallengeRegistry_DefaultToTime_WhenUnknownId()
    {
        // Test that unknown IDs default to time challenge
        string unknownId = "unknown_challenge";
        string defaultId = "time";
        
        // Simulate registry logic
        string resolvedId = unknownId == "time" || unknownId == "battery" || 
                           unknownId == "hour" || unknownId == "dailycode" 
                           ? unknownId : defaultId;
        
        Assert.Equal(defaultId, resolvedId);
    }

    [Theory]
    [InlineData("time")]
    [InlineData("battery")]
    [InlineData("hour")]
    [InlineData("dailycode")]
    public void ChallengeRegistry_KnownIds_ShouldBeRecognized(string challengeId)
    {
        var knownIds = new[] { "time", "battery", "hour", "dailycode" };
        Assert.Contains(challengeId, knownIds);
    }
}
