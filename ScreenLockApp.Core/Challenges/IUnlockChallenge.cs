namespace ScreenLockApp.Core.Challenges;

/// <summary>
/// Interface for unlock challenges.
/// </summary>
public interface IUnlockChallenge
{
    /// <summary>
    /// Gets the unique identifier for this challenge type.
    /// </summary>
    string ChallengeId { get; }

    /// <summary>
    /// Gets the display name for this challenge.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the description of this challenge.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the required input length for this challenge.
    /// </summary>
    int RequiredLength { get; }

    /// <summary>
    /// Validates the user input against the current challenge requirements.
    /// </summary>
    /// <param name="input">User input to validate</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    ValidationResult Validate(string input);

    /// <summary>
    /// Gets a hint message to display to the user.
    /// </summary>
    /// <returns>Hint message string</returns>
    string GetHint();
}
