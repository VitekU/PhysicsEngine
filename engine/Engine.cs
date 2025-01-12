using System.Numerics;

namespace engine;

public class Engine
{
    private readonly List<RigidBody2> _bodies;
    private static int _heightBoundary;
    private static int _widthBoundary;
    private static int _widthZeroBoundary;
    private readonly float _maxRadius;
    private readonly float _maxMass;
    private readonly float _maxRestitution;
    private readonly float _maxWidth;
    private readonly float _maxHeight;
    public float Gravitation { get; set; }
    private float _stepLenght;
    private readonly int _substeps = 64;
    private readonly Collisions _collisions;
    public List<Vector2> CPs { get; set; }
    public bool TryHold { get; set; }
    public Vector2 MousePos { get; set; }
    

    public void Step(float delta)
    {
        CPs.Clear(); 
        _stepLenght += delta;
        if (_stepLenght >= 1/60f)
        {
            RemoveOutBodies();
            for (int s = 0; s < _substeps; s++)
            {
                for (int i = 0; i < _bodies.Count; i++)
                {
                    RigidBody2 a = _bodies[i];
                    
                    for (int j = i + 1; j < _bodies.Count; j++)
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
                        body.ApplyForce(new Vector2(0, Gravitation));
                    }
                    HoldLogic(body);
                    
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

    private void HoldLogic(RigidBody2 body)
    {
        if (body.IsStatic)
        {
            return;
            
        }
        if (TryHold)
        {
            if (Vector2.DistanceSquared(MousePos, body.Position) < 2500)
            {
                body.IsHeld = true;
            }
        }
        else
        {
            body.IsHeld = false;
        }
        
        if (body.IsHeld)
        {
            body.ApplyForce((MousePos - body.Position) * 30f);
            body.ApplyForce(-1.5f * body.Velocity);
        }
    }
    public void EditBoundaries(Vector2 w, Vector2 h, Vector2 z)
    {
        _widthBoundary = (int)w.X;
        _heightBoundary = (int)h.X;
        _widthZeroBoundary = (int)z.X;
    }

    private static bool IsOutsideBounds(RigidBody2 body)
    {
        if (body.IsStatic)
        {
            return false;
        }
        // check for objects being outside of the boundaries (except for the upper boundary)
        if (body is Circle2D circle)
        {
            if (circle.Position.X - circle.Radius - 20 > _widthBoundary)
            {
                return true;
            }

            if (circle.Position.X + circle.Radius + 20 < _widthZeroBoundary)
            {
                return true;
            }

            if (circle.Position.Y - circle.Radius - 20 > _heightBoundary)
            {
                return true;
            }
            
            return false;
        }
        
        if (body is Rectangle2D rect)
        {
            if (rect.Position.X - rect.Width / 2f - 20 > _widthBoundary)
            {
                return true;
            }
            
            if (rect.Position.X + rect.Width / 2f + 20 < _widthZeroBoundary)
            {
                return true;
            }

            if (rect.Position.Y - rect.Height / 2f - 20 > _heightBoundary)
            {
                return true;
            }
            return false;
        }

        return false;
    } 
    private void RemoveOutBodies()
    {
        _bodies.RemoveAll(IsOutsideBounds);
    }

    public void RemoveAllBodies()
    {
        _bodies.Clear();
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
        _heightBoundary = 1000;
        _widthBoundary = 1600;
        _widthZeroBoundary = 0;
        TryHold = false;
        MousePos = Vector2.Zero;

        _bodies = new List<RigidBody2>();
    }
}