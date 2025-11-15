# ScreenLockApp

Advanced time-based screen lock Android application with configurable security features.

## Overview

ScreenLockApp is a .NET for Android (MAUI Android) application that provides time-based screen locking functionality. Users must enter the current time (in HHMM format) to unlock the screen, with configurable tolerance levels and grace periods.

## Features

### Core Functionality
- **Time-Based Authentication**: Unlock screen by entering the current time in 24-hour format (HHMM)
- **Custom Numeric Keypad**: On-screen keypad replaces system keyboard for secure input
- **Visual Feedback**: Real-time display of entered digits with masked formatting
- **Haptic Feedback**: Tactile response on button presses

### Configurable Settings

#### Tolerance Levels
Choose from three tolerance modes:
- **Strict (0 minutes)**: Exact match required - only the precise current time is valid
- **Normal (±1 minute)**: Default mode - accepts time within ±1 minute of actual time
- **Relaxed (±2 minutes)**: More forgiving - accepts time within ±2 minutes

The tolerance setting helps account for the time it takes to type the digits and slight clock variations.

#### Grace Period
After successful unlock, the app can wait before triggering the lock screen again:
- **Off**: No grace period
- **1 minute**: 60-second grace period
- **2 minutes**: 120-second grace period (default)
- **5 minutes**: 300-second grace period

This prevents immediate re-locking after unlocking.

#### Lock Task Mode (Kiosk Mode)
- Optional device owner mode for enhanced security
- Requires device owner/admin permissions
- Prevents user from exiting the lock screen through system UI
- Automatically disabled if permissions are not available

### Advanced Features

#### Time Change Detection
- Monitors system time and timezone changes via broadcast receivers
- Detects manual time adjustments while lock screen is active
- Validates entered time against current system time
- Handles midnight wrap-around correctly (23:59 → 00:00 and vice versa)

#### Progressive Hints
- After 3 failed attempts, displays helpful hints
- Shows active tolerance level: "Tolerance: ±1 minute" or "Exact only" for strict mode
- Hints help users understand the current validation rules

#### Smart State Management
- Uses dependency injection for clean architecture
- LockCoordinator tracks lock state globally
- ViewModel pattern separates UI from business logic
- Survives activity recreation and memory pressure

## Architecture

### Project Structure

```
ScreenLockApp/
├── ScreenLockApp.Core/          # Shared business logic
│   ├── AppConfig.cs             # Configuration constants
│   ├── ITimeProvider.cs         # Time abstraction for testing
│   ├── IPreferencesProvider.cs  # Settings abstraction
│   ├── TimeValidator.cs         # Time validation logic
│   ├── LockCoordinator.cs       # Global state management
│   └── LockViewModel.cs         # Lock screen view model
├── ScreenLockApp/               # Android application
│   ├── App.cs                   # Application class with DI setup
│   ├── MainActivity.cs          # Main settings screen
│   ├── LockActivity.cs          # Lock screen with keypad
│   ├── LockService.cs           # Foreground monitoring service
│   ├── TimeChangeReceiver.cs    # Time change broadcast receiver
│   ├── PreferencesProvider.cs   # SharedPreferences implementation
│   └── Resources/               # Android resources
│       ├── layout/              # XML layouts
│       ├── values/              # Strings, styles, arrays
│       └── drawable/            # Button backgrounds
└── ScreenLockApp.Tests/         # Unit and property-based tests
    ├── TimeValidatorTests.cs    # Traditional unit tests
    └── TimeValidatorPropertyTests.cs  # FsCheck property tests
```

### Dependency Injection

The app uses `Microsoft.Extensions.DependencyInjection` for service management:

- **Singletons**: TimeProvider, LockCoordinator, PreferencesProvider
- **Transients**: TimeValidator (with current tolerance), LockViewModel
- Services are injected via `App.GetService<T>()` static helper

### Testing

#### Unit Tests (20 tests)
- Input validation (empty, null, invalid format, invalid ranges)
- Exact time matching
- Tolerance boundary testing (±1 minute default)
- Midnight wrap-around scenarios
- Edge cases and boundary conditions

