using System.Numerics;

namespace engine;

public abstract class VectorMathHelper
{
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }
    public static Vector2 Cross(float scalar, Vector2 v)
    {
        return new Vector2(-scalar * v.Y, scalar * v.X);
    }
    public static bool FloatCompare(float x, float y)
    {
        return Math.Abs(x - y) < 0.05f;
    }
    public static bool VectorCompare(Vector2 v, Vector2 u)
    {
        return FloatCompare(v.X, u.X) && FloatCompare(v.Y, u.Y);
    } 
}