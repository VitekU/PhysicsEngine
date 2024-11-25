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
    public float RotationalInertia { get; protected set; }
    public float InverseRotationalInertia { get; protected set; }
    public float StaticFrictionC { get; set; }
    public float DynamicFrictionC { get; set; }
    
    
    protected virtual void UpdateVertices() {}
    protected abstract float CalculateRotationalInertia();
    
    public void ApplyGravity(Vector2 a)
    {
        Acceleration += a;
    }
    public void Move(Vector2 force)
    {
        if (!IsStatic)
        {
            Position += force;
        } 
    }
    
    public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
    {
        Velocity += impulse * InverseMass;
        float torque = VectorMathHelper.Cross(contactVector, impulse);
        AngularVelocity += torque * InverseRotationalInertia;
    }
    
    public void Step(float delta)
    {
        Velocity += Acceleration * delta;
        Position += Velocity * delta;
        
        
        Angle = (float)((Angle += AngularVelocity * delta) % (2 * Math.PI));
        UpdateVertices();
        
        
        if (VectorMathHelper.VectorCompare(Velocity, Vector2.Zero))
        {
            Velocity = Vector2.Zero;
        }
        
        Acceleration = Vector2.Zero;
    }

    protected RigidBody2(Vector2 position, float restitution, float mass, bool isStatic, float angle, float dk, float sk)
    {
        Position = position;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        AngularVelocity = 0;
        Restitution = restitution;
        Mass = mass;
        IsStatic = isStatic;
        Angle = angle;
        DynamicFrictionC = dk;
        StaticFrictionC = sk;
        
        if (isStatic)
        {
            Mass = float.MaxValue;
            InverseMass = 0;
        }
        else
        {
            InverseMass = 1 / mass;
        }
    }
}