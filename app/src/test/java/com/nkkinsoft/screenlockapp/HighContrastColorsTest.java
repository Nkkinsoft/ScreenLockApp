package com.nkkinsoft.screenlockapp;

import android.graphics.Color;

import org.junit.Test;

import static org.junit.Assert.*;

/**
 * Unit tests for high contrast color palette compliance with WCAG AA standards.
 * Verifies that primary and error colors meet minimum 4.5:1 contrast ratio
 * against the background color.
 */
public class HighContrastColorsTest {
    
    // High contrast theme colors (from colors_high_contrast.xml)
    private static final int HIGH_CONTRAST_BACKGROUND = Color.parseColor("#0B0B0E");
    private static final int HIGH_CONTRAST_PRIMARY = Color.parseColor("#66B2FF");
    private static final int HIGH_CONTRAST_ERROR = Color.parseColor("#FF5555");
    private static final int HIGH_CONTRAST_SUCCESS = Color.parseColor("#37C464");
    private static final int HIGH_CONTRAST_TEXT_PRIMARY = Color.parseColor("#FFFFFF");
    
    // WCAG AA minimum contrast ratio for normal text
    private static final double WCAG_AA_MINIMUM = 4.5;
    
    @Test
    public void testPrimaryColorContrast() {
        double contrast = ContrastChecker.calculateContrast(
            HIGH_CONTRAST_PRIMARY, 
            HIGH_CONTRAST_BACKGROUND
        );
        
        assertTrue(
            "Primary color contrast ratio must be >= 4.5:1 for WCAG AA compliance. " +
            "Actual: " + String.format("%.2f", contrast) + ":1",
            contrast >= WCAG_AA_MINIMUM
        );
        
        // Also verify it passes the helper method
        assertTrue(
            "Primary color should meet WCAG AA standard",
            ContrastChecker.meetsWCAG_AA(HIGH_CONTRAST_PRIMARY, HIGH_CONTRAST_BACKGROUND)
        );
    }
    
    @Test
    public void testErrorColorContrast() {
        double contrast = ContrastChecker.calculateContrast(
            HIGH_CONTRAST_ERROR, 
            HIGH_CONTRAST_BACKGROUND
        );
        
        assertTrue(
            "Error color contrast ratio must be >= 4.5:1 for WCAG AA compliance. " +
            "Actual: " + String.format("%.2f", contrast) + ":1",
            contrast >= WCAG_AA_MINIMUM
        );
        
        // Also verify it passes the helper method
        assertTrue(
            "Error color should meet WCAG AA standard",
            ContrastChecker.meetsWCAG_AA(HIGH_CONTRAST_ERROR, HIGH_CONTRAST_BACKGROUND)
        );
    }
    
    @Test
    public void testSuccessColorContrast() {
        double contrast = ContrastChecker.calculateContrast(
            HIGH_CONTRAST_SUCCESS, 
            HIGH_CONTRAST_BACKGROUND
        );
        
        assertTrue(
            "Success color contrast ratio must be >= 4.5:1 for WCAG AA compliance. " +
            "Actual: " + String.format("%.2f", contrast) + ":1",
            contrast >= WCAG_AA_MINIMUM
        );
    }
    
    @Test
    public void testTextPrimaryColorContrast() {
        double contrast = ContrastChecker.calculateContrast(
            HIGH_CONTRAST_TEXT_PRIMARY, 
            HIGH_CONTRAST_BACKGROUND
        );
        
        assertTrue(
            "Text primary color contrast ratio must be >= 4.5:1 for WCAG AA compliance. " +
            "Actual: " + String.format("%.2f", contrast) + ":1",
            contrast >= WCAG_AA_MINIMUM
        );
        
        // White on near-black should actually meet AAA (7:1)
        assertTrue(
            "White text should achieve very high contrast",
            contrast >= 7.0
        );
    }
    
    @Test
    public void testContrastCalculationAccuracy() {
        // Test with known values: white on black should be 21:1
        int white = Color.parseColor("#FFFFFF");
        int black = Color.parseColor("#000000");
        
        double contrast = ContrastChecker.calculateContrast(white, black);
        
        // Allow small floating point error
        assertTrue(
            "White on black should be approximately 21:1. Actual: " + contrast,
            Math.abs(contrast - 21.0) < 0.1
        );
    }
    
    @Test
    public void testContrastWithLuminanceValues() {
        // Test the overloaded method that takes luminance values
        double luminance1 = ContrastChecker.calculateLuminance(HIGH_CONTRAST_PRIMARY);
        double luminance2 = ContrastChecker.calculateLuminance(HIGH_CONTRAST_BACKGROUND);
        
        double contrastFromLuminance = ContrastChecker.calculateContrast(luminance1, luminance2);
        double contrastFromColors = ContrastChecker.calculateContrast(
            HIGH_CONTRAST_PRIMARY, 
            HIGH_CONTRAST_BACKGROUND
        );
        
        // Both methods should produce the same result
        assertEquals(
            "Contrast calculation should be consistent",
            contrastFromColors, 
            contrastFromLuminance, 
            0.01
        );
    }
}
