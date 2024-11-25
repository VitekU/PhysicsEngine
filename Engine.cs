using System.Numerics;

namespace engine;

public class Engine
{
    private readonly List<RigidBody2> _bodies;
    private int _heightBoundary;
    private int _widthBoundary;
    private readonly float _maxRadius;
    private readonly float _maxMass;
    private readonly float _maxRestitution;
    private readonly float _maxWidth;
    private readonly float _maxHeight;
    private float _stepLenght;
    private readonly int _substeps = 16;
    private readonly Collisions _collisions;
    
    public List<Vector2> CPs { get; set; }
    public float Gravitation { get; set; }

    public void Step(float delta)
    {
        CPs.Clear(); 
        _stepLenght += delta;
        if (_stepLenght >= 1/60f)
        {
            for (int s = 0; s < _substeps; s++)
            {
                for (int i = 0; i < BodyCount(); i++)
                {
                    RigidBody2 a = _bodies[i];
                    
                    for (int j = i + 1; j < BodyCount(); j++)
                    {
                        RigidBody2 b = _bodies[j];
    
                        if (a is Circle2D circleA)
                        {
                            if (b is Circle2D circleB)
                            {
                                if (_collisions.CircleCollision(circleA, circleB, out Vector2 normal, out Vector2 contactPoint))
                                {
                                    CPs.Add(contactPoint);
                                    _collisions.ResolveCollision(circleA, circleB, normal, contactPoint, Vector2.Zero, 1);
                                }
                            }
                            else if (b is Rectangle2D rectangleB)
                            {
                                if (_collisions.CircleRectangleCollision(circleA, rectangleB, out Vector2 normal, out Vector2 contactPoint))
                                {
                                    CPs.Add(contactPoint);
                                    _collisions.ResolveCollision(circleA, rectangleB, normal, contactPoint, Vector2.Zero, 1);
                                }
                            }
                        }
                        else if (a is Rectangle2D rectangleA)
                        {
                            if (b is Circle2D circleB)
                            {
                                if (_collisions.CircleRectangleCollision(circleB, rectangleA, out Vector2 normal, out Vector2 contactPoint))
                                {
                                    CPs.Add(contactPoint);
                                    _collisions.ResolveCollision(rectangleA, circleB, normal,contactPoint, Vector2.Zero, 1);
                                }
                            }
                            else if (b is Rectangle2D rectangleB)
                            {
                                if (_collisions.RectangleRectangleCollision(rectangleA, rectangleB, out Vector2 normal, out Vector2 cp1, out Vector2 cp2, out int n))
                                {
                                    if (n == 1)
                                    {
                                        CPs.Add(cp1);
                                    }
                                    else
                                    {
                                        CPs.Add(cp1); 
                                        CPs.Add(cp2);
                                    }
                                    _collisions.ResolveCollision(rectangleA, rectangleB, normal, cp1, cp2, n);
                                }
                            }
                        }
                    }
                }
                
                foreach (var body in _bodies)
                {
                    if (!body.IsStatic)
                    {
                        body.ApplyGravity(new Vector2(0, Gravitation));
                    }
                        
                    body.Step(1 / 60f / _substeps);
                }
            }
            _stepLenght = 0;
        }
    }
        
    // managing bodies in the engine
    public List<RigidBody2> GetBodies()
    {
        return _bodies;
    }

    private int BodyCount()
    {
        return _bodies.Count;
    }
        
    public bool AddCircle(Vector2 position, float restitution, float mass, bool isStatic, float radius, float angle, float dk, float sk)
    {
        if (radius <= _maxRadius && mass <= _maxMass && restitution <= _maxRestitution && restitution > 0 && mass > 0 && radius > 0)
        {
            _bodies.Add(new Circle2D(position, restitution, mass, isStatic, angle, dk, sk, radius));
            return true;
        }
        return false;
    }

    public bool AddRectangle(Vector2 position, float restitution, float mass, bool iSstatic, float width, float height, float angle, float dk, float sk)
    {
        if (width <= _maxWidth && height <= _maxHeight && mass <= _maxMass && restitution <= _maxRestitution && restitution > 0 && mass > 0 && width > 0 && height > 0)
        {
            _bodies.Add(new Rectangle2D(position, restitution, mass, iSstatic, (float)(angle / 180 * Math.PI), dk, sk, width, height));
            return true;
        }
        return false;
    }
    public Engine(float maxRadius, float maxMass, float maxRestituion, float maxHeight, float maxWidth)
    {
        _maxRadius = maxRadius;
        _maxMass = maxMass;
        _maxRestitution = maxRestituion;
        _maxHeight = maxHeight;
        _maxWidth = maxWidth;
        Gravitation = 1000f;
        CPs = new List<Vector2>();
        _collisions = new Collisions();

        _bodies = new();
    }
}