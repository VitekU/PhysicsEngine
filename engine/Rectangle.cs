using System.Numerics;

namespace engine;

public sealed class Rectangle2D : RigidBody2
{ 
    public Vector2[] Vertices { get; private set; }
    public float Height { get; private set; }
    public float Width { get; private set; }
    

    protected override void UpdateVertices()
    {
        LoadVertices(Position, Height, Width, Angle);
    }
    
    private Vector2 RotatePoint(Vector2 point, Vector2 origin, float angle)
    {
        Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(angle, origin);
        return Vector2.Transform(point, rotationMatrix);
    }
    private void LoadVertices(Vector2 center, float height, float width, float angle)
    {
        Vector2 v1 = new Vector2(center.X + width / 2, center.Y - height / 2);
        Vector2 v2 = new Vector2(center.X + width / 2, center.Y + height / 2);
        Vector2 v3 = new Vector2(center.X - width / 2, center.Y + height / 2);
        Vector2 v4 = new Vector2(center.X - width / 2, center.Y - height / 2);
        
        Vertices[0] = RotatePoint(v1, center, angle);
        Vertices[1] = RotatePoint(v2, center, angle);
        Vertices[2] = RotatePoint(v3, center, angle);
        Vertices[3] = RotatePoint(v4, center, angle);
    }
    protected override float CalculateRotationalInertia()
    {
        return (float)(Mass * (Math.Pow(Height, 2) + Math.Pow(Width, 2)) / 12f);
    }
    
    public Rectangle2D(Vector2 position, float restitution, float mass, bool isStatic, float angle, float dk, float sk, float width, float height) : base(position, restitution, mass, isStatic, angle, dk, sk)
    {
        Height = height;
        Width = width;
        Vertices = new Vector2[4];
        LoadVertices(position, height, width, angle);
        RotationalInertia = CalculateRotationalInertia();
        if (isStatic)
        {
            InverseRotationalInertia = 0;
        }
        else
        {
            InverseRotationalInertia = 1 / RotationalInertia;
        }
    }

    
}