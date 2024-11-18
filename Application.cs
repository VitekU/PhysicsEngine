using System.Numerics;
using Raylib_cs;

namespace engine;

public class Application
{
    private bool _run;
    private RigidBody2 _movingBody;
    private Engine _engine;
    private bool _isSpacePressed;
    private int _fps;
    private Camera2D _camera;
    private int k = 10;

    public void Start()
    {
        Raylib.InitWindow(800,600, "engine");
        _engine.AddRectangle(new Vector2(40f, 20f), 0.6f, 2f, false, 10f, 10f, 0f);

        _engine.AddRectangle(new Vector2(40f, 50f), 1f, 1f, true, 70f, 4f, 0f);
        //_engine.AddCircle(new Vector2(15f, 15f), 0.8f, 2f, false, 3f, 10f);
        
        _movingBody = _engine.GetBodies()[0];
        _run = true;
        Raylib.SetTargetFPS(60);
        ApplicationLoop();
    }
    
    private void Render(List<RigidBody2> bodies)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.BeginMode2D(_camera);
        
        foreach (var body in bodies)
        {
            if (body is Circle2D circle)
            {
                Raylib.DrawCircle((int)(circle.Position.X * k), (int)(circle.Position.Y * k), circle.Radius * k, Color.Red);
            }
            if (body is Rectangle2D rectangle)
            {
                Rectangle rect = new Rectangle(rectangle.Position * k, rectangle.Width * k, rectangle.Height * k);
                Vector2 origin = new Vector2(rectangle.Width / 2f * k, rectangle.Height / 2f * k);
                Raylib.DrawRectanglePro(rect, origin, rectangle.Angle, rectangle.Color);
                
            }
        }
        Raylib.EndMode2D();
        Raylib.DrawText(Convert.ToString(_fps), 5, 0, 30, Color.Black);
        Raylib.DrawText(Convert.ToString(_engine.GetBodies().Count), 5, 40, 30, Color.Black);
        Raylib.EndDrawing();
    }

    private void PauseOrResume()
    {
        if (Raylib.IsKeyDown(KeyboardKey.Space))
        {
            if (!_isSpacePressed)
            {
                _run = !_run;
                _isSpacePressed = true;
            }
        }
        else
        {
            _isSpacePressed = false;
        }
    }

    private void TrySpawnBodies()
    {
        Vector2 mousePos = Raylib.GetMousePosition() / 10f;
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            _engine.AddCircle(mousePos, 0.9f, 1f, false, 3f, 0);
        }
        else if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            _engine.AddRectangle(mousePos, 0.9f, 2f, false, 10f, 10f, 0);
        }
    }

    private void TryZoom()
    {
        if (Raylib.IsKeyDown(KeyboardKey.Up))
        {
            if (_camera.Zoom <= 2)
            {
                _camera.Zoom += 0.01f; 
            }
        }
        else if (Raylib.IsKeyDown(KeyboardKey.Down))
        {
            if (_camera.Zoom >= 0.1)
            {
                _camera.Zoom -= 0.01f;
            }
        }
    }
    

    private void ApplicationLoop()
    {
        while (!Raylib.WindowShouldClose())
        {
            _fps = Raylib.GetFPS();
            PauseOrResume();
            if (_run)
            {

                // move the movable body
                Vector2 directionOfMovement = Vector2.Zero;
                if (Raylib.IsKeyDown(KeyboardKey.A))
                {
                    directionOfMovement.X--;
                }
                if (Raylib.IsKeyDown(KeyboardKey.D))
                {
                    directionOfMovement.X++;
                }
                if (Raylib.IsKeyDown(KeyboardKey.W))
                {
                    directionOfMovement.Y--;
                }
                if (Raylib.IsKeyDown(KeyboardKey.S))
                {
                    directionOfMovement.Y++;
                }
                
                // zoom 
                TryZoom();
                
                // add bodies with a click of a mouse
                TrySpawnBodies();
                
                _movingBody.Force(directionOfMovement * 50);
                
                _engine.Step(Raylib.GetFrameTime());
            }
            Render(_engine.GetBodies());
        }
        Raylib.CloseWindow();
    }

    public Application(Engine engine)
    {
        _engine = engine;
        _run = false;
        _camera = new Camera2D();
        _camera.Target = new Vector2(400,300);
        _camera.Offset = new Vector2(400,300);
        _camera.Zoom = 1f;
        _fps = Raylib.GetFPS();
    }
}