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

            Vector2 ab = b - a;
            Vector2 ax = circle.Position - a;

            float projection = Vector2.Dot(ab, ax);
            float dist = projection / ab.LengthSquared();

            Vector2 cp;
            if (dist < 0)
            {
                cp = a;
            }
            else if (dist > 1)
            {
                cp = b;
            }
            else
            {
                cp = a + ab * dist;
            }

            float d = Vector2.Distance(cp, circle.Position);
            if (d <= smallestDistance)
            {
                smallestDistance = d;
                contactPoint = cp;
            }
        }
    }
}