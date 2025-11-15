package com.nkkinsoft.screenlockapp;

import android.os.Bundle;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.preference.PreferenceFragmentCompat;
import androidx.preference.SwitchPreferenceCompat;

/**
 * Settings activity for configuring app preferences including theme settings.
 */
public class SettingsActivity extends AppCompatActivity {
    private ThemeManager themeManager;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        // Apply theme before setContentView
        themeManager = new ThemeManager(this);
        themeManager.apply();
        
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        
        if (savedInstanceState == null) {
            getSupportFragmentManager()
                    .beginTransaction()
                    .replace(R.id.settings_container, new SettingsFragment())
                    .commit();
        }
        
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
    }
    
    @Override
    public boolean onSupportNavigateUp() {
        onBackPressed();
        return true;
    }
    
    public static class SettingsFragment extends PreferenceFragmentCompat {
        @Override
        public void onCreatePreferences(Bundle savedInstanceState, String rootKey) {
            setPreferencesFromResource(R.xml.preferences, rootKey);
            
            // Handle high contrast toggle
            SwitchPreferenceCompat highContrastPref = findPreference("high_contrast");
            if (highContrastPref != null) {
                highContrastPref.setOnPreferenceChangeListener((preference, newValue) -> {
                    // Recreate activity to apply new theme
                    if (getActivity() != null) {
                        getActivity().recreate();
                    }
                    return true;
                });
            }
        }
    }
}
