using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ScreenLockApp.Core;

namespace ScreenLockApp;

[Activity(Label = "@string/lock_screen_title",
    LaunchMode = LaunchMode.SingleTask,
    ExcludeFromRecents = true,
    Theme = "@style/Theme.AppCompat.NoActionBar")]
public class LockActivity : AppCompatActivity
{
    private LockViewModel? _viewModel;
    private LockCoordinator? _coordinator;
    private IPreferencesProvider? _preferencesProvider;

    private TextView? _timeDigits;
    private TextView? _hintMessage;
    private TextView? _feedbackMessage;
    private Button? _buttonSubmit;

    // Keypad buttons
    private Button? _button0, _button1, _button2, _button3, _button4;
    private Button? _button5, _button6, _button7, _button8, _button9;
    private Button? _buttonClear;

    private Vibrator? _vibrator;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Make fullscreen and prevent going back
        Window?.AddFlags(WindowManagerFlags.ShowWhenLocked |
                        WindowManagerFlags.DismissKeyguard |
                        WindowManagerFlags.KeepScreenOn |
                        WindowManagerFlags.TurnScreenOn);

        SetContentView(Resource.Layout.activity_lock);

        // Get services from DI
        _viewModel = App.GetService<LockViewModel>();
        _coordinator = App.GetService<LockCoordinator>();
        _preferencesProvider = App.GetService<IPreferencesProvider>();
        _vibrator = GetSystemService(VibratorService) as Vibrator;

        // Initialize views
        InitializeViews();

        // Set up event handlers
        SetupEventHandlers();

