using System.Numerics;

namespace engine;

public class Circle2D : RigidBody2
{
    public readonly float Radius;
    
    public Circle2D(Vector2 position, float restitution, float mass, bool isStatic,float angle, float radius) : base(position, restitution, mass, isStatic, angle)
    {
        Radius = radius;
    }
}