using System.Numerics;

namespace engine;

public abstract class RigidBody2
{
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; set; }
    
    public float AngularVelocity { get; set; }
    
    public float Angle { get; private set; }
    public Vector2 Acceleration { get; private set; }
    public float Restitution { get; private set; }
    public float Mass { get; private set; }
    public float InverseMass { get; private set; }
    public bool IsStatic { get; private set; }
    
    protected virtual void UpdateVertices(float angleChange) {}

    public void ApplyGravity(Vector2 a)
    {
        Acceleration += a;
    }
    public void Impulse(Vector2 force)
    {
        Position += force * InverseMass;
    }
    
    public void Force(Vector2 force)
    {
        Acceleration += force * InverseMass;
    }
    public void Step(float delta)
    {
        Velocity += Acceleration * delta;
        Position += Velocity * delta;

        Angle = (Angle += AngularVelocity * delta) % 360;
        UpdateVertices(AngularVelocity * delta);
        
        Velocity *= 1f;

        if (Velocity.Length() < 0.1f)
        {
            Velocity = Vector2.Zero;
        }

        Acceleration = Vector2.Zero;
    }

    protected RigidBody2(Vector2 position, float restitution, float mass, bool isStatic, float angle)
    {
        Position = position;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        Restitution = restitution;
        Mass = mass;
        InverseMass = 1 / mass;
        IsStatic = isStatic;
        Angle = angle;
        
        
        if (isStatic)
        {
            Mass = float.MaxValue;
            InverseMass = 0;
        }
    }
    
}