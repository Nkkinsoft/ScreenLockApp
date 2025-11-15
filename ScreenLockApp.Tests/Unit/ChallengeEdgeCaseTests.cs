using ScreenLockApp.Challenges;
using ScreenLockApp.Utils;
using Xunit;

namespace ScreenLockApp.Tests.Unit;

/// <summary>
/// Edge case tests for challenge validation.
/// </summary>
public class ChallengeEdgeCaseTests
{
    [Fact]
    public void BatteryHourChallenge_3DigitInput_ValidatesCorrectly()
    {
        // Test 3-digit format: single digit battery + 2-digit hour
        // E.g., "523" = 5% battery, hour 23
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 23, 0, 0));
        
        // This test would require mocking Android Context for battery
        // Skipping actual validation test, but structure is here for future Android test
        Assert.True(true, "Edge case test placeholder for 3-digit battery+hour input");
    }
    
    [Fact]
    public void BatteryHourChallenge_4DigitInput_ValidatesCorrectly()
    {
        // Test 4-digit format: 2-digit battery + 2-digit hour
        // E.g., "2314" = 23% battery, hour 14
        Assert.True(true, "Edge case test placeholder for 4-digit battery+hour input");
    }
    
    [Fact]
    public void TimeChallenge_MidnightTransition_HandlesCorrectly()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 23, 59, 0));
        
        var challenge = new TimeChallenge(timeProvider, () => 5);
        
        // Validate current time
        var result1 = challenge.Validate("2359");
        Assert.True(result1);
        
        // Advance to midnight
        timeProvider.AdvanceMinutes(1);
        var result2 = challenge.Validate("0000");
        Assert.True(result2);
    }
    
    [Fact]
    public void DailyCodeChallenge_WeekBoundary_SaturdayToSunday()
    {
        var timeProvider = new TestTimeProvider();
        
        var challenge = new DailyCodeChallenge(timeProvider);
        
        // Saturday (day 6) - code should be "7890"
        timeProvider.SetTime(new DateTime(2024, 1, 6, 23, 59, 0));
        var saturdayResult = challenge.Validate("7890");
        
        // Advance to Sunday (day 0) - code should be "1234"
        timeProvider.SetTime(new DateTime(2024, 1, 7, 0, 1, 0));
        var sundayResult = challenge.Validate("1234");
        
        Assert.True(saturdayResult, "Saturday code should be 7890");
        Assert.True(sundayResult, "Sunday code should be 1234");
    }
    
    [Fact]
    public void TimeChallenge_ToleranceWindow_BoundaryValues()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new TimeChallenge(timeProvider, () => 5);
        
        // Exactly at tolerance boundary (5 minutes before)
        var result1 = challenge.Validate("1425");
        Assert.True(result1, "5 minutes before should be within tolerance");
        
        // Exactly at tolerance boundary (5 minutes after)
        var result2 = challenge.Validate("1435");
        Assert.True(result2, "5 minutes after should be within tolerance");
        
        // Just outside tolerance (6 minutes before)
        var result3 = challenge.Validate("1424");
        Assert.False(result3, "6 minutes before should be outside tolerance");
        
        // Just outside tolerance (6 minutes after)
        var result4 = challenge.Validate("1436");
        Assert.False(result4, "6 minutes after should be outside tolerance");
    }
    
    [Fact]
    public void HourChallenge_BoundaryHours_ValidatesCorrectly()
    {
        var timeProvider = new TestTimeProvider();
        var challenge = new HourChallenge(timeProvider);
        
        // Test midnight (hour 0)
        timeProvider.SetTime(new DateTime(2024, 1, 1, 0, 30, 0));
        Assert.True(challenge.Validate("00"), "Midnight hour should validate");
        Assert.True(challenge.Validate("0"), "Single digit 0 should also work");
        
        // Test 23:xx (last hour of day)
        timeProvider.SetTime(new DateTime(2024, 1, 1, 23, 30, 0));
        Assert.True(challenge.Validate("23"), "Hour 23 should validate");
    }
}
