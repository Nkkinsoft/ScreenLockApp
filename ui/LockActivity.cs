using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ScreenLockApp.challenges;
using ScreenLockApp.data;

namespace ScreenLockApp.ui;

[Activity(Label = "Lock Screen", Theme = "@style/LockTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
public class LockActivity : AppCompatActivity
{
    private LockViewModel? _viewModel;
    private ChallengeRegistry? _registry;
    private PreferencesProvider? _prefs;

    private TextView? _tvChallengeName;
    private TextView? _tvHint;
    private TextView? _tvInput;
    private TextView? _tvError;
    private GridLayout? _keypad;
    private Button? _btnSubmit;
    private Button? _btnClear;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Make fullscreen and prevent going back
        Window?.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        Window?.AddFlags(WindowManagerFlags.ShowWhenLocked);
        Window?.AddFlags(WindowManagerFlags.DismissKeyguard);
        Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
        Window?.AddFlags(WindowManagerFlags.TurnScreenOn);

        SetContentView(Resource.Layout.activity_lock);

        _prefs = new PreferencesProvider(this);
        _registry = new ChallengeRegistry(this);
        _viewModel = new LockViewModel();

        InitializeViews();
        LoadActiveChallenge();
        SetupKeypad();
    }

    private void InitializeViews()
    {
        _tvChallengeName = FindViewById<TextView>(Resource.Id.tv_challenge_name);
        _tvHint = FindViewById<TextView>(Resource.Id.tv_hint);
        _tvInput = FindViewById<TextView>(Resource.Id.tv_input);
        _tvError = FindViewById<TextView>(Resource.Id.tv_error);
        _keypad = FindViewById<GridLayout>(Resource.Id.keypad);
        _btnSubmit = FindViewById<Button>(Resource.Id.btn_submit);
        _btnClear = FindViewById<Button>(Resource.Id.btn_clear);

        if (_btnSubmit != null)
            _btnSubmit.Click += OnSubmitClicked;

        if (_btnClear != null)
            _btnClear.Click += OnClearClicked;

        // Bind ViewModel to UI
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += (s, e) =>
            {
                RunOnUiThread(() =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(LockViewModel.Input):
                            if (_tvInput != null)
                                _tvInput.Text = _viewModel.Input;
                            break;
                        case nameof(LockViewModel.ErrorMessage):
                            if (_tvError != null)
                            {
                                _tvError.Text = _viewModel.ErrorMessage;
                                _tvError.Visibility = string.IsNullOrEmpty(_viewModel.ErrorMessage) 
                                    ? ViewStates.Gone 
                                    : ViewStates.Visible;
                            }
                            break;
                        case nameof(LockViewModel.HintText):
                            if (_tvHint != null)
                                _tvHint.Text = _viewModel.HintText;
                            break;
                        case nameof(LockViewModel.Challenge):
                            if (_tvChallengeName != null && _viewModel.Challenge != null)
                                _tvChallengeName.Text = _viewModel.Challenge.DisplayName;
                            break;
                    }
                });
            };
        }
    }

    private void LoadActiveChallenge()
    {
        if (_registry != null && _viewModel != null)
        {
            var challenge = _registry.GetActiveChallenge();
            _viewModel.SwitchChallenge(challenge);
        }
    }

    private void SetupKeypad()
    {
        if (_keypad == null) return;

        // Create number buttons 0-9
        for (int i = 0; i <= 9; i++)
        {
            int digit = i;
            var button = FindViewById<Button>(GetKeypadButtonId(digit));
            if (button != null)
            {
                button.Click += (s, e) => OnDigitClicked(digit.ToString());
            }
        }

        // Backspace button
        var btnBackspace = FindViewById<Button>(Resource.Id.btn_backspace);
        if (btnBackspace != null)
        {
            btnBackspace.Click += (s, e) => OnBackspaceClicked();
        }
    }

    private int GetKeypadButtonId(int digit)
    {
        return digit switch
        {
            0 => Resource.Id.btn_0,
            1 => Resource.Id.btn_1,
            2 => Resource.Id.btn_2,
            3 => Resource.Id.btn_3,
            4 => Resource.Id.btn_4,
            5 => Resource.Id.btn_5,
            6 => Resource.Id.btn_6,
            7 => Resource.Id.btn_7,
            8 => Resource.Id.btn_8,
            9 => Resource.Id.btn_9,
            _ => 0
        };
    }

    private void OnDigitClicked(string digit)
    {
        _viewModel?.AppendDigit(digit);
        ProvideHapticFeedback();
    }

    private void OnBackspaceClicked()
    {
        _viewModel?.DeleteLastDigit();
        ProvideHapticFeedback();
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        _viewModel?.ClearInput();
        ProvideHapticFeedback();
    }

    private void OnSubmitClicked(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        var result = _viewModel.ValidateInput();
        
        if (result.IsValid)
        {
            // Unlock successful
            ProvideHapticFeedback();
            Finish();
        }
        else
        {
            // Show error
            _viewModel.ErrorMessage = result.ErrorMessage ?? "Invalid input";
            ProvideHapticFeedback(true);
        }
    }

    private void ProvideHapticFeedback(bool isError = false)
    {
        if (_prefs?.GetEnableHaptics() == true)
        {
            var vibrator = GetSystemService(VibratorService) as Android.OS.Vibrator;
            if (vibrator != null && vibrator.HasVibrator)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var effect = isError 
                        ? VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude)
                        : VibrationEffect.CreateOneShot(50, VibrationEffect.DefaultAmplitude);
                    vibrator.Vibrate(effect);
                }
                else
                {
#pragma warning disable CS0618
                    vibrator.Vibrate(isError ? 200 : 50);
#pragma warning restore CS0618
                }
            }
        }
    }

    public override void OnBackPressed()
    {
        // Prevent going back
    }
}
