using Android.Content;
using ScreenLockApp.Core.Challenges;
using ScreenLockApp.Data;
using ScreenLockApp.Utils;

namespace ScreenLockApp.Challenges;

/// <summary>
/// Factory for creating challenge instances with Android context.
/// </summary>
public class ChallengeFactory
{
    private readonly Context _context;
    private readonly PreferencesProvider _prefs;
    private readonly ChallengeRegistry _registry;

    public ChallengeFactory(Context context, PreferencesProvider prefs)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _prefs = prefs ?? throw new ArgumentNullException(nameof(prefs));
        _registry = new ChallengeRegistry();

        RegisterChallenges();
    }

    private void RegisterChallenges()
    {
        // Register Time challenge
        _registry.RegisterChallenge("time", () => new TimeChallenge());

        // Register Battery + Hour challenge
        _registry.RegisterChallenge("battery_hour", () =>
        {
            return new BatteryHourChallenge(
                getBatteryPercent: () => BatteryHelper.GetBatteryPercent(_context),
                batteryTolerance: _prefs.BatteryTolerance,
                strictMode: _prefs.StrictMode
            );
        });
    }

    /// <summary>
    /// Gets the currently selected challenge.
    /// </summary>
    public IUnlockChallenge GetCurrentChallenge()
    {
        var challengeId = _prefs.SelectedChallenge;
        var challenge = _registry.GetChallenge(challengeId);
        
        // Fallback to time challenge if selected challenge is not available
        return challenge ?? new TimeChallenge();
    }

    /// <summary>
    /// Gets a challenge by ID.
    /// </summary>
    public IUnlockChallenge? GetChallenge(string challengeId)
    {
        return _registry.GetChallenge(challengeId);
    }

    /// <summary>
    /// Gets all available challenge IDs.
    /// </summary>
    public IEnumerable<string> GetAvailableChallengeIds()
    {
        return _registry.GetAllChallengeIds();
    }
}
