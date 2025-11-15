namespace ScreenLockApp.Challenges;

/// <summary>
/// Represents a challenge that must be solved to unlock the screen.
/// </summary>
public interface IUnlockChallenge
{
    /// <summary>
    /// Gets the unique identifier for this challenge.
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Gets the display name of this challenge.
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// Gets the hint text to show to the user.
    /// </summary>
    string GetHint();
    
    /// <summary>
    /// Validates the user's input against the challenge solution.
    /// </summary>
    /// <param name="userInput">The user's input code.</param>
    /// <returns>True if the input matches the solution, false otherwise.</returns>
    bool Validate(string userInput);
    
    /// <summary>
    /// Updates the challenge state if needed (e.g., time-based challenges).
    /// </summary>
    void Update();
}
