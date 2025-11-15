namespace ScreenLockApp.Challenges;

/// <summary>
/// Registry for managing available unlock challenges.
/// </summary>
public class ChallengeRegistry
{
    private readonly Dictionary<string, IUnlockChallenge> _challenges = new();
    
    /// <summary>
    /// Registers a challenge with the registry.
    /// </summary>
    /// <param name="challenge">The challenge to register.</param>
    public void Register(IUnlockChallenge challenge)
    {
        _challenges[challenge.Id] = challenge;
    }
    
    /// <summary>
    /// Gets a challenge by its ID.
    /// </summary>
    /// <param name="id">The challenge ID.</param>
    /// <returns>The challenge, or null if not found.</returns>
    public IUnlockChallenge? Get(string id)
    {
        return _challenges.TryGetValue(id, out var challenge) ? challenge : null;
    }
    
    /// <summary>
    /// Gets all registered challenges.
    /// </summary>
    /// <returns>A collection of all challenges.</returns>
    public IEnumerable<IUnlockChallenge> GetAll()
    {
        return _challenges.Values;
    }
    
    /// <summary>
    /// Checks if a challenge with the given ID is registered.
    /// </summary>
    /// <param name="id">The challenge ID.</param>
    /// <returns>True if the challenge is registered, false otherwise.</returns>
    public bool Contains(string id)
    {
        return _challenges.ContainsKey(id);
    }
}
