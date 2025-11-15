# ScreenLockApp

Advanced screen lock Android application with multi-challenge architecture, high-contrast accessibility, and comprehensive security features.

## Version

**v0.2.0-alpha** - Multi-challenge architecture with accessibility enhancements

## Features

### Multi-Challenge Architecture
ScreenLockApp supports five different unlock challenges, each designed to provide unique security and usability characteristics:

1. **Time Challenge** - Enter current time in HHMM format
2. **Battery Challenge** - Enter current battery percentage
3. **Hour Challenge** - Enter current hour (00-23)
4. **Daily Code Challenge** - Enter day-specific code that rotates weekly
5. **Battery + Hour Challenge** - Composite challenge combining battery percentage and hour

### Challenge Comparison

| Challenge | Format | Tolerance Support | Use Case |
|-----------|--------|-------------------|----------|
| Time | HHMM (e.g., 1430) | Yes (0-15 min) | Dynamic, changes every minute |
| Battery | BB (e.g., 87) | Yes (0-20%) | Device-state dependent |
| Hour | HH (e.g., 14) | No | Changes hourly, simpler input |
| Daily Code | 4-digit code | No | Memorizable, changes daily |
| Battery+Hour | BBHH (e.g., 8714) | Yes (battery only) | Maximum security, dual factor |

### Settings & Preferences

#### Challenge Configuration
- **Active Challenge**: Switch between available challenges
- **Time Tolerance**: Minutes of tolerance for time-based validation (0-15 min)
- **Battery Tolerance**: Percentage tolerance for battery-based validation (0-20%)
- **Grace Period**: Minutes before re-lock after successful unlock (0-60 min)
- **Strict Mode**: Forces all tolerances to 0 for maximum security

#### Appearance & Accessibility
- **High Contrast Theme**: Enhanced color palette meeting WCAG AA standards
  - Primary: #000080 (dark blue) on #000000 (black) background
  - Error: #FF0000 (bright red) - contrast ratio 5.25:1
  - Success: #00FF00 (bright green) - contrast ratio 15.3:1
  - All color combinations meet minimum 4.5:1 contrast requirement
- **Theme Mode**: Light, Dark, or System default
- **TalkBack Support**: Full screen reader accessibility with live regions
- **Content Descriptions**: All interactive elements labeled for accessibility

#### Behavior
- **Enable Haptics**: Vibration feedback on button presses
- **Auto Start**: Launch lock screen on device boot
- **Debug Mode**: Enable diagnostic logging for troubleshooting

### High-Contrast Theme

The high-contrast theme provides enhanced visibility with carefully selected colors:

**Color Palette:**
- Background: #000000 (pure black)
- Primary: #000080 (navy blue)
- Accent: #00FFFF (cyan)
- Error: #FF0000 (pure red)
- Success: #00FF00 (pure green)
- Text: #FFFFFF (pure white)

**Contrast Ratios:**
All combinations achieve WCAG AA compliance (≥4.5:1):
- Error on Black: 5.25:1 ✓
- Success on Black: 15.3:1 ✓
- White Text on Black: 21:1 ✓

### Accessibility Features

1. **TalkBack Integration**
   - Challenge change announcements
   - Validation failure notifications
   - Comprehensive content descriptions

2. **Live Regions**
   - Error messages announced automatically
   - Dynamic hint updates

3. **Keyboard Navigation**
   - All controls accessible via external keyboard
   - Logical tab order

4. **Touch Targets**
   - Minimum 48dp touch target size
   - Adequate spacing between interactive elements

### Foreground Service & Receivers

- **LockScreenService**: Persistent foreground service keeping lock active
- **TimeChangeReceiver**: Monitors time changes (TIME_TICK, TIME_SET, TIMEZONE_CHANGED)
- **BatteryChangeReceiver**: Monitors battery level changes

### Security Features

#### Challenge Validation
- Input buffering with secure clearing
- Strict mode for zero-tolerance validation
- Grace period prevents immediate re-lock

#### Special Cases Handled
- **Battery 100% Normalization**: Automatically normalizes to 99% for consistency
- **Midnight Hour Transition**: Correctly handles 00:00 hour validation
- **Week Boundary**: Daily codes transition smoothly across week boundaries
- **Time Zone Changes**: Automatically adjusts time-based challenges

#### Threat Model

**What This App Protects Against:**
- Casual unauthorized access by others
- Accidental unlocks
- Simple shoulder-surfing (with strict mode)

**What This App Does NOT Protect Against:**
- Determined attackers with physical access
- Exploitation of Android vulnerabilities
- Brute force attacks (no rate limiting implemented)
- Device owner privileges required for true kiosk mode

**Note**: This is a lock screen *supplement*, not a replacement for Android's built-in security. Always use in conjunction with device PIN/password/biometrics.

## Testing

### Unit Tests
Located in `ScreenLockApp.Tests/Unit/`:
- `ContrastCheckerTests.cs`: Validates WCAG contrast ratio calculations
- `TimeValidatorTests.cs`: Tests time validation logic and tolerance handling

