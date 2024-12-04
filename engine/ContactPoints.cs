using System.Numerics;

namespace engine;

public abstract class ContactPoints
{
    public static void CircleCircleContact(Circle2D a, Circle2D b, out Vector2 contactPoint)
    {
        Vector2 centersDirection = Vector2.Normalize(b.Position - a.Position);
        contactPoint = a.Position + centersDirection * a.Radius;
    }

    public static void CircleRectangleContact(Circle2D circle, Rectangle2D rect, out Vector2 contactPoint)
    {
        contactPoint = Vector2.Zero;
        float smallestDistance = float.MaxValue;
        
        for (int i = 0; i < rect.Vertices.Length; i++)
        {
            Vector2 a = rect.Vertices[i];
            Vector2 b = rect.Vertices[(i + 1) % rect.Vertices.Length];

            Vector2 cp = ClosestPointOnLineSeg(a, b, circle.Position);

            float d = Vector2.Distance(cp, circle.Position);
            if (d <= smallestDistance)
            {
                smallestDistance = d;
                contactPoint = cp;
            }
        }
    }

    public static void RectangleRectangleContact(Rectangle2D rectA, Rectangle2D rectB, out Vector2 cp1, out Vector2 cp2, out int n)
    {
        n = 0;
        cp1 = Vector2.Zero;
        cp2 = Vector2.Zero;
        float minDistance = float.MaxValue;

        for (int i = 0; i < rectA.Vertices.Length; i++)
        {
            Vector2 point = rectA.Vertices[i];
            for (int j = 0; j < rectB.Vertices.Length; j++)
            {
                Vector2 a = rectB.Vertices[j];
                Vector2 b = rectB.Vertices[(j + 1) % rectB.Vertices.Length];

                Vector2 cp = ClosestPointOnLineSeg(a, b, point);
                float d = Vector2.Distance(cp, point);
                //Console.WriteLine($"{cp} {point}");
                
                if (d < minDistance)
                {
                    cp1 = cp;
                    minDistance = d;
                    n = 1;
                } 
                else if (VectorMathHelper.FloatCompare(d, minDistance))
                {
                    cp2 = cp;
                    n = 2;
                }
            }
        }
        for (int i = 0; i < rectB.Vertices.Length; i++)
        {
            Vector2 point = rectB.Vertices[i];
            for (int j = 0; j < rectA.Vertices.Length; j++)
            {
                Vector2 a = rectA.Vertices[j];
                Vector2 b = rectA.Vertices[(j + 1) % rectA.Vertices.Length];

                Vector2 cp = ClosestPointOnLineSeg(a, b, point);
                float d = Vector2.Distance(cp, point);

                if (d < minDistance)
                {
                    cp1 = cp;
                    minDistance = d;
                    n = 1;
                }
                else if (VectorMathHelper.FloatCompare(d, minDistance))
                {
                    if (!VectorMathHelper.VectorCompare(cp, cp1))
                    {
                        cp2 = cp;
                        n = 2;
                    }
                }
            }
        }
    }

    private static Vector2 ClosestPointOnLineSeg(Vector2 a, Vector2 b, Vector2 point)
    {
        Vector2 ab = b - a;
        Vector2 ap = point - a;
        
        float projection = Vector2.Dot(ab, ap);
        float dist = projection / ab.LengthSquared();
        
        if (dist < 0)
        {
            return a;
        }
        if (dist > 1)
        {
            return b;
        }
        return a + ab * dist;
    }
}