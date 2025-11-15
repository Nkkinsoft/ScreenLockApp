package com.nkkinsoft.screenlockapp;

import android.content.Context;
import android.content.SharedPreferences;
import androidx.preference.PreferenceManager;

/**
 * ThemeManager handles theme application and switching between
 * standard dark theme and high-contrast theme variants.
 */
public class ThemeManager {
    private static final String PREF_HIGH_CONTRAST = "high_contrast";
    
    private final Context context;
    private final SharedPreferences preferences;
    
    public ThemeManager(Context context) {
        this.context = context;
        this.preferences = PreferenceManager.getDefaultSharedPreferences(context);
    }
    
    /**
     * Apply the appropriate theme based on user preferences.
     * Should be called before setContentView() in activities.
     */
    public void apply() {
        if (isHighContrastEnabled()) {
            context.setTheme(R.style.Theme_ScreenLock_HighContrast);
        } else {
            context.setTheme(R.style.Theme_ScreenLock_Dark);
        }
    }
    
    /**
     * Check if high contrast theme is enabled.
     */
    public boolean isHighContrastEnabled() {
        return preferences.getBoolean(PREF_HIGH_CONTRAST, false);
    }
    
    /**
     * Enable or disable high contrast theme.
     */
    public void setHighContrastEnabled(boolean enabled) {
        preferences.edit().putBoolean(PREF_HIGH_CONTRAST, enabled).apply();
    }
    
    /**
     * Get the currently active theme resource ID.
     */
    public int getCurrentTheme() {
        return isHighContrastEnabled() 
            ? R.style.Theme_ScreenLock_HighContrast 
            : R.style.Theme_ScreenLock_Dark;
    }
}
