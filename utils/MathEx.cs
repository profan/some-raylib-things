using System.Numerics;
using Raylib_cs;

namespace utils;

public static class MathEx
{
    /// <summary>
    /// Returns the angle represented as a normalized direction vector.
    /// </summary>>
    public static Vector2 AsVector(this float angle)
    {
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    /// <summary>
    /// Returns the vector (assumed to be a direction vector) represented as an angle in radians.
    /// </summary>
    public static float AsAngle(this Vector2 vector)
    {
        return MathF.Atan2(vector.Y, vector.X);
    }

    /// <summary>
    /// Returns the ... clockwise? perpendicular vector of this vector.
    /// </summary>
    public static Vector2 Perpendicular(this Vector2 vector)
    {
        return new Vector2(vector.Y, -vector.X);
    }
    
    /// <summary>
    /// Returns the given angle in radians mapped to the range [-PI, PI].
    /// </summary>
    public static float MapAngle(float r)
    {
        return r - (MathF.PI * 2.0f) * MathF.Floor((r + MathF.PI) * (1.0f / (MathF.PI * 2.0f)));
    }

    /// <summary>
    /// Returns the given vector rotated by the specific angle, with the world origin as the pivot.
    /// </summary>
    public static Vector2 Rotated(this Vector2 vector, float angle)
    {
        return new Vector2(
            vector.X * MathF.Cos(angle) - vector.Y * MathF.Sin(angle),
            vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle));
    }

    /// <summary>
    /// Returns the given vector rotated by the specific angle, with the specific origin as the pivot.
    /// </summary>
    public static Vector2 RotatedAroundOrigin(this Vector2 vector, float angle, Vector2 origin)
    {
        Vector2 originTranslated = vector - origin;
        Vector2 rotatedTranslated = originTranslated.Rotated(angle);
        return rotatedTranslated + origin;
    }

    /// <summary>
    /// Returns the given angle in radians, in degrees.
    /// </summary>
    public static float Rad2Deg(this float radians)
    {
        return radians / MathF.PI * 180.0f;
    }

    /// <summary>
    /// Returns the given angle in degrees, in radians.
    /// </summary>
    public static float Deg2Rad(this float degrees)
    {
        return degrees * MathF.PI / 180.0f;
    }

    /// <summary>
    /// Returns the interpolated value between from and to, at position t.
    /// </summary>
    public static float Lerp(this float from, float to, float t)
    {
        return from + (to - from) * t;
    }

    /// <summary>
    /// Returns true if the given point is inside the bounds defined by min/max.
    /// </summary>
    public static bool IsInsideBounds(this Vector2 vector, Vector2 min, Vector2 max)
    {
        min = Vector2.Min(min, max);
        max = Vector2.Max(min, max);
        return Raylib.CheckCollisionPointRec(vector, new Rectangle(min, max - min));
    }
}