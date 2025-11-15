# ScreenLockApp

Advanced Screen Lock Android application with time-based authentication built using Xamarin/.NET for Android.

## Features

### 1. **Time-Based Unlock System**
- Unlock the screen by entering the current device time in 24-hour format (HHMM)
- ±1 minute tolerance for user convenience
- Handles midnight wrap-around correctly (23:59 → 00:00)
- Comprehensive input validation with clear error messages

### 2. **Fullscreen Lock Overlay**
- Immersive fullscreen activity that appears over all apps
- Prevents navigation using back, recents, or home buttons
- Material Design dark theme with centered time input
- Secure mode preventing screenshots
- Progress indicators for all async operations
- Accessibility labels for screen readers

### 3. **Device Administration**
- Device admin capabilities for enhanced security
- Admin activation flow with user-friendly UI
- Graceful degradation if admin permissions are revoked
- Warning messages when attempting to disable admin

### 4. **Background Service**
- Foreground service for lock monitoring
- Persists across device reboots
- User-configurable auto-start on boot
- Settings stored in SharedPreferences

### 5. **MVVM Architecture**
- Clean separation of concerns:
  - `/Core`: TimeValidator and validation models
  - `/UI`: Activities and ViewModels
  - `/Admin`: DeviceAdminReceiver
  - `/Service`: LockService and BootReceiver
  - `/Data`: PreferencesProvider

## Project Structure

```
ScreenLockApp/
├── ScreenLockApp.Core/          # Core business logic
│   └── TimeValidator.cs         # Time validation with ±1 min tolerance
├── ScreenLockApp/               # Android application
│   ├── Admin/
│   │   └── TimeDeviceAdminReceiver.cs
│   ├── Data/
│   │   └── PreferencesProvider.cs
│   ├── Service/
│   │   ├── LockService.cs      # Foreground service
│   │   └── BootReceiver.cs     # Boot-completed receiver
│   ├── UI/
│   │   ├── MainActivity.cs     # Main settings screen
│   │   ├── LockActivity.cs     # Fullscreen lock overlay
│   │   ├── AdminActivationActivity.cs
│   │   └── LockViewModel.cs    # MVVM pattern
│   ├── Resources/
│   │   ├── layout/             # XML layouts
│   │   ├── values/             # Strings, colors, themes
│   │   ├── drawable/           # Vector drawables
│   │   └── xml/                # Device admin policies
│   └── Properties/
│       └── AndroidManifest.xml # Permissions and components
└── ScreenLockApp.Tests/         # Unit tests
    └── Core/
        └── TimeValidatorTests.cs # 20 comprehensive tests

```

## Prerequisites

- **.NET 9.0 SDK** or later
- **Android workload** for .NET
- **Android SDK** (API Level 26 or higher)
- **Java Development Kit (JDK)**

## Setup Instructions

### 1. Install .NET and Android Workload

```bash
# Install .NET SDK (if not already installed)
# Download from: https://dotnet.microsoft.com/download

# Install Android workload
dotnet workload install android
```

### 2. Clone and Build

```bash
# Clone the repository
git clone https://github.com/Nkkinsoft/ScreenLockApp.git
cd ScreenLockApp

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run unit tests
dotnet test ScreenLockApp.Tests/ScreenLockApp.Tests.csproj
```

### 3. Deploy to Device/Emulator

```bash
# Build and deploy the APK
dotnet build ScreenLockApp/ScreenLockApp.csproj -t:Install -f net9.0-android
```

Or use Visual Studio / Visual Studio Code with the .NET MAUI extension.

## Usage

### First Launch

1. **Enable Device Admin**
   - On first launch, tap "Enable Device Admin"
   - Grant device administrator permissions when prompted
   - This is required for the lock screen to function

2. **Configure Settings**
   - Toggle "Enable Lock" to activate the lock functionality
   - Enable "Auto-start on boot" if you want the service to start automatically
   - Tap "Trigger Lock Now" to test the lock screen

### Using the Lock Screen

1. When the lock screen appears, enter the current time in 24-hour format (HHMM)
   - Example: For 1:27 PM, enter `1327`
   - Example: For 9:05 AM, enter `0905`

2. The system accepts times within ±1 minute of the current time
   - If current time is 13:27, valid inputs are: 1326, 1327, 1328

3. After 3 failed attempts, a hint will appear: "Use current 24h time HHMM"

4. Enter the correct time and tap "Unlock" to dismiss the lock screen

## Security Features

- **FLAG_SECURE**: Prevents screenshots of the lock screen
- **FLAG_SHOW_WHEN_LOCKED**: Displays over the lock screen
- **FLAG_KEEP_SCREEN_ON**: Keeps the screen active
- **Immersive fullscreen**: Hides system UI bars
- **Back button disabled**: Cannot exit lock screen via back button
- **No transient data storage**: Time validation happens in-memory only

## Testing

### Unit Tests

The project includes comprehensive unit tests for the TimeValidator:

```bash
dotnet test ScreenLockApp.Tests/ScreenLockApp.Tests.csproj
```

**Test Coverage:**
- Exact time match
- ±1 minute tolerance (before and after)
- Midnight wrap-around (23:59 → 00:00)
- Invalid input handling (empty, non-numeric, wrong length)
- Invalid hour/minute ranges
- Boundary conditions

All 20 tests pass successfully.

## Android Permissions

The app requires the following permissions (declared in AndroidManifest.xml):

- `RECEIVE_BOOT_COMPLETED` - Auto-start on device boot
- `FOREGROUND_SERVICE` - Run background monitoring service
- `FOREGROUND_SERVICE_SPECIAL_USE` - Special use foreground service
- `POST_NOTIFICATIONS` - Display service notifications (Android 13+)
- `WAKE_LOCK` - Keep device awake when needed
- `DISABLE_KEYGUARD` - Show over lock screen
- `SYSTEM_ALERT_WINDOW` - Display overlay windows
- `SCHEDULE_EXACT_ALARM` - Precise alarm scheduling (Android 13+)

## Technical Details

### Time Validation Algorithm

The `TimeValidator` class implements a robust time validation algorithm:

1. Validates input format (4 digits, numeric)
2. Extracts hours (00-23) and minutes (00-59)
3. Compares input time with current system time
4. Calculates difference in minutes, handling midnight wrap-around
5. Returns success if difference is ≤ 1 minute

### Midnight Wrap-Around Handling

Special logic handles midnight transitions:
- If difference > 12 hours, assume forward wrap (23:59 → 00:00)
- If difference < -12 hours, assume backward wrap (00:00 → 23:59)
- Ensures ±1 minute tolerance works across midnight boundary

### Material Design Theme

- Dark theme with Material Components
- Primary color: `#BB86FC` (purple)
- Secondary color: `#03DAC6` (teal)
- Background: `#121212` (dark)
- Follows Material Design 3 guidelines

## Known Limitations

- Lock screen can be bypassed by rebooting the device (unless auto-start is enabled)
- Device admin can be disabled through Settings > Security
- Requires user to grant device admin permissions manually

## Future Enhancements

- Biometric authentication option
- Custom lock intervals and schedules
- PIN backup authentication
- Geofencing-based lock triggers
- Enhanced statistics and logging

## License

This project is provided as-is for educational and demonstration purposes.

## Contributing

Contributions are welcome! Please ensure:
- All unit tests pass
- New features include tests
- Code follows existing style conventions
- Documentation is updated

## Support

For issues, questions, or feature requests, please open an issue on GitHub.