### Property-Based Tests
Located in `ScreenLockApp.Tests/PropertyBased/`:
- `TimeValidatorPropertyTests.cs`: FsCheck-based invariant testing
  - Zero tolerance exact match property
  - Symmetric tolerance property
  - HHMM format validity property

### Instrumentation Tests
Located in `ScreenLockApp.Tests/Instrumentation/`:
- `ChallengeSwitchHintTest.cs`: Dynamic challenge switching without restart
- `HighContrastColorResolutionTest.cs`: Theme attribute resolution
- `MinuteTickAutoUnlockTest.cs`: Time-based auto-unlock with test provider
- `BatteryPercentUpdateTest.cs`: Battery broadcast handling

**Running Tests:**
```bash
# Unit and property-based tests
dotnet test ScreenLockApp.Tests/ScreenLockApp.Tests.csproj

# Instrumentation tests (requires Android device/emulator)
# These are marked as Skip and require AndroidX.Test framework
```

## Architecture

### Core Components

```
ScreenLockApp/
├── Activities/
│   ├── MainActivity.cs          # Main launcher activity
│   ├── SettingsActivity.cs      # Preferences UI
│   └── LockActivity.cs          # Lock screen UI
├── Challenges/
│   ├── IUnlockChallenge.cs      # Challenge interface
│   ├── ChallengeRegistry.cs     # Challenge management
│   ├── TimeChallenge.cs         # Time-based challenge
│   ├── BatteryChallenge.cs      # Battery-based challenge
│   ├── HourChallenge.cs         # Hour-based challenge
│   ├── DailyCodeChallenge.cs    # Daily rotating code
│   └── BatteryHourChallenge.cs  # Composite challenge
├── ViewModels/
│   └── LockViewModel.cs         # Lock screen state & validation
├── Themes/
│   ├── ThemeManager.cs          # Theme application
│   └── ContrastChecker.cs       # WCAG contrast validation
├── Utils/
│   ├── ITimeProvider.cs         # Time abstraction (testable)
│   └── TimeValidator.cs         # Time validation logic
├── Services/
│   └── LockScreenService.cs     # Foreground service
└── Receivers/
    ├── TimeChangeReceiver.cs    # Time broadcast receiver
    └── BatteryChangeReceiver.cs # Battery broadcast receiver
```

### Design Patterns

- **Strategy Pattern**: `IUnlockChallenge` interface for pluggable challenges
- **Registry Pattern**: `ChallengeRegistry` for challenge management
- **Observer Pattern**: `ChallengeChangedEventHandler` for state notifications
- **Dependency Injection**: `ITimeProvider` abstraction for testability
- **MVVM**: ViewModel separates UI logic from Activities

## Building

### Prerequisites
- .NET 8.0 SDK or later
- Android SDK (API 21+)
- Optional: Android workload (`dotnet workload install android`)

### Build Commands
```bash
# Restore dependencies
dotnet restore ScreenLockApp.csproj

# Build debug version
dotnet build ScreenLockApp.csproj -c Debug

# Build release version
dotnet build ScreenLockApp.csproj -c Release

# Run unit tests
dotnet test ScreenLockApp.Tests/ScreenLockApp.Tests.csproj
```

## Installation

1. Build the APK:
   ```bash
   dotnet build ScreenLockApp.csproj -c Release -t:SignAndroidPackage
   ```

2. Install on device:
   ```bash
   adb install -r bin/Release/net8.0-android/com.nkkinsoft.screenlockapp-Signed.apk
   ```

3. Grant necessary permissions:
   - System alert window permission
   - Notification permission (Android 13+)

## Usage

### First Launch
1. Open the app
2. Tap "Settings" to configure your preferred challenge
3. Adjust tolerance values and enable strict mode if desired
4. Return to main screen
5. Tap "Lock Screen" to test

### Switching Challenges
1. Open Settings
2. Select "Active Challenge"
3. Choose from available challenges
4. Changes take effect immediately

### Enabling High-Contrast Theme
1. Open Settings
2. Enable "High Contrast Theme"
3. Theme applies instantly to all screens

## Roadmap

### Completed (v0.2.0-alpha)
- ✅ Multi-challenge architecture
- ✅ High-contrast theme with WCAG compliance
- ✅ Comprehensive accessibility features
- ✅ Unit, property-based, and instrumentation tests
- ✅ Debug mode logging
- ✅ Complete documentation

### Planned Features
- [ ] Custom daily codes (user-defined)
- [ ] Challenge scheduling (different challenges at different times)
- [ ] Biometric integration
- [ ] PIN fallback mechanism
- [ ] Rate limiting for brute force protection
- [ ] Device admin/kiosk mode support
- [ ] Challenge history and statistics
- [ ] Localization (multiple languages)
- [ ] Widget for quick lock
- [ ] Themed icon options

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

[License information to be added]

## Acknowledgments

- Built with .NET for Android (formerly Xamarin.Android)
- Uses AndroidX libraries for modern Android compatibility
- FsCheck for property-based testing
- xUnit for unit testing framework

## Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Email: nkkinx@gmail.com

---

**Disclaimer**: This application is provided as-is without warranty. Use at your own risk. Not intended to replace device security measures.
