using System.Numerics;


namespace engine;

public class Collisions
{
    private  readonly Vector2[] _contacts = new Vector2[2];
    private  readonly Vector2[] _impulses = new Vector2[2];
    private  readonly Vector2[] _frictionImpulses = new Vector2[2];
    private  readonly Vector2[] _raList = new Vector2[2];
    private  readonly Vector2[] _rbList = new Vector2[2];
    private  readonly float[] _jList = new float[2];
    private  float _e;
    private  float _dk;
    private  float _sk;
    
    public  bool CircleCollision(Circle2D a, Circle2D b, out Vector2 n, out Vector2 cp)
    {
        float distance = Vector2.Distance(a.Position, b.Position);
        float radii = a.Radius + b.Radius;
        cp = Vector2.Zero;
        if (distance < radii)
        {
            Vector2 normal = Vector2.Normalize(b.Position - a.Position);
            float depth = radii - distance;
            
            a.Move(-normal * depth / 2f);
            b.Move(normal * depth / 2f);
            
            n = normal;
            ContactPoints.CircleCircleContact(a, b, out cp);
            return true;
        }
        
        n = Vector2.Zero;
        return false;
    }

    public  bool CircleRectangleCollision(Circle2D circle, Rectangle2D rectangle, out Vector2 normal, out Vector2 cp)
    {
        normal = Vector2.Zero;
        cp = Vector2.Zero;
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
            separatingAxis = Vector2.Normalize(separatingAxis);
            
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
        separatingAxis = Vector2.Normalize(separatingAxis);
        
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
        
        Vector2 direction = Vector2.Normalize(rectangle.Position - circle.Position);
        
        

        if (Vector2.Dot(direction, normal) < 0f)
        {
            normal *= -1;
        }
        ContactPoints.CircleRectangleContact(circle, rectangle, out Vector2 contactPoint);
        cp = contactPoint;
        
        circle.Move(-normal * depth / 2f);
        rectangle.Move(normal * depth / 2f);
        return true;
    }

    public  bool RectangleRectangleCollision(Rectangle2D rectA, Rectangle2D rectB, out Vector2 normal, out Vector2 cp1, out Vector2 cp2, out int n)
    {
        cp1 = Vector2.Zero;
        cp2 = Vector2.Zero;
        n = 0;
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
            separatingAxis = Vector2.Normalize(separatingAxis);
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
            separatingAxis = Vector2.Normalize(separatingAxis);
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
            
        Vector2 direction = Vector2.Normalize(rectA.Position - rectB.Position);
        
        if (Vector2.Dot(direction, normal) < 0f)
        {
            normal *= -1;
        }

        ContactPoints.RectangleRectangleContact(rectA, rectB, out cp1, out cp2, out n);
        rectA.Move(normal * depth / 2f);
        rectB.Move(-normal * depth / 2f);
        return true;
    } 
    
    // separatin axis theorem => projectcircle a projectrectangle projektuji body na osu, univerzalne lze pouit i pro rect rect kolizi nebo poly poly kolizi
    private  void ProjectCircle(Circle2D circle, Vector2 axis, out float min, out float max)
    {
        Vector2 axisDirection = Vector2.Normalize(axis);
        Vector2 p1 = circle.Position + axisDirection * circle.Radius;
        Vector2 p2 = circle.Position - axisDirection * circle.Radius;
        
        min = Math.Min(Vector2.Dot(p1, axis), Vector2.Dot(p2, axis));
        max = Math.Max(Vector2.Dot(p1, axis), Vector2.Dot(p2, axis));
    }
    
    private  void ProjectRectangle(Rectangle2D rectangle, Vector2 axis, out float min, out float max)
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
    
    public void ResolveCollision(RigidBody2 a, RigidBody2 b, Vector2 normal, Vector2 cp1, Vector2 cp2, int cpCount)
    {
        if (a.IsStatic && b.IsStatic)
        {
            return;
        }

        _contacts[0] = cp1;
        _contacts[1] = cp2;
        
        _e = Math.Min(a.Restitution, b.Restitution);
        _dk = (a.DynamicFrictionC + b.DynamicFrictionC) / 2f;
        _sk = (a.StaticFrictionC + b.StaticFrictionC) / 2f;
        
        
        for (int i = 0; i < cpCount; i++)
        {
            Vector2 ra = _contacts[i] - a.Position;
            Vector2 rb = _contacts[i] - b.Position;

            Vector2 rvVec = b.Velocity + VectorMathHelper.Cross(b.AngularVelocity, rb) - (a.Velocity + VectorMathHelper.Cross(a.AngularVelocity, ra));

            float relativeVel = Vector2.Dot(rvVec, normal);
            
            float raCrossN = VectorMathHelper.Cross(ra, normal);
            float rbCrossN = VectorMathHelper.Cross(rb, normal);
            float denominator = a.InverseMass + b.InverseMass + raCrossN * raCrossN * a.InverseRotationalInertia + rbCrossN * rbCrossN * b.InverseRotationalInertia;

            float j = -(1 + _e) * relativeVel;
            j /= (denominator * cpCount);

            Vector2 impulse = normal * j;
            _jList[i] = j;
            _impulses[i] = impulse;
            _raList[i] = ra;
            _rbList[i] = rb;
        }
        for (int i = 0; i < cpCount; i++)
        {
            Vector2 impulse = _impulses[i];
            Vector2 ra = _raList[i];
            Vector2 rb = _rbList[i];
            a.ApplyImpulse(-impulse, ra);
            b.ApplyImpulse(impulse, rb );
        }
        
        
        // friction
        
        for (int i = 0; i < cpCount; i++)
        {
            Vector2 ra = _contacts[i] - a.Position;
            Vector2 rb = _contacts[i] - b.Position;

            Vector2 rvVec = b.Velocity + VectorMathHelper.Cross(b.AngularVelocity, rb) - (a.Velocity + VectorMathHelper.Cross(a.AngularVelocity, ra));

            Vector2 tangent = rvVec - (normal * Vector2.Dot(rvVec, normal));
            
            if (VectorMathHelper.VectorCompare(tangent, Vector2.Zero))
            {
                continue;
            }

            tangent = -Vector2.Normalize(tangent);
            
            float raCrossT = VectorMathHelper.Cross(ra, tangent);
            float rbCrossT = VectorMathHelper.Cross(rb, tangent);
            float denominator = a.InverseMass + b.InverseMass + raCrossT * raCrossT * a.InverseRotationalInertia + rbCrossT * rbCrossT * b.InverseRotationalInertia;
            
            float relativeVel = Vector2.Dot(rvVec, tangent);
            float jt = -relativeVel;
            jt /= (denominator * cpCount);
            float j = _jList[i];
            
            Vector2 frictionImpulse;
            
            if (Math.Abs(jt) <= j * _sk)
            {
                frictionImpulse = jt * tangent;
            }
            else
            {
                frictionImpulse = -j * tangent * _dk;
            }
            
            if (VectorMathHelper.VectorCompare(frictionImpulse, Vector2.Zero))
            {
                return;
            }
            _frictionImpulses[i] = frictionImpulse;
            _raList[i] = ra;
            _rbList[i] = rb;
        }
        for (int i = 0; i < cpCount; i++)
        {
            Vector2 frictionImpulse = _frictionImpulses[i];
            Vector2 ra = _raList[i];
            Vector2 rb = _rbList[i];
            a.ApplyImpulse(-frictionImpulse, ra);
            b.ApplyImpulse(frictionImpulse, rb);
        }
    }
}