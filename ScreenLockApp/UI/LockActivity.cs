using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using Google.Android.Material.Button;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using ScreenLockApp.Data;

namespace ScreenLockApp.UI;

/// <summary>
/// Fullscreen lock overlay activity that requires time-based authentication.
/// Implements immersive fullscreen mode with security flags.
/// </summary>
[Activity(
    Label = "@string/lock_screen_title",
    Theme = "@style/LockScreenTheme",
    ExcludeFromRecents = true,
    LaunchMode = Android.Content.PM.LaunchMode.SingleTask,
    ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
public class LockActivity : AppCompatActivity
{
    private LockViewModel _viewModel = null!;
    private PreferencesProvider _prefs = null!;
    
    private TextInputEditText? _timeInput;
    private MaterialButton? _unlockButton;
    private CircularProgressIndicator? _progressIndicator;
    private TextView? _errorText;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Apply security flags
        ApplySecurityFlags();
        
        SetContentView(Resource.Layout.activity_lock);
        
        _viewModel = new LockViewModel();
        _prefs = new PreferencesProvider(this);
        
        InitializeViews();
        SetupViewModelBindings();
    }

    private void ApplySecurityFlags()
    {
        if (Window == null) return;

        // Prevent screenshots
        Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
        
        // Show when locked
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            SetShowWhenLocked(true);
            SetTurnScreenOn(true);
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
#pragma warning restore CS0618
        }
        
        // Keep screen on
        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        
        // Make fullscreen immersive
        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            Window.SetDecorFitsSystemWindows(false);
            var controller = Window.InsetsController;
            if (controller != null)
            {
                controller.Hide(WindowInsets.Type.SystemBars());
                controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
            }
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.Fullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Immersive |
                SystemUiFlags.ImmersiveSticky);
#pragma warning restore CS0618
        }
    }

    private void InitializeViews()
    {
        _timeInput = FindViewById<TextInputEditText>(Resource.Id.timeInput);
        _unlockButton = FindViewById<MaterialButton>(Resource.Id.unlockButton);
        _progressIndicator = FindViewById<CircularProgressIndicator>(Resource.Id.progressIndicator);
        _errorText = FindViewById<TextView>(Resource.Id.errorText);

        if (_unlockButton != null)
        {
            _unlockButton.Click += OnUnlockClicked;
        }

        if (_timeInput != null)
        {
            _timeInput.EditorAction += (s, e) =>
            {
                if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
                {
                    OnUnlockClicked(s, EventArgs.Empty);
                    e.Handled = true;
                }
            };
        }
    }

    private void SetupViewModelBindings()
    {
        _viewModel.PropertyChanged += (s, e) =>
        {
            RunOnUiThread(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(LockViewModel.IsValidating):
                        UpdateProgressIndicator();
                        break;
                    case nameof(LockViewModel.ErrorMessage):
                        UpdateErrorMessage();
                        break;
                }
            });
        };
    }

    private void UpdateProgressIndicator()
    {
        if (_progressIndicator != null && _unlockButton != null)
        {
            _progressIndicator.Visibility = _viewModel.IsValidating ? ViewStates.Visible : ViewStates.Gone;
            _unlockButton.Enabled = !_viewModel.IsValidating;
        }
    }

    private void UpdateErrorMessage()
    {
        if (_errorText != null)
        {
            if (!string.IsNullOrEmpty(_viewModel.ErrorMessage))
            {
                _errorText.Text = _viewModel.ErrorMessage;
                _errorText.Visibility = ViewStates.Visible;
                
                // Show hint after 3 failures
                var hint = _viewModel.GetHintMessage();
                if (!string.IsNullOrEmpty(hint))
                {
                    _errorText.Text = $"{_viewModel.ErrorMessage}\n{hint}";
                }
            }
            else
            {
                _errorText.Visibility = ViewStates.Gone;
            }
        }
    }

    private async void OnUnlockClicked(object? sender, EventArgs e)
    {
        if (_timeInput == null) return;

        _viewModel.TimeInput = _timeInput.Text ?? string.Empty;
        
        bool isValid = await _viewModel.ValidateTimeAsync();
        
        if (isValid)
        {
            // Reset failure count in preferences
            _prefs.ResetFailureCount();
            
            // Unlock successful - close activity
            Finish();
        }
        else
        {
            // Increment failure count in preferences
            _prefs.IncrementFailureCount();
            
            // Clear input for retry
            _timeInput.Text = string.Empty;
        }
    }

    public override void OnBackPressed()
    {
        // Prevent back button from closing the lock screen
        // User must enter correct time to unlock
    }

    protected override void OnPause()
    {
        base.OnPause();
        
        // Ensure lock screen stays on top
        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            var controller = Window?.InsetsController;
            if (controller != null)
            {
                controller.Hide(WindowInsets.Type.SystemBars());
            }
        }
    }
}
