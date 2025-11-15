package com.nkkinsoft.screenlockapp;

import android.graphics.Color;

/**
 * Utility class for calculating and validating color contrast ratios.
 * Implements WCAG 2.0 contrast calculation algorithm.
 */
public class ContrastChecker {
    
    /**
     * Calculate the contrast ratio between two colors.
     * 
     * @param color1 First color (typically foreground)
     * @param color2 Second color (typically background)
     * @return Contrast ratio (1:1 to 21:1)
     */
    public static double calculateContrast(int color1, int color2) {
        double luminance1 = calculateLuminance(color1);
        double luminance2 = calculateLuminance(color2);
        
        // Ensure lighter color is in numerator
        double lighter = Math.max(luminance1, luminance2);
        double darker = Math.min(luminance1, luminance2);
        
        return (lighter + 0.05) / (darker + 0.05);
    }
    
    /**
     * Calculate the contrast ratio between two colors given their luminance values.
     * 
     * @param luminanceA Relative luminance of first color
     * @param luminanceB Relative luminance of second color
     * @return Contrast ratio (1:1 to 21:1)
     */
    public static double calculateContrast(double luminanceA, double luminanceB) {
        double lighter = Math.max(luminanceA, luminanceB);
        double darker = Math.min(luminanceA, luminanceB);
        
        return (lighter + 0.05) / (darker + 0.05);
    }
    
    /**
     * Calculate relative luminance of a color.
     * Uses WCAG 2.0 formula.
     * 
     * @param color Android color int
     * @return Relative luminance (0 to 1)
     */
    public static double calculateLuminance(int color) {
        // Extract RGB components and normalize to 0-1
        double r = Color.red(color) / 255.0;
        double g = Color.green(color) / 255.0;
        double b = Color.blue(color) / 255.0;
        
        // Apply gamma correction
        r = (r <= 0.03928) ? r / 12.92 : Math.pow((r + 0.055) / 1.055, 2.4);
        g = (g <= 0.03928) ? g / 12.92 : Math.pow((g + 0.055) / 1.055, 2.4);
        b = (b <= 0.03928) ? b / 12.92 : Math.pow((b + 0.055) / 1.055, 2.4);
        
        // Calculate luminance using ITU-R BT.709 coefficients
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }
    
    /**
     * Check if contrast ratio meets WCAG AA standard for normal text (4.5:1).
     * 
     * @param color1 First color
     * @param color2 Second color
     * @return true if contrast meets AA standard
     */
    public static boolean meetsWCAG_AA(int color1, int color2) {
        return calculateContrast(color1, color2) >= 4.5;
    }
    
    /**
     * Check if contrast ratio meets WCAG AAA standard for normal text (7:1).
     * 
     * @param color1 First color
     * @param color2 Second color
     * @return true if contrast meets AAA standard
     */
    public static boolean meetsWCAG_AAA(int color1, int color2) {
        return calculateContrast(color1, color2) >= 7.0;
    }
}
