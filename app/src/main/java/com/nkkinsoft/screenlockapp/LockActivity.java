package com.nkkinsoft.screenlockapp;

import android.os.Bundle;
import android.view.KeyEvent;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;

/**
 * Lock screen activity that displays a PIN entry interface.
 * Demonstrates the high-contrast error color display.
 */
public class LockActivity extends AppCompatActivity {
    private static final String DEMO_PIN = "1234";
    
    private ThemeManager themeManager;
    private EditText pinInput;
    private TextView errorText;
    private Button unlockButton;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        // Apply theme before setContentView
        themeManager = new ThemeManager(this);
        themeManager.apply();
        
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_lock);
        
        pinInput = findViewById(R.id.pin_input);
        errorText = findViewById(R.id.error_text);
        unlockButton = findViewById(R.id.btn_unlock);
        
        unlockButton.setOnClickListener(v -> validatePin());
        
        // Hide error text initially
        errorText.setVisibility(TextView.GONE);
    }
    
    private void validatePin() {
        String enteredPin = pinInput.getText().toString();
        
        if (enteredPin.equals(DEMO_PIN)) {
            // Correct PIN
            errorText.setVisibility(TextView.GONE);
            finish();
        } else {
            // Incorrect PIN - show error with theme-aware color
            errorText.setVisibility(TextView.VISIBLE);
            errorText.setText(R.string.incorrect_pin);
            pinInput.setText("");
        }
    }
    
    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        // Disable back button
        if (keyCode == KeyEvent.KEYCODE_BACK) {
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }
}
