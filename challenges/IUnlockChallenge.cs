namespace ScreenLockApp.challenges;

/// <summary>
/// Result of challenge validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
}

/// <summary>
/// Interface for unlock challenge strategies
/// </summary>
public interface IUnlockChallenge
{
    /// <summary>
    /// Unique identifier for the challenge (e.g., "time", "battery")
    /// </summary>
    string Id { get; }

    /// <summary>
    /// User-facing display name
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Short help text describing the challenge
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Validates user input against the current challenge target
    /// </summary>
    ValidationResult Validate(string input);

    /// <summary>
    /// Returns a hint about what to enter (e.g., "Enter current time HHMM")
    /// </summary>
    string CurrentTargetHint();

    /// <summary>
    /// Expected input length (for UI optimization)
    /// </summary>
    int ExpectedInputLength { get; }
}
