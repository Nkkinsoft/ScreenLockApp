using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using ScreenLockApp.Challenges;
using ScreenLockApp.Themes;
using ScreenLockApp.Utils;
using ScreenLockApp.ViewModels;

namespace ScreenLockApp.Activities;

[Activity(Label = "Lock Screen", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
public class LockActivity : AppCompatActivity
{
    private LockViewModel? _viewModel;
    private TextView? _hintText;
    private TextView? _inputDisplay;
    private TextView? _errorText;
    private Button? _btnUnlock;
    private Button? _btnClear;
    private readonly Button?[] _digitButtons = new Button?[10];
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Apply theme before setContentView
        ThemeManager.ApplyTheme(this);
        
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_lock);
        
        // Make fullscreen and prevent dismissal
        Window?.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        Window?.AddFlags(WindowManagerFlags.ShowWhenLocked);
        Window?.AddFlags(WindowManagerFlags.DismissKeyguard);
        
        InitializeViews();
        InitializeViewModel();
        UpdateUI();
    }
    
    private void InitializeViews()
    {
        _hintText = FindViewById<TextView>(Resource.Id.hintText);
        _inputDisplay = FindViewById<TextView>(Resource.Id.inputDisplay);
        _errorText = FindViewById<TextView>(Resource.Id.errorText);
        _btnUnlock = FindViewById<Button>(Resource.Id.btnUnlock);
        _btnClear = FindViewById<Button>(Resource.Id.btnClear);
        
        // Set error text as live region for accessibility
        if (_errorText != null)
        {
            _errorText.AccessibilityLiveRegion = AccessibilityLiveRegion.Polite;
        }
        
        // Initialize digit buttons
        for (int i = 0; i < 10; i++)
        {
            int resId = Resources?.GetIdentifier($"btn{i}", "id", PackageName) ?? 0;
            _digitButtons[i] = FindViewById<Button>(resId);
            if (_digitButtons[i] != null)
            {
                int digit = i;
                _digitButtons[i]!.Click += (s, e) => OnDigitClick(digit);
                
                // Set content description for accessibility
                int descResId = Resources?.GetIdentifier($"content_desc_digit_{i}", "string", PackageName) ?? 0;
                if (descResId != 0)
                {
                    _digitButtons[i]!.ContentDescription = GetString(descResId);
                }
            }
        }
        
        if (_btnUnlock != null)
        {
            _btnUnlock.Click += BtnUnlock_Click;
            _btnUnlock.ContentDescription = GetString(Resource.String.content_desc_unlock);
        }
        
        if (_btnClear != null)
        {
            _btnClear.Click += BtnClear_Click;
            _btnClear.ContentDescription = GetString(Resource.String.content_desc_clear);
        }
    }
    
    private void InitializeViewModel()
    {
        var registry = new ChallengeRegistry();
        var timeProvider = new SystemTimeProvider();
        
        var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
        
        Func<int> getToleranceMinutes = () =>
        {
            var strictMode = prefs?.GetBoolean("strict_mode", false) ?? false;
            if (strictMode) return 0;
            return prefs?.GetInt("tolerance_minutes", 5) ?? 5;
        };
        
        Func<int> getBatteryTolerance = () =>
        {
            var strictMode = prefs?.GetBoolean("strict_mode", false) ?? false;
            if (strictMode) return 0;
            return prefs?.GetInt("battery_tolerance", 5) ?? 5;
        };
        
        // Register all challenges
        registry.Register(new TimeChallenge(timeProvider, getToleranceMinutes));
        registry.Register(new BatteryChallenge(this, getBatteryTolerance));
        registry.Register(new HourChallenge(timeProvider));
        registry.Register(new DailyCodeChallenge(timeProvider));
        registry.Register(new BatteryHourChallenge(this, timeProvider, getBatteryTolerance));
        
        _viewModel = new LockViewModel(this, registry, timeProvider);
        _viewModel.ChallengeChanged += ViewModel_ChallengeChanged;
    }
    
    private void ViewModel_ChallengeChanged(object sender, ChallengeChangedEventArgs e)
    {
        UpdateUI();
        
        // Announce challenge change for accessibility
        if (_hintText != null)
        {
            var announcement = string.Format(
                GetString(Resource.String.announce_challenge_changed) ?? "{0}",
                e.NewChallenge?.DisplayName ?? ""
            );
            _hintText.AnnounceForAccessibility(announcement);
        }
    }
    
    private void OnDigitClick(int digit)
    {
        _viewModel?.AppendDigit((char)('0' + digit));
        UpdateUI();
        PerformHapticFeedback();
    }
    
    private void BtnClear_Click(object? sender, EventArgs e)
    {
        _viewModel?.ClearBuffer();
        UpdateUI();
        ClearError();
        PerformHapticFeedback();
    }
    
    private void BtnUnlock_Click(object? sender, EventArgs e)
    {
        if (_viewModel?.ValidateInput() == true)
        {
            Finish();
        }
        else
        {
            ShowError(GetString(Resource.String.error_validation_failed) ?? "Validation failed");
            _viewModel?.ClearBuffer();
            UpdateUI();
            
            // Announce error for accessibility
            if (_errorText != null)
            {
                _errorText.AnnounceForAccessibility(
                    GetString(Resource.String.announce_validation_failed) ?? "Validation failed"
                );
            }
        }
        PerformHapticFeedback();
    }
    
    private void UpdateUI()
    {
        if (_hintText != null)
        {
            _hintText.Text = _viewModel?.CurrentHint ?? "";
        }
        
        if (_inputDisplay != null)
        {
            var buffer = _viewModel?.InputBuffer ?? "";
            _inputDisplay.Text = new string('*', buffer.Length);
        }
    }
    
    private void ShowError(string message)
    {
        if (_errorText != null)
        {
            _errorText.Text = message;
            _errorText.Visibility = ViewStates.Visible;
            
            // Apply error color from theme
            var errorColor = ThemeManager.GetThemeColor(this, Resource.Attribute.colorError);
            _errorText.SetTextColor(new Android.Graphics.Color(errorColor));
        }
    }
    
    private void ClearError()
    {
        if (_errorText != null)
        {
            _errorText.Text = "";
            _errorText.Visibility = ViewStates.Gone;
        }
    }
    
    private void PerformHapticFeedback()
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
        var hapticsEnabled = prefs?.GetBoolean("enable_haptics", true) ?? true;
        
        if (hapticsEnabled)
        {
            Window?.DecorView.PerformHapticFeedback(FeedbackConstants.VirtualKey);
        }
    }
    
    public override void OnBackPressed()
    {
        // Prevent back button from dismissing lock screen
        // Do nothing
    }
}
