using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ScreenLockApp.Core;

namespace ScreenLockApp;

[Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light")]
public class MainActivity : AppCompatActivity
{
    private IPreferencesProvider? _preferencesProvider;
    private LockCoordinator? _coordinator;

    private RadioGroup? _toleranceGroup;
    private RadioButton? _toleranceStrict;
    private RadioButton? _toleranceNormal;
    private RadioButton? _toleranceRelaxed;
    private CheckBox? _gracePeriodEnabled;
    private Spinner? _gracePeriodDuration;
    private CheckBox? _lockTaskModeEnabled;
    private Button? _triggerLockButton;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        // Get services from DI
        _preferencesProvider = App.GetService<IPreferencesProvider>();
        _coordinator = App.GetService<LockCoordinator>();

        // Initialize views
        InitializeViews();

        // Load current settings
        LoadSettings();

        // Set up event handlers
        SetupEventHandlers();
    }

    private void InitializeViews()
    {
        _toleranceGroup = FindViewById<RadioGroup>(Resource.Id.toleranceGroup);
        _toleranceStrict = FindViewById<RadioButton>(Resource.Id.toleranceStrict);
        _toleranceNormal = FindViewById<RadioButton>(Resource.Id.toleranceNormal);
        _toleranceRelaxed = FindViewById<RadioButton>(Resource.Id.toleranceRelaxed);
        _gracePeriodEnabled = FindViewById<CheckBox>(Resource.Id.gracePeriodEnabled);
        _gracePeriodDuration = FindViewById<Spinner>(Resource.Id.gracePeriodDuration);
        _lockTaskModeEnabled = FindViewById<CheckBox>(Resource.Id.lockTaskModeEnabled);
        _triggerLockButton = FindViewById<Button>(Resource.Id.triggerLockButton);
    }

    private void LoadSettings()
    {
        if (_preferencesProvider == null) return;

        // Load tolerance setting
        int tolerance = _preferencesProvider.GetToleranceMinutes();
        if (_toleranceStrict != null && _toleranceNormal != null && _toleranceRelaxed != null)
        {
            switch (tolerance)
            {
                case 0:
                    _toleranceStrict.Checked = true;
                    break;
                case 1:
                    _toleranceNormal.Checked = true;
                    break;
                case 2:
                    _toleranceRelaxed.Checked = true;
                    break;
            }
        }

        // Load grace period settings
        if (_gracePeriodEnabled != null)
        {
            _gracePeriodEnabled.Checked = _preferencesProvider.GetGracePeriodEnabled();
        }

        if (_gracePeriodDuration != null)
        {
            int minutes = _preferencesProvider.GetGracePeriodMinutes();
            int position = minutes switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                5 => 3,
                _ => 2 // Default to 2 minutes
            };
            _gracePeriodDuration.SetSelection(position);
        }

        // Load lock task mode setting
        if (_lockTaskModeEnabled != null)
        {
            _lockTaskModeEnabled.Checked = _preferencesProvider.GetLockTaskModeEnabled();
        }
    }

    private void SetupEventHandlers()
    {
        if (_toleranceGroup != null)
        {
            _toleranceGroup.CheckedChange += OnToleranceChanged;
        }

        if (_gracePeriodEnabled != null)
        {
            _gracePeriodEnabled.CheckedChange += OnGracePeriodEnabledChanged;
        }

        if (_gracePeriodDuration != null)
        {
            _gracePeriodDuration.ItemSelected += OnGracePeriodDurationChanged;
        }

        if (_lockTaskModeEnabled != null)
        {
            _lockTaskModeEnabled.CheckedChange += OnLockTaskModeChanged;
        }

        if (_triggerLockButton != null)
        {
            _triggerLockButton.Click += OnTriggerLockClicked;
        }
    }

    private void OnToleranceChanged(object? sender, RadioGroup.CheckedChangeEventArgs e)
    {
        if (_preferencesProvider == null) return;

        int tolerance = 1; // Default
        if (_toleranceStrict?.Checked == true)
        {
            tolerance = 0;
        }
        else if (_toleranceNormal?.Checked == true)
        {
            tolerance = 1;
        }
        else if (_toleranceRelaxed?.Checked == true)
        {
            tolerance = 2;
        }

        _preferencesProvider.SetToleranceMinutes(tolerance);
        Toast.MakeText(this, Resource.String.settings_saved, ToastLength.Short)?.Show();
    }

    private void OnGracePeriodEnabledChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        _preferencesProvider?.SetGracePeriodEnabled(e.IsChecked);
    }

    private void OnGracePeriodDurationChanged(object? sender, AdapterView.ItemSelectedEventArgs e)
    {
        if (_preferencesProvider == null) return;

        int minutes = e.Position switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 5,
            _ => 2
        };

        _preferencesProvider.SetGracePeriodMinutes(minutes);
    }

    private void OnLockTaskModeChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        _preferencesProvider?.SetLockTaskModeEnabled(e.IsChecked);
    }

    private void OnTriggerLockClicked(object? sender, EventArgs e)
    {
        if (_preferencesProvider == null || _coordinator == null) return;

        // Trigger lock with current tolerance
        int tolerance = _preferencesProvider.GetToleranceMinutes();
        _coordinator.TriggerLock(tolerance);

        // Start lock activity
        var intent = new Intent(this, typeof(LockActivity));
        intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
        StartActivity(intent);
    }

    protected override void OnDestroy()
    {
        // Clean up event handlers
        if (_toleranceGroup != null)
        {
            _toleranceGroup.CheckedChange -= OnToleranceChanged;
        }

        if (_gracePeriodEnabled != null)
        {
            _gracePeriodEnabled.CheckedChange -= OnGracePeriodEnabledChanged;
        }

        if (_gracePeriodDuration != null)
        {
            _gracePeriodDuration.ItemSelected -= OnGracePeriodDurationChanged;
        }

        if (_lockTaskModeEnabled != null)
        {
            _lockTaskModeEnabled.CheckedChange -= OnLockTaskModeChanged;
        }

        if (_triggerLockButton != null)
        {
            _triggerLockButton.Click -= OnTriggerLockClicked;
        }

        base.OnDestroy();
    }
}
