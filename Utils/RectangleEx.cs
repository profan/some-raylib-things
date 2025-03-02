using System.Numerics;
using Raylib_cs;

namespace Utils;

public static class RectangleEx
{
    /// <summary>
    /// Returns the rectangle translated by the given offset, useful if you have a rectangle for local bounds, but sometimes need it transformed.
    /// </summary>
    public static Rectangle Offset(this Rectangle rect, Vector2 offset)
    {
        return new Rectangle(rect.X + offset.X, rect.Y + offset.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Returns the rectangle rotated by the given angle, useful if you have a rectangle for local bounds, but sometimes need it transformed.
    /// </summary>
    public static Rectangle Rotated(this Rectangle rect, float angle)
    {
        return new Rectangle(rect.Position.Rotated(angle), rect.Size.Rotated(angle)); ;
    }

    /// <summary>
    /// Returns the center of the rectangle.
    /// </summary>
    public static Vector2 Center(this Rectangle rect)
    {
        return new Vector2(rect.X + (rect.Width * 0.5f), rect.Y + (rect.Height * 0.5f));
    }

    /// <summary>
    /// Returns the radius of the bounding circle of the given rectangle.
    /// </summary>
    public static float Radius(this Rectangle rect)
    {
        float maxDimension = Math.Max(rect.Width, rect.Height);
        return maxDimension / 2.0f;
    }

    public static Vector2 Min(this Rectangle rect)
    {
        return new Vector2(rect.X, rect.Y);
    }

    public static Vector2 Max(this Rectangle rect)
    {
        return new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
    }
}