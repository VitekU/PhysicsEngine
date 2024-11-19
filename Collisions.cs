using System.Numerics;


namespace engine;

public abstract class Collisions
{
    public static bool CircleCollision(Circle2D a, Circle2D b, out Vector2 n, out Vector2 cp)
    {
        float distance = Vector2.Distance(a.Position, b.Position);
        float radii = a.Radius + b.Radius;
            
        if (distance < radii)
        {
            Vector2 normal = Vector2.Normalize(b.Position - a.Position);
            float depth = radii - distance;
            
            a.Impulse(-normal * depth / 2f);
            b.Impulse(normal * depth / 2f);
            
            n = normal;
            ContactPoints.CircleCircleContact(a, b, out cp);
            return true;
        }
        
        n = Vector2.Zero;
        cp = Vector2.Zero;
        return false;
    }

    public static bool CircleRectangleCollision(Circle2D circle, Rectangle2D rectangle, out Vector2 normal, out Vector2 _cp)
    {
        normal = Vector2.Zero;
        _cp = Vector2.Zero;
        Vector2 closestPoint = rectangle.Vertices[0];
        Vector2 separatingAxis;
        float minA;
        float maxA;
        float minB;
        float maxB;
        
        float depth = float.MaxValue;
        float currentDepth;
        
        //<3
        for (int i = 0; i < rectangle.Vertices.Length; i++)
        {
            if (Vector2.DistanceSquared(closestPoint, circle.Position) >
                Vector2.DistanceSquared(rectangle.Vertices[i], circle.Position))
            {
                closestPoint = rectangle.Vertices[i];
            }
            Vector2 a = rectangle.Vertices[i];
            Vector2 b = rectangle.Vertices[(i + 1) % rectangle.Vertices.Length];
            Vector2 edge = b - a;
            
            separatingAxis = new Vector2(-edge.Y, edge.X);
            
            ProjectRectangle(rectangle, separatingAxis, out minA, out  maxA);
            ProjectCircle(circle, separatingAxis, out  minB, out  maxB);
            
            if (minB > maxA || minA > maxB)
            {
                return false;
            }

            currentDepth = Math.Min(maxA - minB, maxB - minA);

            if (currentDepth < depth)
            {
                depth = currentDepth;
                normal = separatingAxis;
            }
        }

        separatingAxis = circle.Position - closestPoint;
        
        ProjectRectangle(rectangle, separatingAxis, out minA, out  maxA);
        ProjectCircle(circle, separatingAxis, out  minB, out  maxB);
        
        if (minB > maxA || minA > maxB)
        {
            return false;
        }
        
        currentDepth = Math.Min(maxA - minB, maxB - minA);
        if (currentDepth < depth)
        {
            depth = currentDepth;
            normal = separatingAxis;
        }
        
        
        depth /= normal.Length();
        normal = Vector2.Normalize(normal);

       

        Vector2 direction = Vector2.Normalize(rectangle.Position - circle.Position);
        
        //Console.WriteLine($"{depth} {normal} {direction}");

        if (Vector2.Dot(direction, normal) < 0f)
        {
            normal *= -1;
        }
        ContactPoints.CircleRectangleContact(circle, rectangle, out Vector2 cp);
        _cp = cp;
        circle.Impulse(-normal * depth / 2f);
        rectangle.Impulse(normal * depth / 2f);
        return true;
    }

    public static bool RectangleRectangleCollision(Rectangle2D rectA, Rectangle2D rectB, out Vector2 normal)
    {
        normal = Vector2.Zero;
        Vector2 separatingAxis;
        float minA;
        float maxA;
        float minB;
        float maxB;
        float depth = float.MaxValue;
        float currentDepth;

        for (int i = 0; i < rectA.Vertices.Length; i++)
        {
            Vector2 a = rectA.Vertices[i];
            Vector2 b = rectA.Vertices[(i + 1) % rectA.Vertices.Length];
            Vector2 edge = b - a;
            separatingAxis = new Vector2(-edge.Y, edge.X);
            ProjectRectangle(rectA, separatingAxis, out minA, out maxA);
            ProjectRectangle(rectB, separatingAxis, out minB, out maxB);
            
            if (minB > maxA || minA > maxB)
            {
                return false;
            }

            currentDepth = Math.Min(maxA - minB, maxB - minA);

            if (currentDepth < depth)
            {
                depth = currentDepth;
                normal = separatingAxis;
            }
        }
        
        for (int i = 0; i < rectB.Vertices.Length; i++)
        {
            Vector2 a = rectB.Vertices[i];
            Vector2 b = rectB.Vertices[(i + 1) % rectB.Vertices.Length];
            Vector2 edge = b - a;
            separatingAxis = new Vector2(-edge.Y, edge.X);
            ProjectRectangle(rectA, separatingAxis, out minA, out maxA);
            ProjectRectangle(rectB, separatingAxis, out minB, out maxB);
            
            if (minB > maxA || minA > maxB)
            {
                return false;
            }

            currentDepth = Math.Min(maxA - minB, maxB - minA);

            if (currentDepth < depth)
            {
                depth = currentDepth;
                normal = separatingAxis;
            }
        }
        
        depth /= normal.Length();
        normal = Vector2.Normalize(normal);
            
        Vector2 direction = Vector2.Normalize(rectA.Position - rectB.Position);
        
        if (Vector2.Dot(direction, normal) < 0f)
        {
            normal *= -1;
        }

        
        rectA.Impulse(normal * depth / 2f);
        rectB.Impulse(-normal * depth / 2f);
        return true;
    } 
    
    // separatin axis theorem => projectcircle a projectrectangle projektuji body na osu, univerzalne lze pouit i pro rect rect kolizi nebo poly poly kolizi
    private static void ProjectCircle(Circle2D circle, Vector2 axis, out float min, out float max)
    {
        Vector2 axisDirection = Vector2.Normalize(axis);
        Vector2 p1 = circle.Position + axisDirection * circle.Radius;
        Vector2 p2 = circle.Position - axisDirection * circle.Radius;
        
        min = Math.Min(Vector2.Dot(p1, axis), Vector2.Dot(p2, axis));
        max = Math.Max(Vector2.Dot(p1, axis), Vector2.Dot(p2, axis));
    }
    
    private static void ProjectRectangle(Rectangle2D rectangle, Vector2 axis, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;
        
        foreach (var p in rectangle.Vertices)
        {
            float indexOfAlignment = Vector2.Dot(p, axis);
            min = Math.Min(indexOfAlignment, min);
            max = Math.Max(indexOfAlignment, max);
        }
    }
    
    
    public static void ResolveCollision(RigidBody2 a, RigidBody2 b, Vector2 normal)
    {
        Vector2 relativeVelocity = b.Velocity - a.Velocity;
        
        float e = Math.Min(a.Restitution, b.Restitution);

        float j = -(1 + e) * Vector2.Dot(relativeVelocity, normal) / (a.InverseMass + b.InverseMass);

        a.Velocity -= j * a.InverseMass * normal;
        b.Velocity += j * b.InverseMass * normal;
    }
}