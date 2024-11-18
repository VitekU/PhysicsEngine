using System.Numerics;
using Color = Raylib_cs.Color;

namespace engine;

public class Rectangle2D : RigidBody2
{ 
    public Vector2[] Vertices { get; private set; }
    public float Height { get; private set; }
    public float Width { get; private set; }
    
    public Raylib_cs.Color Color = Color.Blue;


    protected override void UpdateVertices(float angleChange)
    {
        LoadVertices(Position, Height, Width, Angle);
    }
    
    private Vector2 RotatePoint(Vector2 point, Vector2 origin, float angle)
    {
        Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation((float)(angle * Math.PI / 180), origin);
        return Vector2.Transform(point, rotationMatrix);
    }
    private void LoadVertices(Vector2 center, float height, float width, float angle)
    {
        Vector2 v1 = new Vector2(center.X + width / 2, center.Y + height / 2);
        Vector2 v2 = new Vector2(center.X + width / 2, center.Y - height / 2);
        Vector2 v3 = new Vector2(center.X - width / 2, center.Y - height / 2);
        Vector2 v4 = new Vector2(center.X - width / 2, center.Y + height / 2);

        Vertices = new[]
        {
            RotatePoint(v1, center, angle), RotatePoint(v2, center, angle), RotatePoint(v3, center, angle),
            RotatePoint(v4, center, angle)
        };

    }
    public Rectangle2D(Vector2 position, float restitution, float mass, bool isStatic,float angle, float width, float height) : base(position, restitution, mass, isStatic, angle)
    {
        Height = height;
        Width = width;
        Vertices = Array.Empty<Vector2>();
        LoadVertices(position, height, width, angle);
    }
}