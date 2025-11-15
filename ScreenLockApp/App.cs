using Android.App;
using Android.Runtime;
using Microsoft.Extensions.DependencyInjection;
using ScreenLockApp.Core;

namespace ScreenLockApp;

[Application]
public class App : Application
{
    private static ServiceProvider? _serviceProvider;

    public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        // Set up dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register singletons
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddSingleton<LockCoordinator>();
        services.AddSingleton<IPreferencesProvider>(provider => new PreferencesProvider(this));

        // Register transients - new instance each time
        services.AddTransient<TimeValidator>(provider =>
        {
            var prefsProvider = provider.GetRequiredService<IPreferencesProvider>();
            var timeProvider = provider.GetRequiredService<ITimeProvider>();
            int tolerance = prefsProvider.GetToleranceMinutes();
            return new TimeValidator(tolerance, timeProvider);
        });

        services.AddTransient<LockViewModel>(provider =>
        {
            var validator = provider.GetRequiredService<TimeValidator>();
            var coordinator = provider.GetRequiredService<LockCoordinator>();
            return new LockViewModel(validator, coordinator);
        });
    }

    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("ServiceProvider not initialized");
        }

        return _serviceProvider.GetRequiredService<T>();
    }

    public static T? TryGetService<T>() where T : class
    {
        return _serviceProvider?.GetService<T>();
    }
}
