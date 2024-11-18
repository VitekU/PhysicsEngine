using System.Numerics;

namespace engine
{
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
        

        public void Step(float delta)
        {
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
                                    if (Collisions.CircleCollision(circleA, circleB, out Vector2 normal))
                                    {
                                        Collisions.ResolveCollision(circleA, circleB, normal);
                                    }
                                }
                                else if (b is Rectangle2D rectangleB)
                                {
                                    if (Collisions.CircleRectangleCollision(circleA, rectangleB, out Vector2 normal))
                                    {
                                        Collisions.ResolveCollision(circleA, rectangleB, normal);
                                    }
                                }
                            }
                            else if (a is Rectangle2D rectangleA)
                            {
                                if (b is Circle2D circleB)
                                {
                                    if (Collisions.CircleRectangleCollision(circleB, rectangleA, out Vector2 normal))
                                    {
                                        Collisions.ResolveCollision(rectangleA, circleB, normal);;
                                    }
                                }
                                else if (b is Rectangle2D rectangleB)
                                {
                                    if (Collisions.RectangleRectangleCollision(rectangleA, rectangleB, out Vector2 normal))
                                    {
                                        Collisions.ResolveCollision(rectangleA, rectangleB, normal);
                                    }
                                }
                            }
                        }
                    }
                
                    foreach (var body in _bodies)
                    {
                        if (!body.IsStatic)
                        {
                            body.ApplyGravity(new Vector2(0, 100f));
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
        public int BodyCount()
        {
            return _bodies.Count;
        }
        
        public bool AddCircle(Vector2 position, float restitution, float mass, bool isStatic, float radius, float angle)
        {
            if (radius <= _maxRadius && mass <= _maxMass && restitution <= _maxRestitution)
            {
                _bodies.Add(new Circle2D(position, restitution, mass, isStatic, angle, radius));
                return true;
            }
            return false;
        }

        public bool AddRectangle(Vector2 position, float restitution, float mass, bool iSstatic, float width, float height, float angle)
        {
            if (width <= _maxWidth && height <= _maxHeight && mass <= _maxMass && restitution <= _maxRestitution)
            {
                _bodies.Add(new Rectangle2D(position, restitution, mass, iSstatic, angle, width, height));
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

            _bodies = new();
        }
    }
}