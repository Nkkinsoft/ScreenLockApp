# Changelog

All notable changes to ScreenLockApp will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0-alpha] - 2025-11-15

### Added
- **Multi-Challenge Architecture**
  - Five distinct unlock challenges: Time, Battery, Hour, Daily Code, Battery+Hour
  - `IUnlockChallenge` interface for extensible challenge system
  - `ChallengeRegistry` for dynamic challenge management
  - `ChallengeChangeNotifier` event system for real-time UI updates

- **Challenge Implementations**
  - `TimeChallenge`: HHMM format with configurable tolerance (0-15 minutes)
  - `BatteryChallenge`: Battery percentage with tolerance (0-20%)
  - `HourChallenge`: Current hour (00-23) validation
  - `DailyCodeChallenge`: Day-of-week rotating codes
  - `BatteryHourChallenge`: Composite challenge combining battery and hour

- **High-Contrast Theme**
  - Complete high-contrast color palette meeting WCAG AA standards
  - `ThemeManager` for consistent theme application across activities
  - `ContrastChecker` utility for WCAG compliance validation
  - All contrast ratios ≥4.5:1 verified through unit tests
  - Dynamic theme switching without app restart

- **Accessibility Features**
  - TalkBack support with challenge change announcements
  - Live regions for error messages
  - Comprehensive content descriptions on all interactive elements
  - Haptic feedback toggle preference
  - Minimum 48dp touch targets throughout

- **Testing Infrastructure**
  - Unit tests for `TimeValidator` and `ContrastChecker`
  - Property-based tests using FsCheck for `TimeValidator` invariants
  - Instrumentation tests (skeleton) for UI and integration testing
  - `TestTimeProvider` for deterministic time-based testing

- **Preferences & Configuration**
  - Settings Activity with comprehensive preference screen
  - Strict mode for zero-tolerance validation
  - Grace period to prevent immediate re-lock
  - Theme mode selection (Light/Dark/System)
  - Debug mode logging with guard to prevent sensitive data exposure

- **Services & Receivers**
  - `LockScreenService` foreground service
  - `TimeChangeReceiver` for time tick and timezone change events
  - `BatteryChangeReceiver` for battery level updates

- **Documentation**
  - Comprehensive README with features, architecture, and usage
  - Challenge comparison table
  - Contrast ratio documentation
  - Threat model and security considerations
  - CHANGELOG for version tracking

- **Special Case Handling**
  - Battery 100% normalization to 99%
  - Midnight hour transition (00:00) support
  - Daily code week boundary transitions
  - Timezone change handling

### Changed
- Updated app version to 0.2.0-alpha
- Enhanced theme system to support dynamic switching
- Improved input validation with tolerance ranges

### Technical Details
- Target Framework: .NET 8.0 for Android
- Minimum Android API: 21 (Android 5.0)
- Dependencies:
  - Xamarin.AndroidX.AppCompat 1.6.1.7
  - Xamarin.Google.Android.Material 1.10.0.2
  - Xamarin.AndroidX.ConstraintLayout 2.1.4.7
  - Xamarin.AndroidX.Preference 1.2.1.6

### Testing
- 7 unit tests for core functionality
- 3 property-based tests for invariants
- 4 instrumentation test skeletons (require Android runtime)

### Known Issues
- Instrumentation tests require Android device/emulator to run
- Kiosk mode requires device owner privileges (not implemented)
- No rate limiting for brute force protection
- Single language support (English only)

## [0.1.0] - 2025-11-15

### Added
- Initial repository setup
- Basic project structure
- README placeholder

---

[0.2.0-alpha]: https://github.com/Nkkinsoft/ScreenLockApp/compare/v0.1.0...v0.2.0-alpha
[0.1.0]: https://github.com/Nkkinsoft/ScreenLockApp/releases/tag/v0.1.0
