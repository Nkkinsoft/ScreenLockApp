# ScreenLockApp

Advanced Screen Lock Android App with Multi-Challenge Authentication

## Overview

ScreenLockApp is a .NET for Android application that provides a customizable screen lock system with multiple unlock challenge types. Users can choose from various authentication methods beyond traditional PIN or pattern locks.

## Features

### Lock Challenge Types

1. **Time Challenge**
   - Enter the current time in HHMM format (24-hour)
   - Example: `1430` for 2:30 PM
   - Configurable tolerance: 0, ±1, ±2, ±5, or ±10 minutes
   - Handles day boundaries correctly

2. **Battery Challenge**
   - Enter the current battery percentage as an integer
   - Example: `57` for 57%
   - Configurable tolerance: 0 or ±1%
   - Real-time battery status monitoring

3. **Hour Challenge**
   - Enter only the current hour (00-23)
   - Example: `14` for 2 PM
   - No tolerance - exact match required
   - Simplified version of time challenge

4. **Daily Code Challenge (Beta)**
   - Enter a code derived from the current date
   - Format: DDWD (DD = day of month, W = day of week 0-6)
   - Example: 15th Wednesday = `153`
   - Deterministic and changes daily

### Settings & Configuration

#### Lock Challenge Settings
- Select active challenge type
- View challenge descriptions and examples
- Dynamic hint updates based on selected challenge

#### Tolerance & Parameters
- **Time Tolerance**: 0, ±1, ±2, ±5, or ±10 minutes
- **Battery Tolerance**: 0 or ±1%
- **Grace Period**: Off, 1, 2, or 5 minutes

#### Behavior
- **Auto-start on boot**: Automatically start lock service when device boots
- **Haptic feedback**: Vibration feedback for button presses
- **Kiosk mode**: Lock device in kiosk mode (requires device owner permissions)

#### Appearance
- **Theme mode**: Dark, Light, or System default
- **High contrast mode**: Enhanced contrast for accessibility

#### Security
- **Strict mode**: Forces exact match (tolerance = 0) for time challenge
- **NTP drift check**: Coming soon - validate time against network time
- **Biometric fallback**: Coming soon - fingerprint/face unlock option

## Architecture

### Modular Challenge System

The app uses a pluggable architecture based on the `IUnlockChallenge` interface:

```csharp
public interface IUnlockChallenge
{
    string Id { get; }                  // e.g., "time", "battery"
    string DisplayName { get; }         // User-facing name
    string Description { get; }         // Help text
    ValidationResult Validate(string input);
    string CurrentTargetHint();         // Dynamic hint
    int ExpectedInputLength { get; }    // For UI optimization
}
```

### Components

- **ChallengeRegistry**: Manages available challenges and resolves active challenge
- **PreferencesProvider**: Centralized settings management using SharedPreferences
- **BatteryStatusProvider**: Real-time battery status monitoring
- **ThemeManager**: Handles app theme switching
- **LockViewModel**: MVVM pattern for lock screen state management

## Usage

1. **Install and Launch**: Install the app and open it
2. **Configure Settings**: Tap "Settings" to choose your preferred challenge type and configure options
3. **Start Lock**: Tap "Start Lock" to activate the lock screen
4. **Unlock**: Enter the correct value based on the active challenge type

## Development

### Requirements
- .NET 8.0 or later
- Android SDK (API level 21+)
- Visual Studio 2022 or VS Code with C# extensions

### Building

```bash
dotnet build ScreenLockApp.csproj
```

### Running Tests

```bash
dotnet test ScreenLockApp.Tests/ScreenLockApp.Tests.csproj
```

## Project Structure

```
ScreenLockApp/
├── challenges/          # Challenge implementations
│   ├── IUnlockChallenge.cs
│   ├── TimeChallenge.cs
│   ├── BatteryChallenge.cs
│   ├── HourChallenge.cs
│   ├── DailyCodeChallenge.cs
│   └── ChallengeRegistry.cs
├── data/               # Data layer
│   ├── PreferencesProvider.cs
│   └── ThemeManager.cs
├── providers/          # Service providers
│   └── BatteryStatusProvider.cs
├── ui/                 # UI components
│   ├── MainActivity.cs
│   ├── LockActivity.cs
│   ├── SettingsActivity.cs
│   └── LockViewModel.cs
├── services/           # Background services
│   └── LockService.cs
├── receivers/          # Broadcast receivers
│   └── BootReceiver.cs
└── Resources/          # Android resources
    ├── layout/
    ├── values/
    └── drawable/
```

## Future Enhancements

- Biometric authentication fallback
- NTP time synchronization
- Remote unlock via web interface
- Custom challenge plugins
- PIN/Pattern backup unlock
- Emergency bypass codes
- Usage statistics and analytics

## License

Copyright © 2025 Nkkinsoft. All rights reserved.

## Author

Abdellah Amri (nkkinx@gmail.com)
