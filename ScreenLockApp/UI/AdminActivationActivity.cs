using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using Google.Android.Material.ProgressIndicator;
using ScreenLockApp.Admin;

namespace ScreenLockApp.UI;

/// <summary>
/// Activity for requesting device administrator activation.
/// </summary>
[Activity(
    Label = "@string/admin_activation_title",
    Theme = "@style/AppTheme")]
public class AdminActivationActivity : AppCompatActivity
{
    private const int RequestCodeEnableAdmin = 1001;
    
    private MaterialButton? _activateButton;
    private MaterialButton? _cancelButton;
    private CircularProgressIndicator? _progressIndicator;
    
    private DevicePolicyManager? _devicePolicyManager;
    private ComponentName? _adminComponent;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_admin_activation);
        
        InitializeDeviceAdmin();
        InitializeViews();
    }

    private void InitializeDeviceAdmin()
    {
        _devicePolicyManager = (DevicePolicyManager?)GetSystemService(DevicePolicyService);
        _adminComponent = new ComponentName(this, Java.Lang.Class.FromType(typeof(TimeDeviceAdminReceiver)));
    }

    private void InitializeViews()
    {
        _activateButton = FindViewById<MaterialButton>(Resource.Id.activateButton);
        _cancelButton = FindViewById<MaterialButton>(Resource.Id.cancelButton);
        _progressIndicator = FindViewById<CircularProgressIndicator>(Resource.Id.adminProgressIndicator);

        if (_activateButton != null)
        {
            _activateButton.Click += OnActivateClicked;
        }

        if (_cancelButton != null)
        {
            _cancelButton.Click += (s, e) => Finish();
        }

        // Check if already activated
        if (IsAdminActive())
        {
            Toast.MakeText(this, Resource.String.admin_activation_success, ToastLength.Short)?.Show();
            SetResult(Result.Ok);
            Finish();
        }
    }

    private void OnActivateClicked(object? sender, EventArgs e)
    {
        ShowProgress(true);
        RequestAdminActivation();
    }

    private void RequestAdminActivation()
    {
        if (_devicePolicyManager == null || _adminComponent == null)
        {
            ShowError();
            return;
        }

        try
        {
            var intent = new Intent(DevicePolicyManager.ActionAddDeviceAdmin);
            intent.PutExtra(DevicePolicyManager.ExtraDeviceAdmin, _adminComponent);
            intent.PutExtra(
                DevicePolicyManager.ExtraAddExplanation,
                GetString(Resource.String.admin_activation_message));
            
            StartActivityForResult(intent, RequestCodeEnableAdmin);
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("ScreenLockApp", $"Error requesting admin activation: {ex.Message}");
            ShowError();
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        
        ShowProgress(false);

        if (requestCode == RequestCodeEnableAdmin)
        {
            if (resultCode == Result.Ok)
            {
                Toast.MakeText(this, Resource.String.admin_activation_success, ToastLength.Short)?.Show();
                SetResult(Result.Ok);
                Finish();
            }
            else
            {
                Toast.MakeText(this, Resource.String.admin_activation_failed, ToastLength.Short)?.Show();
                SetResult(Result.Canceled);
            }
        }
    }

    private bool IsAdminActive()
    {
        if (_devicePolicyManager == null || _adminComponent == null)
            return false;
        
        return _devicePolicyManager.IsAdminActive(_adminComponent);
    }

    private void ShowProgress(bool show)
    {
        RunOnUiThread(() =>
        {
            if (_progressIndicator != null)
            {
                _progressIndicator.Visibility = show ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
            }
            
            if (_activateButton != null)
            {
                _activateButton.Enabled = !show;
            }
        });
    }

    private void ShowError()
    {
        ShowProgress(false);
        Toast.MakeText(this, Resource.String.admin_activation_failed, ToastLength.Long)?.Show();
    }
}