        // Try lock task mode if enabled
        TryEnableLockTaskMode();
    }

    private void InitializeViews()
    {
        _timeDigits = FindViewById<TextView>(Resource.Id.timeDigits);
        _hintMessage = FindViewById<TextView>(Resource.Id.hintMessage);
        _feedbackMessage = FindViewById<TextView>(Resource.Id.feedbackMessage);
        _buttonSubmit = FindViewById<Button>(Resource.Id.buttonSubmit);

        // Initialize keypad buttons
        _button0 = FindViewById<Button>(Resource.Id.button0);
        _button1 = FindViewById<Button>(Resource.Id.button1);
        _button2 = FindViewById<Button>(Resource.Id.button2);
        _button3 = FindViewById<Button>(Resource.Id.button3);
        _button4 = FindViewById<Button>(Resource.Id.button4);
        _button5 = FindViewById<Button>(Resource.Id.button5);
        _button6 = FindViewById<Button>(Resource.Id.button6);
        _button7 = FindViewById<Button>(Resource.Id.button7);
        _button8 = FindViewById<Button>(Resource.Id.button8);
        _button9 = FindViewById<Button>(Resource.Id.button9);
        _buttonClear = FindViewById<Button>(Resource.Id.buttonClear);
    }

    private void SetupEventHandlers()
    {
        if (_viewModel == null) return;

        // ViewModel events
        _viewModel.InputChanged += OnInputChanged;
        _viewModel.ValidationCompleted += OnValidationCompleted;
        _viewModel.HintRequested += OnHintRequested;

        // Keypad button clicks
        _button0?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('0')));
        _button1?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('1')));
        _button2?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('2')));
        _button3?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('3')));
        _button4?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('4')));
        _button5?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('5')));
        _button6?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('6')));
        _button7?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('7')));
        _button8?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('8')));
        _button9?.SetOnClickListener(new KeypadClickListener(() => OnDigitClicked('9')));

        _buttonClear?.SetOnClickListener(new KeypadClickListener(OnClearClicked));
        _buttonSubmit?.SetOnClickListener(new KeypadClickListener(OnSubmitClicked));

        // Coordinator events
        if (_coordinator != null)
        {
            _coordinator.LockStateChanged += OnLockStateChanged;
        }
    }

    private void OnDigitClicked(char digit)
    {
        _viewModel?.AppendDigit(digit);
        ProvideFeedback();
    }

    private void OnClearClicked()
    {
        _viewModel?.ClearInput();
        ProvideFeedback();
    }

    private void OnSubmitClicked()
    {
        _viewModel?.ValidateInput();
        ProvideFeedback();
    }

    private void OnInputChanged(object? sender, string input)
    {
        if (_timeDigits != null)
        {
            // Format as HH:MM for display (but keep raw input internally)
            if (input.Length >= 2)
            {
                string formatted = input.Length >= 4
                    ? $"{input.Substring(0, 2)}:{input.Substring(2, 2)}"
                    : $"{input.Substring(0, 2)}:{input.Substring(2).PadRight(2, '_')}";
                _timeDigits.Text = formatted;
            }
            else if (input.Length == 1)
            {
                _timeDigits.Text = $"{input}_:__";
            }
            else
            {
                _timeDigits.Text = "__:__";
            }
        }

        // Enable/disable submit button
        if (_buttonSubmit != null)
        {
            _buttonSubmit.Enabled = input.Length == 4;
        }
    }

    private void OnValidationCompleted(object? sender, ValidationResult result)
    {
        if (_feedbackMessage == null) return;

        _feedbackMessage.Visibility = ViewStates.Visible;

        if (result.IsValid)
        {
            _feedbackMessage.Text = GetString(Resource.String.validation_success);
            _feedbackMessage.SetTextColor(Android.Graphics.Color.Green);

            // Record unlock time
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _preferencesProvider?.SetLastUnlockEpochMs(now);

            // Exit lock task mode if needed
            TryStopLockTaskMode();

            // Finish activity and return to main
            Finish();
        }
        else
        {
            _feedbackMessage.Text = GetString(Resource.String.validation_failure);
            _feedbackMessage.SetTextColor(Android.Graphics.Color.Red);

            // Hide feedback after delay
            new Handler(Looper.MainLooper!).PostDelayed(() =>
            {
                if (_feedbackMessage != null)
                {
                    _feedbackMessage.Visibility = ViewStates.Gone;
                }
            }, 2000);
        }
    }

    private void OnHintRequested(object? sender, string hint)
    {
        if (_hintMessage == null) return;

        _hintMessage.Text = hint;
        _hintMessage.Visibility = ViewStates.Visible;
    }

    private void OnLockStateChanged(object? sender, bool isLocked)
    {
        if (!isLocked)
        {
            // User unlocked successfully - exit
            RunOnUiThread(() =>
            {
                TryStopLockTaskMode();
                Finish();
            });
        }
    }

    private void ProvideFeedback()
    {
        // Haptic feedback
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            _vibrator?.Vibrate(VibrationEffect.CreateOneShot(50, VibrationEffect.DefaultAmplitude));
        }
        else
        {
#pragma warning disable CS0618
            _vibrator?.Vibrate(50);
#pragma warning restore CS0618
        }
    }

    private void TryEnableLockTaskMode()
    {
        if (_preferencesProvider?.GetLockTaskModeEnabled() != true)
        {
            return;
        }

        try
        {
            var dpm = GetSystemService(DevicePolicyService) as DevicePolicyManager;
            if (dpm?.IsDeviceOwnerApp(PackageName) == true)
            {
                StartLockTask();
            }
        }
        catch (Exception)
        {
            // Silently fail if not device owner
        }
    }

    private void TryStopLockTaskMode()
    {
        try
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var activityManager = GetSystemService(ActivityService) as ActivityManager;
                if (activityManager?.LockTaskModeState != 0) // 0 = not in lock task mode
                {
                    StopLockTask();
                }
            }
        }
        catch (Exception)
        {
            // Silently fail
        }
    }

    public override void OnBackPressed()
    {
        // Prevent back button from exiting lock screen
        // Do nothing
    }

    protected override void OnDestroy()
    {
        // Clean up event handlers
        if (_viewModel != null)
        {
            _viewModel.InputChanged -= OnInputChanged;
            _viewModel.ValidationCompleted -= OnValidationCompleted;
            _viewModel.HintRequested -= OnHintRequested;
        }

        if (_coordinator != null)
        {
            _coordinator.LockStateChanged -= OnLockStateChanged;
        }

        base.OnDestroy();
    }

    // Helper class for button click listeners
    private class KeypadClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private readonly Action _action;

        public KeypadClickListener(Action action)
        {
            _action = action;
        }

        public void OnClick(View? v)
        {
            _action();
        }
    }
}
