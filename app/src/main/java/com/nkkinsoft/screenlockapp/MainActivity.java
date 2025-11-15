package com.nkkinsoft.screenlockapp;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import androidx.appcompat.app.AppCompatActivity;

/**
 * Main activity for ScreenLockApp.
 * Provides navigation to settings and lock screen activation.
 */
public class MainActivity extends AppCompatActivity {
    private ThemeManager themeManager;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        // Apply theme before setContentView
        themeManager = new ThemeManager(this);
        themeManager.apply();
        
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        Button settingsButton = findViewById(R.id.btn_settings);
        Button lockButton = findViewById(R.id.btn_activate_lock);
        
        settingsButton.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, SettingsActivity.class);
            startActivity(intent);
        });
        
        lockButton.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, LockActivity.class);
            startActivity(intent);
        });
    }
}
