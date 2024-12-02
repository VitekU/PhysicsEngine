using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace engine;

public class Application
{
    private bool _run;
    private readonly Engine _engine;
    private int _fps;
    private Camera2D _camera;
    private bool _iSGuiVisible;
    private bool _iSFpsVisible;
    private bool _iSBodyCountVisible;
    private Vector2 _debugWindowLocation;
    private float _debugWindowHeight;
    private float _debugWindowWidth;
    private readonly Color _lineColor;
    private readonly Color _bgColor;
    private int _currentScreenWidth;
    private int _currentScreenHeight;
    private readonly Color _borderColor;
    // properties of the bodies that are being spawned
    private float _spawnRectRestitution;
    private float _spawnCircleRestitution;
    private float _spawnRectMass;
    private float _spawnCircleMass;
    private float _spawnRectHeight;
    private float _spawnRectWidth;
    private float _spawnCircleRadius;
    private float _spawnAngle;
    private bool _spawnIsStatic;
    private float _spawnDk;
    private float _spawnSk;
    private bool _areContactPointsVisible;
    
    public void Start()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1600,1000, "engine");
        rlImGui.Setup();
        
        //_engine.AddRectangle(new Vector2( 400, 110), 0.8f, 10f, false, 100f, 100f, 0f);
        //_engine.AddRectangle(new Vector2(800, 800f), 0.8f, 1f, true, 1000, 80f, 0f);
        _engine.AddRectangle(new Vector2(800f, 800f), 0.8f, 1f, true, 1400f, 80f, 0f, 0.1f, 0.2f);
        _engine.AddRectangle(new Vector2(450f, 300f), 0.6f, 1f, true, 800, 80f, 10, 0.1f, 0.2f);
        _engine.AddRectangle(new Vector2(1200f, 500f), 0.6f, 1f, true, 700, 80f, -10, 0.1f, 0.2f);
        _run = true;
        Raylib.SetTargetFPS(60);
        ApplicationLoop();
    }
    
    private void Render(List<RigidBody2> bodies)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.BeginMode2D(_camera);

        // render the fancy background
        _currentScreenHeight = Raylib.GetScreenHeight();
        _currentScreenWidth = Raylib.GetScreenWidth();
        //RenderBackground(_currentScreenWidth, _currentScreenWidth);
        foreach (var body in bodies)
        {
            if (body is Circle2D circle)
            {
                Raylib.DrawCircle((int)(circle.Position.X), (int)(circle.Position.Y), circle.Radius , _borderColor);
                Raylib.DrawCircle((int)(circle.Position.X), (int)(circle.Position.Y), circle.Radius - 2f , Color.Red);
                //Raylib.DrawLineEx(circle.Position, circle.PointForRotation, 2f, Color.Black);
            }
            if (body is Rectangle2D rectangle)
            {
                Rectangle rect = new Rectangle(rectangle.Position.X, rectangle.Position.Y, rectangle.Width, rectangle.Height);
                Vector2 origin = new Vector2(rectangle.Width / 2f , rectangle.Height / 2f );
                Rectangle rectFill = new Rectangle(rectangle.Position.X, rectangle.Position.Y, rectangle.Width - 4, rectangle.Height - 4);
                Vector2 originFill = new Vector2((rectangle.Width - 4f) / 2f , (rectangle.Height - 4f) / 2f );
                Raylib.DrawRectanglePro(rect, origin, (float)(rectangle.Angle / Math.PI * 180), _borderColor);
                Raylib.DrawRectanglePro(rectFill, originFill, (float)(rectangle.Angle / Math.PI * 180), Color.Blue);
            }
        }

        if (_areContactPointsVisible)
        {
            foreach (var cp in _engine.CPs)
            {
                Raylib.DrawCircle((int)cp.X, (int)cp.Y, 3f, Color.Green);
            } 
        }
        Raylib.EndMode2D();
        HandleGui();
        Raylib.EndDrawing();
    }

    private void RenderBackground(int w, int h)
    {
        Vector2 startPoint = new Vector2(-20f, -20f);
        Vector2 endPoint = new Vector2(-20f, h + 20);
        Vector2 differenceX = new Vector2(100f, 0f);
        Vector2 differenceY = new Vector2(0f, 100f);
        
        while (startPoint.X < w)
        {
            Raylib.DrawLineEx(startPoint, endPoint, 1f, _lineColor);
            startPoint += differenceX;
            endPoint += differenceX;
        }
        
        startPoint = new Vector2(-20f, -20f);
        endPoint = new Vector2(w + 20, -20f);
        
        while (startPoint.Y < h)
        {
            Raylib.DrawLineEx(startPoint, endPoint, 1f, _lineColor);
            startPoint += differenceY;
            endPoint += differenceY;
        }
    }
    private void HandleGui()
    {
        float g = _engine.Gravitation / 10;
        if (Raylib.IsKeyPressed(KeyboardKey.I))
        {
            _iSGuiVisible = !_iSGuiVisible;
        }
        rlImGui.Begin();
        if (_iSGuiVisible)
        {
            ImGui.SetNextWindowPos(new Vector2(400, 50), ImGuiCond.Appearing);
            ImGui.SetNextWindowSize(new Vector2(520, 300), ImGuiCond.Appearing);
            if (ImGui.Begin("Settings"))
            {
                ImGui.SliderFloat("Gravitation", ref g, 10f, 500f);
                _engine.Gravitation = g * 10;
                ImGui.Checkbox("Show FPS", ref _iSFpsVisible);
                ImGui.Checkbox("Show BodyCount", ref _iSBodyCountVisible);
                ImGui.Checkbox("Show contact points", ref _areContactPointsVisible);
            }

            if (ImGui.CollapsingHeader("Spawning bodies params"))
            {
                ImGui.InputFloat("Rectangle mass", ref _spawnRectMass);
                ImGui.InputFloat("Circle mass", ref _spawnCircleMass);
                ImGui.InputFloat("Circle restitution", ref _spawnCircleRestitution);
                ImGui.InputFloat("Rectangle restitution", ref _spawnRectRestitution);
                ImGui.InputFloat("Circle radius", ref _spawnCircleRadius);
                ImGui.InputFloat("Rectangle height", ref _spawnRectHeight);
                ImGui.InputFloat("Rectangle width", ref _spawnRectWidth);
                ImGui.InputFloat("Body angle", ref _spawnAngle);
                ImGui.InputFloat("Dynamic friction", ref _spawnDk);
                ImGui.InputFloat("Static friction", ref _spawnSk);
                ImGui.Checkbox("Is body static", ref _spawnIsStatic);
            }
            
            _debugWindowLocation = ImGui.GetWindowPos();
            _debugWindowHeight = ImGui.GetWindowHeight();
            _debugWindowWidth = ImGui.GetWindowWidth();
            ImGui.End();
        }
        rlImGui.End();

        if (_iSFpsVisible && _iSBodyCountVisible)
        {
            Raylib.DrawText(Convert.ToString(_fps), 5, 5, 30, Color.Black);
            Raylib.DrawText(Convert.ToString(_engine.GetBodies().Count), 5, 40, 30, Color.Black);
        }
        else if (_iSFpsVisible)
        {
            Raylib.DrawText(Convert.ToString(_fps), 5, 5, 30, Color.Black);
        }
        else if (_iSBodyCountVisible)
        {
            Raylib.DrawText(Convert.ToString(_engine.GetBodies().Count), 5, 5, 30, Color.Black);
        }
    }
    private void PauseOrResume()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            _run = !_run;
        }
    }
    private void TrySpawnBodies()
    {
        Vector2 mousePos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), _camera);
        
        Console.WriteLine(mousePos);
        if (_iSGuiVisible)
        {
            if (mousePos.X >= _debugWindowLocation.X && mousePos.X <= _debugWindowLocation.X + _debugWindowWidth &&
                mousePos.Y >= _debugWindowLocation.Y && mousePos.Y <= _debugWindowLocation.Y + _debugWindowHeight)
            {
                return;
            }
        }
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            _engine.AddCircle(mousePos, _spawnCircleRestitution, _spawnCircleMass, _spawnIsStatic, _spawnCircleRadius, _spawnAngle, _spawnDk, _spawnSk);
        }
        else if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            _engine.AddRectangle(mousePos, _spawnRectRestitution, _spawnRectMass, _spawnIsStatic, _spawnRectWidth, _spawnRectHeight, _spawnAngle, _spawnDk, _spawnSk);
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

    private void ResizeBoundaries()
    {
        _engine.EditBoundaries(Raylib.GetScreenToWorld2D(new Vector2(_currentScreenWidth, 0), _camera), 
            Raylib.GetScreenToWorld2D(new Vector2(_currentScreenHeight, 0), _camera));
    }

    private void ApplicationLoop()
    {
        while (!Raylib.WindowShouldClose())
        {
            PauseOrResume();
            if (_run)
            {
                TrySpawnBodies();
                TryZoom();
                
                _fps = Raylib.GetFPS();
                _engine.Step(Raylib.GetFrameTime());
            }
            Render(_engine.GetBodies());
        }
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    public Application(Engine engine)
    {
        _engine = engine;
        _run = false;
        _iSGuiVisible = false;
        _iSFpsVisible = true;
        _iSBodyCountVisible = true;
        _spawnRectRestitution = 0.8f;
        _spawnCircleRestitution = 0.8f;
        _spawnRectMass = 100f;
        _spawnCircleMass = 30f;
        _spawnRectHeight = 100f;
        _spawnRectWidth = 100f;
        _spawnCircleRadius = 30f;
        _spawnAngle = 0f;
        _spawnIsStatic = false;
        _spawnDk = 0.1f;
        _spawnSk = 0.2f;
        _areContactPointsVisible = false;
        _lineColor = new Color(255, 255, 255, 80);
        _bgColor = new Color(2, 2, 13, 255);
        _borderColor = new Color(0, 0, 0, 200);
        _camera = new Camera2D();
        _camera.Target = new Vector2(400,300);
        _camera.Offset = new Vector2(400,300);
        _camera.Zoom = 1f;
        _fps = Raylib.GetFPS();
    }
}