#### Property-Based Tests (7 properties)
Uses FsCheck to generate randomized test cases:
- Exact match always valid regardless of tolerance
- Within tolerance range always valid
- Outside tolerance range always invalid  
- Midnight wrap-around respects tolerance (forward and backward)
- Invalid input always rejected
- Zero tolerance accepts only exact match

All 27 tests pass consistently.

## Threat Model

### Security Considerations

**What This App Protects Against:**
- Casual unauthorized access when device is left unattended
- Automated unlock attempts (requires human to know current time)
- Social engineering attacks that rely on predictable passwords

**Known Limitations:**
1. **Time Synchronization**: 
   - User can manually change system time to bypass lock
   - Requires device owner mode to prevent Settings access
   - Time change detection notifies but cannot prevent

2. **Device Reboot**:
   - Lock state does not persist across reboots
   - Grace period timer resets on app restart

3. **Safe Mode / ADB**:
   - Technical users can bypass via Android debug tools
   - Not suitable for high-security scenarios

4. **Accessibility Services**:
   - Other apps with accessibility permissions may interact with UI
   - Consider disabling accessibility for lock activity

**Recommended Use Cases:**
- Personal device protection from casual access
- Parental controls with time-awareness requirement
- Educational demonstration of time-based authentication
- Privacy screen for shared devices in trusted environments

**Not Recommended For:**
- Enterprise security
- Protection of sensitive financial/health data  
- Scenarios where attacker has physical device control

### Mitigation Strategies

To enhance security:
1. Enable Lock Task Mode (requires device owner setup)
2. Use strict tolerance (0 minutes) for maximum security
3. Disable grace period or use short duration (1 minute)
4. Combine with Android's built-in screen lock
5. Monitor device policy for time change attempts

## Installation

### Prerequisites
- .NET 9.0 SDK or later
- Android workload: `dotnet workload install android`
- Android device or emulator (API 26+)

### Build and Run

```bash
# Clone the repository
git clone https://github.com/Nkkinsoft/ScreenLockApp.git
cd ScreenLockApp

# Build the solution
dotnet build

# Run tests
dotnet test

# Deploy to connected device/emulator
dotnet build ScreenLockApp/ScreenLockApp.csproj -t:Install -f net9.0-android
```

## Usage

1. **Configure Settings**:
   - Open the app
   - Select desired tolerance level (Strict/Normal/Relaxed)
   - Configure grace period if desired
   - Enable Lock Task Mode (optional, requires device owner)

2. **Trigger Lock**:
   - Tap "Trigger Screen Lock" button
   - Lock screen appears immediately

3. **Unlock**:
   - Enter current time using on-screen keypad
   - Format: HHMM (24-hour, e.g., 1342 for 1:42 PM)
   - Tap Submit when 4 digits entered
   - Screen unlocks if time is valid within tolerance

4. **Hints**:
   - After 3 failed attempts, hint appears
   - Shows current tolerance setting
   - Clear and retry with correct time

## Configuration

All settings are persisted in SharedPreferences:

| Setting | Key | Type | Default | Range |
|---------|-----|------|---------|-------|
| Tolerance | `tolerance_minutes` | int | 1 | 0-2 |
| Grace Period Enabled | `grace_period_enabled` | bool | true | - |
| Grace Period Duration | `grace_period_minutes` | int | 2 | 0, 1, 2, 5 |
| Last Unlock Time | `last_unlock_epoch_ms` | long | 0 | Unix epoch |
| Lock Task Mode | `lock_task_mode_enabled` | bool | false | - |

## Contributing

Contributions are welcome! Please ensure:
- All tests pass (`dotnet test`)
- Code follows existing style
- New features include tests
- README updated for significant changes

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with .NET for Android (formerly Xamarin.Android)
- Uses Microsoft.Extensions.DependencyInjection for IoC
- Property-based testing with FsCheck
- UI design follows Material Design guidelines

## Support

For issues, feature requests, or questions:
- Open an issue on GitHub
- Contact: [Project Owner]

---

**Version**: 1.0.0  
**Last Updated**: November 2025  
**Minimum Android Version**: 8.0 (API 26)  
**Target Android Version**: 14 (API 34)

