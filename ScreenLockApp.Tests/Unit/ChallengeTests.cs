using ScreenLockApp.Challenges;
using ScreenLockApp.Utils;
using Xunit;

namespace ScreenLockApp.Tests.Unit;

public class ChallengeTests
{
    [Fact]
    public void TimeChallenge_ExactMatch_ValidatesSuccessfully()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new TimeChallenge(timeProvider, () => 0);
        
        var result = challenge.Validate("1430");
        
        Assert.True(result);
    }
    
    [Fact]
    public void TimeChallenge_WithinTolerance_ValidatesSuccessfully()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new TimeChallenge(timeProvider, () => 5);
        
        var result = challenge.Validate("1433");
        
        Assert.True(result);
    }
    
    [Fact]
    public void TimeChallenge_StrictMode_RequiresExactMatch()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new TimeChallenge(timeProvider, () => 0);
        
        var resultExact = challenge.Validate("1430");
        var resultOff = challenge.Validate("1431");
        
        Assert.True(resultExact);
        Assert.False(resultOff);
    }
    
    [Fact]
    public void HourChallenge_CurrentHour_ValidatesSuccessfully()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new HourChallenge(timeProvider);
        
        var result = challenge.Validate("14");
        
        Assert.True(result);
    }
    
    [Fact]
    public void HourChallenge_MidnightTransition_ValidatesSuccessfully()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 0, 0, 0));
        
        var challenge = new HourChallenge(timeProvider);
        
        var result = challenge.Validate("00");
        
        Assert.True(result);
    }
    
    [Fact]
    public void HourChallenge_InvalidHour_ReturnsFalse()
    {
        var timeProvider = new TestTimeProvider();
        timeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new HourChallenge(timeProvider);
        
        var result = challenge.Validate("24");
        
        Assert.False(result);
    }
    
    [Fact]
    public void DailyCodeChallenge_CorrectDayCode_ValidatesSuccessfully()
    {
        var timeProvider = new TestTimeProvider();
        // Sunday = 0, expect code "1234"
        timeProvider.SetTime(new DateTime(2024, 1, 7, 12, 0, 0)); // Sunday
        
        var challenge = new DailyCodeChallenge(timeProvider);
        
        var result = challenge.Validate("1234");
        
        Assert.True(result);
    }
    
    [Fact]
    public void DailyCodeChallenge_WeekBoundary_ChangesCode()
    {
        var timeProvider = new TestTimeProvider();
        
        var challenge = new DailyCodeChallenge(timeProvider);
        
        // Sunday
        timeProvider.SetTime(new DateTime(2024, 1, 7, 12, 0, 0));
        var sundayResult = challenge.Validate("1234");
        
        // Monday
        timeProvider.SetTime(new DateTime(2024, 1, 8, 12, 0, 0));
        var mondayResult = challenge.Validate("2345");
        
        Assert.True(sundayResult);
        Assert.True(mondayResult);
    }
    
    [Fact]
    public void ChallengeRegistry_RegisterAndGet_ReturnsChallenge()
    {
        var registry = new ChallengeRegistry();
        var timeProvider = new TestTimeProvider();
        var challenge = new TimeChallenge(timeProvider, () => 5);
        
        registry.Register(challenge);
        var retrieved = registry.Get("time");
        
        Assert.NotNull(retrieved);
        Assert.Equal("time", retrieved?.Id);
    }
    
    [Fact]
    public void ChallengeRegistry_Contains_ReturnsTrueForRegistered()
    {
        var registry = new ChallengeRegistry();
        var timeProvider = new TestTimeProvider();
        var challenge = new TimeChallenge(timeProvider, () => 5);
        
        registry.Register(challenge);
        
        Assert.True(registry.Contains("time"));
        Assert.False(registry.Contains("nonexistent"));
    }
}
