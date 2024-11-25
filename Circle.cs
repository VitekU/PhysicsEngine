using System.Numerics;

namespace engine;

public sealed class Circle2D : RigidBody2
{
    public readonly float Radius;
    public Vector2 PointForRotation;

    protected override void UpdateVertices()
    {
        PointForRotation = Position + new Vector2(Radius, 0);
        PointForRotation = RotatePoint(PointForRotation, Position, Angle);
    }
    
    private Vector2 RotatePoint(Vector2 point, Vector2 origin, float angle)
    {
        Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(angle, origin);
        return Vector2.Transform(point, rotationMatrix);
    }
    public Circle2D(Vector2 position, float restitution, float mass, bool isStatic,float angle, float dk, float sk, float radius) : base(position, restitution, mass, isStatic, angle, dk, sk)
    {
        Radius = radius;
        RotationalInertia = CalculateRotationalInertia();
        PointForRotation = position + new Vector2(radius, 0);
        if (isStatic)
        {
            InverseRotationalInertia = 0;
        }
        else
        {
            InverseRotationalInertia = 1 / RotationalInertia;
        }
    }
    protected override float CalculateRotationalInertia()
    {
        return (float)(Mass * Math.Pow(Radius, 2) / 2);
    }
}