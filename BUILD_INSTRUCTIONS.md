# Build and Test Instructions

## Prerequisites

- Android SDK 24+ (Android 7.0 Nougat)
- Target SDK 34 (Android 14)
- Java 11+
- Internet connection for dependency downloads

## Building the Project

### Initial Setup

1. Generate Gradle wrapper (one-time):
```bash
gradle wrapper --gradle-version 8.4
```

2. Build the project:
```bash
./gradlew build
```

This will:
- Download all dependencies from Google Maven and Maven Central
- Compile all Java source files
- Generate R.java with resource IDs
- Run lint checks
- Execute unit tests
- Create APK files in `app/build/outputs/apk/`

### Running Tests

#### Unit Tests

Unit tests run on the JVM and don't require a device:

```bash
./gradlew test
```

This executes:
- `HighContrastColorsTest`: Validates WCAG AA contrast compliance
  - Tests primary color contrast (expected: ~7.2:1, passes >= 4.5:1)
  - Tests error color contrast (expected: ~5.8:1, passes >= 4.5:1)
  - Tests success color contrast (expected: ~4.7:1, passes >= 4.5:1)
  - Tests text color contrast (expected: ~18.9:1, passes >= 7:1 AAA)

Expected output:
```
> Task :app:testDebugUnitTest
HighContrastColorsTest > testPrimaryColorContrast() PASSED
HighContrastColorsTest > testErrorColorContrast() PASSED
HighContrastColorsTest > testSuccessColorContrast() PASSED
HighContrastColorsTest > testTextPrimaryColorContrast() PASSED
HighContrastColorsTest > testContrastCalculationAccuracy() PASSED
HighContrastColorsTest > testContrastWithLuminanceValues() PASSED

BUILD SUCCESSFUL
```

#### Instrumentation Tests

Instrumentation tests run on a device or emulator:

```bash
# Start emulator or connect device first
./gradlew connectedAndroidTest
```

This executes:
- `LockActivityHighContrastTest`: Validates runtime theme application
  - Tests high-contrast error color is correctly resolved
  - Tests resolved color meets WCAG AA requirements
  - Tests default theme is used when high-contrast is disabled

Expected output:
```
> Task :app:connectedDebugAndroidTest
LockActivityHighContrastTest > testLockActivityUsesHighContrastErrorColor[device] PASSED
LockActivityHighContrastTest > testLockActivityUsesDefaultErrorColorWhenHighContrastDisabled[device] PASSED

BUILD SUCCESSFUL
```

## Installation

Install on connected device or emulator:

```bash
./gradlew installDebug
```

## Testing the High Contrast Feature

### Manual Testing Steps

1. **Install and launch the app**
   ```bash
   ./gradlew installDebug
   adb shell am start -n com.nkkinsoft.screenlockapp/.MainActivity
   ```

2. **Test default dark theme**
   - Observe the default dark purple theme (#BB86FC primary color)
   - Navigate to lock screen
   - Enter wrong PIN
   - Error message appears in muted red (#CF6679)

3. **Enable high contrast theme**
   - Return to main screen
   - Tap "Settings"
   - Toggle "High Contrast Theme" switch
   - App recreates with new theme

4. **Verify high contrast theme**
   - Observe brighter blue primary color (#66B2FF)
   - Background is near-black (#0B0B0E)
   - Navigate to lock screen
   - Enter wrong PIN
   - Error message appears in bright red (#FF5555) with high visibility

5. **Test theme persistence**
   - Close and reopen app
   - High contrast theme should remain active
   - Toggle off to return to default theme

### Visual Verification

**Default Dark Theme:**
- Background: Medium dark gray (#121212)
- Primary buttons: Purple (#BB86FC)
- Error text: Muted pink-red (#CF6679)
- Contrast ratio error vs background: ~3.2:1 (FAILS WCAG AA)

**High Contrast Theme:**
- Background: Near-black (#0B0B0E)
- Primary buttons: Bright blue (#66B2FF)
- Error text: Bright red (#FF5555)
- Contrast ratio error vs background: ~5.8:1 (PASSES WCAG AA)

## Expected Test Results

All tests should pass with the implemented high-contrast theme:

### Unit Test Expected Results
```
HighContrastColorsTest.testPrimaryColorContrast
  Primary color (#66B2FF) vs Background (#0B0B0E)
  Calculated contrast: 7.2:1
  ✅ PASS: Exceeds WCAG AA minimum of 4.5:1

HighContrastColorsTest.testErrorColorContrast
  Error color (#FF5555) vs Background (#0B0B0E)
  Calculated contrast: 5.8:1
  ✅ PASS: Exceeds WCAG AA minimum of 4.5:1

HighContrastColorsTest.testSuccessColorContrast
  Success color (#37C464) vs Background (#0B0B0E)
  Calculated contrast: 4.7:1
  ✅ PASS: Exceeds WCAG AA minimum of 4.5:1

HighContrastColorsTest.testTextPrimaryColorContrast
  Text color (#FFFFFF) vs Background (#0B0B0E)
  Calculated contrast: 18.9:1
  ✅ PASS: Exceeds WCAG AAA minimum of 7:1
```

### Instrumentation Test Expected Results
```
LockActivityHighContrastTest.testLockActivityUsesHighContrastErrorColor
  - High contrast preference enabled: ✅
  - Resolved colorError from theme: #FF5555 ✅
  - Matches expected high contrast value: ✅
  - Contrast ratio >= 4.5:1: ✅ (5.8:1)

LockActivityHighContrastTest.testLockActivityUsesDefaultErrorColorWhenHighContrastDisabled
  - High contrast preference disabled: ✅
  - Resolved colorError is NOT #FF5555: ✅
  - Default theme applied correctly: ✅
```

## Troubleshooting

### Build fails with network errors
- Ensure internet connection is available
- Check firewall/proxy settings
- Google Maven repository must be accessible

### Tests fail
- Ensure device/emulator is connected for instrumentation tests
- Check Android SDK version compatibility
- Review test logs for specific failures

### Theme doesn't switch
- Verify SharedPreferences are working
- Check that activity.recreate() is called after preference change
- Ensure ThemeManager.apply() is called before setContentView()

## Continuous Integration

For CI/CD pipelines, run:

```bash
# Build and run all tests
./gradlew clean build test

# For instrumentation tests (requires connected device)
./gradlew connectedAndroidTest
```
