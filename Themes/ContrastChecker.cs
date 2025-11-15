using Android.Graphics;

namespace ScreenLockApp.Themes;

/// <summary>
/// Checks color contrast ratios for accessibility compliance.
/// </summary>
public static class ContrastChecker
{
    /// <summary>
    /// Calculates the contrast ratio between two colors.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>The contrast ratio (1-21).</returns>
    public static double CalculateContrastRatio(Color foreground, Color background)
    {
        var l1 = GetRelativeLuminance(foreground);
        var l2 = GetRelativeLuminance(background);
        
        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);
        
        return (lighter + 0.05) / (darker + 0.05);
    }
    
    /// <summary>
    /// Checks if contrast ratio meets WCAG AA standard (4.5:1).
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>True if contrast ratio is at least 4.5:1.</returns>
    public static bool MeetsWCAGAA(Color foreground, Color background)
    {
        return CalculateContrastRatio(foreground, background) >= 4.5;
    }
    
    /// <summary>
    /// Checks if contrast ratio meets WCAG AAA standard (7:1).
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns>True if contrast ratio is at least 7:1.</returns>
    public static bool MeetsWCAGAAA(Color foreground, Color background)
    {
        return CalculateContrastRatio(foreground, background) >= 7.0;
    }
    
    private static double GetRelativeLuminance(Color color)
    {
        var r = GetSRGB(color.R / 255.0);
        var g = GetSRGB(color.G / 255.0);
        var b = GetSRGB(color.B / 255.0);
        
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }
    
    private static double GetSRGB(double channel)
    {
        if (channel <= 0.03928)
            return channel / 12.92;
        else
            return Math.Pow((channel + 0.055) / 1.055, 2.4);
    }
}
