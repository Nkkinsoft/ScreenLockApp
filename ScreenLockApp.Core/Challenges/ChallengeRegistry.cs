namespace ScreenLockApp.Core.Challenges;

/// <summary>
/// Registry for managing available unlock challenges.
/// </summary>
public class ChallengeRegistry
{
    private readonly Dictionary<string, Func<IUnlockChallenge>> _challengeFactories;

    public ChallengeRegistry()
    {
        _challengeFactories = new Dictionary<string, Func<IUnlockChallenge>>();
    }

    /// <summary>
    /// Registers a challenge factory.
    /// </summary>
    /// <param name="challengeId">Unique identifier for the challenge</param>
    /// <param name="factory">Factory function to create the challenge instance</param>
    public void RegisterChallenge(string challengeId, Func<IUnlockChallenge> factory)
    {
        _challengeFactories[challengeId] = factory;
    }

    /// <summary>
    /// Gets a challenge by ID.
    /// </summary>
    /// <param name="challengeId">Challenge identifier</param>
    /// <returns>Challenge instance or null if not found</returns>
    public IUnlockChallenge? GetChallenge(string challengeId)
    {
        if (_challengeFactories.TryGetValue(challengeId, out var factory))
        {
            return factory();
        }
        return null;
    }

    /// <summary>
    /// Gets all registered challenge IDs.
    /// </summary>
    /// <returns>Collection of challenge IDs</returns>
    public IEnumerable<string> GetAllChallengeIds()
    {
        return _challengeFactories.Keys;
    }

    /// <summary>
    /// Checks if a challenge is registered.
    /// </summary>
    /// <param name="challengeId">Challenge identifier</param>
    /// <returns>True if the challenge is registered</returns>
    public bool IsRegistered(string challengeId)
    {
        return _challengeFactories.ContainsKey(challengeId);
    }
}
