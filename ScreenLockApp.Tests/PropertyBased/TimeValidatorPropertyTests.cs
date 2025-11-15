using FsCheck;
using FsCheck.Xunit;
using ScreenLockApp.Utils;

namespace ScreenLockApp.Tests.PropertyBased;

public class TimeValidatorPropertyTests
{
    [Property]
    public Property IsWithinTolerance_ZeroTolerance_OnlyExactMatchIsValid()
    {
        return Prop.ForAll<int, int, int, int>(
            (hour, minute, inputHour, inputMinute) =>
            {
                if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
                    return true; // Skip invalid inputs
                if (inputHour < 0 || inputHour > 23 || inputMinute < 0 || inputMinute > 59)
                    return true; // Skip invalid inputs
                    
                var currentTime = new DateTime(2024, 1, 1, hour, minute, 0);
                var inputTime = inputHour * 100 + inputMinute;
                
                var result = TimeValidator.IsWithinTolerance(currentTime, inputTime, 0);
                
                var expectedResult = (hour == inputHour && minute == inputMinute);
                return result == expectedResult;
            });
    }
    
    [Property]
    public Property IsWithinTolerance_SymmetricProperty()
    {
        return Prop.ForAll<int, int>(
            (timeOffset, tolerance) =>
            {
                if (tolerance < 0 || tolerance > 100)
                    return true; // Skip invalid tolerance
                if (Math.Abs(timeOffset) > 60)
                    return true; // Keep test manageable
                    
                var currentTime = new DateTime(2024, 1, 1, 12, 0, 0);
                var inputTime1 = TimeValidator.ToHHMM(currentTime.AddMinutes(timeOffset));
                var inputTime2 = TimeValidator.ToHHMM(currentTime.AddMinutes(-timeOffset));
                
                var result1 = TimeValidator.IsWithinTolerance(currentTime, inputTime1, tolerance);
                var result2 = TimeValidator.IsWithinTolerance(currentTime, inputTime2, tolerance);
                
                return result1 == result2;
            });
    }
    
    [Property]
    public Property ToHHMM_AlwaysReturnsValidFormat()
    {
        return Prop.ForAll<int, int>(
            (hour, minute) =>
            {
                if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
                    return true; // Skip invalid inputs
                    
                var dateTime = new DateTime(2024, 1, 1, hour, minute, 0);
                var result = TimeValidator.ToHHMM(dateTime);
                
                return result >= 0 && result <= 2359 && result / 100 == hour && result % 100 == minute;
            });
    }
}
