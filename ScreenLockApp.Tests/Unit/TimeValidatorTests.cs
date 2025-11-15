using ScreenLockApp.Utils;
using Xunit;

namespace ScreenLockApp.Tests.Unit;

public class TimeValidatorTests
{
    [Fact]
    public void IsWithinTolerance_ExactMatch_ReturnsTrue()
    {
        var currentTime = new DateTime(2024, 1, 1, 14, 30, 0);
        var inputTime = 1430;
        
        var result = TimeValidator.IsWithinTolerance(currentTime, inputTime, 0);
        
        Assert.True(result);
    }
    
    [Fact]
    public void IsWithinTolerance_WithinTolerance_ReturnsTrue()
    {
        var currentTime = new DateTime(2024, 1, 1, 14, 30, 0);
        var inputTime = 1433; // 3 minutes ahead
        
        var result = TimeValidator.IsWithinTolerance(currentTime, inputTime, 5);
        
        Assert.True(result);
    }
    
    [Fact]
    public void IsWithinTolerance_OutsideTolerance_ReturnsFalse()
    {
        var currentTime = new DateTime(2024, 1, 1, 14, 30, 0);
        var inputTime = 1440; // 10 minutes ahead
        
        var result = TimeValidator.IsWithinTolerance(currentTime, inputTime, 5);
        
        Assert.False(result);
    }
    
    [Fact]
    public void IsWithinTolerance_InvalidInput_ReturnsFalse()
    {
        var currentTime = new DateTime(2024, 1, 1, 14, 30, 0);
        var inputTime = 2500; // Invalid time
        
        var result = TimeValidator.IsWithinTolerance(currentTime, inputTime, 5);
        
        Assert.False(result);
    }
    
    [Fact]
    public void IsWithinTolerance_NegativeInput_ReturnsFalse()
    {
        var currentTime = new DateTime(2024, 1, 1, 14, 30, 0);
        var inputTime = -100;
        
        var result = TimeValidator.IsWithinTolerance(currentTime, inputTime, 5);
        
        Assert.False(result);
    }
    
    [Fact]
    public void ToHHMM_ConvertsCorrectly()
    {
        var dateTime = new DateTime(2024, 1, 1, 14, 30, 0);
        
        var result = TimeValidator.ToHHMM(dateTime);
        
        Assert.Equal(1430, result);
    }
    
    [Fact]
    public void ToHHMM_MidnightTransition_ConvertsCorrectly()
    {
        var dateTime = new DateTime(2024, 1, 1, 0, 0, 0);
        
        var result = TimeValidator.ToHHMM(dateTime);
        
        Assert.Equal(0, result);
    }
}
