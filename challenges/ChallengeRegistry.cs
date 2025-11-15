using Android.Content;

namespace ScreenLockApp.challenges;

/// <summary>
/// Registry for managing available unlock challenges
/// </summary>
public class ChallengeRegistry
{
    private readonly Context _context;
    private readonly Dictionary<string, Func<IUnlockChallenge>> _challengeFactories;

    public ChallengeRegistry(Context context)
    {
        _context = context;
        _challengeFactories = new Dictionary<string, Func<IUnlockChallenge>>();
        RegisterDefaultChallenges();
    }

    private void RegisterDefaultChallenges()
    {
        // Register all available challenges
        _challengeFactories["time"] = () => CreateTimeChallenge();
        _challengeFactories["battery"] = () => CreateBatteryChallenge();
        _challengeFactories["hour"] = () => CreateHourChallenge();
        _challengeFactories["dailycode"] = () => CreateDailyCodeChallenge();
    }

    private TimeChallenge CreateTimeChallenge()
    {
        var prefs = new data.PreferencesProvider(_context);
        int tolerance = prefs.GetTimeTolerance();
        return new TimeChallenge(_context, tolerance);
    }

    private BatteryChallenge CreateBatteryChallenge()
    {
        var prefs = new data.PreferencesProvider(_context);
        int tolerance = prefs.GetBatteryTolerance();
        return new BatteryChallenge(_context, tolerance);
    }

    private HourChallenge CreateHourChallenge()
    {
        return new HourChallenge(_context);
    }

    private DailyCodeChallenge CreateDailyCodeChallenge()
    {
        return new DailyCodeChallenge(_context);
    }

    /// <summary>
    /// Gets the active challenge based on user preference
    /// </summary>
    public IUnlockChallenge GetActiveChallenge()
    {
        var prefs = new data.PreferencesProvider(_context);
        string activeChallengeId = prefs.GetActiveChallengeId();
        return GetChallengeById(activeChallengeId);
    }

    /// <summary>
    /// Gets a challenge by its ID, defaults to time challenge if not found
    /// </summary>
    public IUnlockChallenge GetChallengeById(string challengeId)
    {
        if (_challengeFactories.TryGetValue(challengeId, out var factory))
        {
            return factory();
        }

        // Default to time challenge if unknown ID
        return CreateTimeChallenge();
    }

    /// <summary>
    /// Gets all available challenge IDs and display names
    /// </summary>
    public List<(string Id, string DisplayName, string Description)> GetAvailableChallenges()
    {
        var challenges = new List<(string, string, string)>();
        foreach (var kvp in _challengeFactories)
        {
            var challenge = kvp.Value();
            challenges.Add((challenge.Id, challenge.DisplayName, challenge.Description));
        }
        return challenges;
    }
}
