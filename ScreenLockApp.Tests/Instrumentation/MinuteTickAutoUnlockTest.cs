using ScreenLockApp.Challenges;
using ScreenLockApp.Utils;
using Xunit;

namespace ScreenLockApp.Tests.Instrumentation;

/// <summary>
/// Instrumentation test: Deterministic unlock when time advances into tolerance window.
/// Uses TestTimeProvider to inject controlled time values.
/// </summary>
public class MinuteTickAutoUnlockTest
{
    [Fact]
    public void TimeAdvancesIntoToleranceWindow_ValidationSucceeds()
    {
        // Arrange
        var testTimeProvider = new TestTimeProvider();
        testTimeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new TimeChallenge(testTimeProvider, () => 5);
        
        // User enters time slightly ahead
        var userInput = "1433"; // 14:33
        
        // Act - time hasn't advanced yet
        var resultBefore = challenge.Validate(userInput);
        
        // Advance time to match user input
        testTimeProvider.AdvanceMinutes(3);
        
        // Act - time has now advanced
        var resultAfter = challenge.Validate(userInput);
        
        // Assert
        Assert.True(resultBefore, "Should validate within 5-minute tolerance");
        Assert.True(resultAfter, "Should still validate at exact match time");
    }
    
    [Fact]
    public void StrictMode_TimeAdvances_StillRequiresExactMatch()
    {
        // Arrange
        var testTimeProvider = new TestTimeProvider();
        testTimeProvider.SetTime(new DateTime(2024, 1, 1, 14, 30, 0));
        
        var challenge = new TimeChallenge(testTimeProvider, () => 0); // Strict mode: 0 tolerance
        
        var userInput = "1430";
        
        // Act
        var resultExact = challenge.Validate(userInput);
        
        testTimeProvider.AdvanceMinutes(1);
        var resultAfterAdvance = challenge.Validate(userInput);
        
        // Assert
        Assert.True(resultExact, "Exact match should validate");
        Assert.False(resultAfterAdvance, "Should fail after time advances in strict mode");
    }
    
    [Fact(Skip = "Requires Android instrumentation runner")]
    public void RealTimeReceiver_MinuteTick_TriggersHintUpdate()
    {
        // This would test that TIME_TICK broadcast triggers UI update
        Assert.True(true, "Test would verify minute tick receiver integration");
    }
}
