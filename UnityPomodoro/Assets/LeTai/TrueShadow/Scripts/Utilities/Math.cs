using UnityEngine;
using static UnityEngine.Mathf;

namespace LeTai.TrueShadow
{
public static class Math
{
    public static float Angle360(Vector2 from, Vector2 to)
    {
        float angle = Vector2.SignedAngle(from, to);
        return angle < 0 ? 360 + angle : angle;
    }

    public static Vector2 AngleDistanceVector(float angle, float distance, Vector2 zeroVector)
    {
        return Quaternion.Euler(0, 0, -angle) * zeroVector * distance;
    }

    public static Vector2 Rotate(this Vector2 v, float angle)
    {
        var rad = angle * Deg2Rad;
        var s   = Sin(rad);
        var c   = Cos(rad);
        return new Vector2(c * v.x - s * v.y,
                           s * v.x + c * v.y);
    }
}
}
