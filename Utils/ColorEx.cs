using Raylib_cs;

namespace Utils;

public static class ColorEx
{
    /// <summary>
    /// Returns the colour with the new alpha, where alpha should be in [0.0, 1.0].
    /// </summary>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.R, color.G, color.B, (byte)(alpha * 255.0f));
    }

    /// <summary>
    /// Returns the colour with the new alpha, where alpha should be in [0, 255].
    /// </summary>
    public static Color WithAlpha(this Color color, byte alpha)
    {
        return new Color(color.R, color.G, color.B, alpha);
    }

    /// <summary>
    /// Returns the colour interpolated towards dark, do not ask me about the colour space.
    /// </summary>
    public static Color Darken(this Color color, float amount)
    {
        return new Color((color.R / 255.0f) * (1.0f - amount), (color.G / 255.0f) * (1.0f - amount), (color.B / 255.0f) * (1.0f - amount), color.A);
    }
    
    /// <summary>
    /// Returns the colour interpolated towards white, do not ask me about the colour space.
    /// </summary>
    public static Color Lighten(this Color color, float amount)
    {
        return new Color((color.R / 255.0f) * (1.0f + amount), (color.G / 255.0f) * (1.0f + amount), (color.B / 255.0f) * (1.0f + amount), color.A);
    }
}