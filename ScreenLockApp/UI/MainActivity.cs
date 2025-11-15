using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using Google.Android.Material.SwitchMaterial;
using ScreenLockApp.Admin;
using ScreenLockApp.Data;
using ScreenLockApp.Service;

namespace ScreenLockApp.UI;

/// <summary>
/// Main application activity with settings and controls.
/// </summary>
[Activity(
    Label = "@string/app_name",
    Theme = "@style/AppTheme",
    MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    private const int RequestCodeEnableAdmin = 1001;
    
    private PreferencesProvider _prefs = null!;
    private DevicePolicyManager? _devicePolicyManager;
    private ComponentName? _adminComponent;
    
    private TextView? _adminStatusText;
    private MaterialButton? _enableAdminButton;
    private SwitchMaterial? _enableLockSwitch;
    private SwitchMaterial? _autoStartSwitch;
    private MaterialButton? _triggerLockButton;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
        
        _prefs = new PreferencesProvider(this);
        InitializeDeviceAdmin();
        InitializeViews();
        UpdateUI();
    }

    protected override void OnResume()
    {
        base.OnResume();
        UpdateUI();
    }

    private void InitializeDeviceAdmin()
    {
        _devicePolicyManager = (DevicePolicyManager?)GetSystemService(DevicePolicyService);
        _adminComponent = new ComponentName(this, Java.Lang.Class.FromType(typeof(TimeDeviceAdminReceiver)));
    }

    private void InitializeViews()
    {
        _adminStatusText = FindViewById<TextView>(Resource.Id.adminStatusText);
        _enableAdminButton = FindViewById<MaterialButton>(Resource.Id.enableAdminButton);
        _enableLockSwitch = FindViewById<SwitchMaterial>(Resource.Id.enableLockSwitch);
        _autoStartSwitch = FindViewById<SwitchMaterial>(Resource.Id.autoStartSwitch);
        _triggerLockButton = FindViewById<MaterialButton>(Resource.Id.triggerLockButton);

        if (_enableAdminButton != null)
        {
            _enableAdminButton.Click += OnEnableAdminClicked;
        }

        if (_enableLockSwitch != null)
        {
            _enableLockSwitch.CheckedChange += OnEnableLockChanged;
        }

        if (_autoStartSwitch != null)
        {
            _autoStartSwitch.CheckedChange += OnAutoStartChanged;
        }

        if (_triggerLockButton != null)
        {
            _triggerLockButton.Click += OnTriggerLockClicked;
        }
    }

    private void UpdateUI()
    {
        bool isAdminActive = IsAdminActive();
        
        // Update admin status
        if (_adminStatusText != null)
        {
            _adminStatusText.Text = isAdminActive 
                ? GetString(Resource.String.admin_enabled)
                : GetString(Resource.String.admin_disabled);
        }

        if (_enableAdminButton != null)
        {
            _enableAdminButton.Visibility = isAdminActive 
                ? Android.Views.ViewStates.Gone 
                : Android.Views.ViewStates.Visible;
        }

        // Update switches from preferences
        if (_enableLockSwitch != null)
        {
            _enableLockSwitch.CheckedChange -= OnEnableLockChanged;
            _enableLockSwitch.Checked = _prefs.IsLockEnabled;
            _enableLockSwitch.CheckedChange += OnEnableLockChanged;
        }

        if (_autoStartSwitch != null)
        {
            _autoStartSwitch.CheckedChange -= OnAutoStartChanged;
            _autoStartSwitch.Checked = _prefs.AutoStartOnBoot;
            _autoStartSwitch.CheckedChange += OnAutoStartChanged;
        }
    }

    private void OnEnableAdminClicked(object? sender, EventArgs e)
    {
        var intent = new Intent(this, typeof(AdminActivationActivity));
        StartActivityForResult(intent, RequestCodeEnableAdmin);
    }

    private void OnEnableLockChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        _prefs.IsLockEnabled = e.IsChecked;
        
        if (e.IsChecked)
        {
            StartLockService();
        }
        else
        {
            StopLockService();
        }
    }

    private void OnAutoStartChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        _prefs.AutoStartOnBoot = e.IsChecked;
    }

    private void OnTriggerLockClicked(object? sender, EventArgs e)
    {
        if (!IsAdminActive())
        {
            Toast.MakeText(this, "Please enable Device Admin first", ToastLength.Short)?.Show();
            return;
        }

        TriggerLockScreen();
    }

    private bool IsAdminActive()
    {
        if (_devicePolicyManager == null || _adminComponent == null)
            return false;
        
        return _devicePolicyManager.IsAdminActive(_adminComponent);
    }

    private void StartLockService()
    {
        var intent = new Intent(this, typeof(LockService));
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            StartForegroundService(intent);
        }
        else
        {
            StartService(intent);
        }
    }

    private void StopLockService()
    {
        var intent = new Intent(this, typeof(LockService));
        StopService(intent);
    }

    private void TriggerLockScreen()
    {
        var intent = new Intent(this, typeof(LockActivity));
        intent.AddFlags(ActivityFlags.NewTask);
        intent.AddFlags(ActivityFlags.NoHistory);
        StartActivity(intent);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        
        if (requestCode == RequestCodeEnableAdmin)
        {
            UpdateUI();
        }
    }
}
