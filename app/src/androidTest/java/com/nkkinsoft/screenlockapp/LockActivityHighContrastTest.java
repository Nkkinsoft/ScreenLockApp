package com.nkkinsoft.screenlockapp;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.widget.TextView;

import androidx.test.core.app.ActivityScenario;
import androidx.test.core.app.ApplicationProvider;
import androidx.test.ext.junit.runners.AndroidJUnit4;
import androidx.test.platform.app.InstrumentationRegistry;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;

import static org.junit.Assert.*;

/**
 * Instrumentation test for LockActivity in high contrast mode.
 * Verifies that resolved colorError matches expected high contrast value.
 */
@RunWith(AndroidJUnit4.class)
public class LockActivityHighContrastTest {
    
    private Context context;
    private static final int EXPECTED_HIGH_CONTRAST_ERROR = Color.parseColor("#FF5555");
    
    @Before
    public void setUp() {
        context = ApplicationProvider.getApplicationContext();
    }
    
    @Test
    public void testLockActivityUsesHighContrastErrorColor() {
        // Enable high contrast theme
        ThemeManager themeManager = new ThemeManager(context);
        themeManager.setHighContrastEnabled(true);
        
        // Launch LockActivity
        ActivityScenario<LockActivity> scenario = ActivityScenario.launch(LockActivity.class);
        
        scenario.onActivity(activity -> {
            // Verify high contrast theme is applied
            ThemeManager activityThemeManager = new ThemeManager(activity);
            assertTrue(
                "High contrast should be enabled",
                activityThemeManager.isHighContrastEnabled()
            );
            
            // Get the error text view
            TextView errorText = activity.findViewById(R.id.error_text);
            assertNotNull("Error text view should exist", errorText);
            
            // Get the resolved color from theme attribute
            int[] attrs = {R.attr.colorError};
            android.content.res.TypedArray ta = activity.getTheme().obtainStyledAttributes(attrs);
            int resolvedErrorColor = ta.getColor(0, 0);
            ta.recycle();
            
            // Verify it matches the expected high contrast error color
            assertEquals(
                "Resolved colorError should match high contrast error color",
                EXPECTED_HIGH_CONTRAST_ERROR,
                resolvedErrorColor
            );
            
            // Verify the color meets WCAG AA contrast requirements
            int backgroundColor = Color.parseColor("#0B0B0E");
            double contrast = ContrastChecker.calculateContrast(resolvedErrorColor, backgroundColor);
            assertTrue(
                "Error color must meet WCAG AA (4.5:1) contrast requirement. Actual: " + 
                String.format("%.2f", contrast) + ":1",
                contrast >= 4.5
            );
        });
        
        scenario.close();
        
        // Clean up - reset to default theme
        themeManager.setHighContrastEnabled(false);
    }
    
    @Test
    public void testLockActivityUsesDefaultErrorColorWhenHighContrastDisabled() {
        // Ensure high contrast is disabled
        ThemeManager themeManager = new ThemeManager(context);
        themeManager.setHighContrastEnabled(false);
        
        // Launch LockActivity
        ActivityScenario<LockActivity> scenario = ActivityScenario.launch(LockActivity.class);
        
        scenario.onActivity(activity -> {
            // Verify high contrast is disabled
            ThemeManager activityThemeManager = new ThemeManager(activity);
            assertFalse(
                "High contrast should be disabled",
                activityThemeManager.isHighContrastEnabled()
            );
            
            // Get the resolved color from theme attribute
            int[] attrs = {R.attr.colorError};
            android.content.res.TypedArray ta = activity.getTheme().obtainStyledAttributes(attrs);
            int resolvedErrorColor = ta.getColor(0, 0);
            ta.recycle();
            
            // Verify it's NOT the high contrast color
            assertNotEquals(
                "Resolved colorError should NOT be high contrast error color when disabled",
                EXPECTED_HIGH_CONTRAST_ERROR,
                resolvedErrorColor
            );
        });
        
        scenario.close();
    }
}
