namespace engine;

internal abstract class Program
{
    static void Main(string[] args) 
    {
        Engine stroj = new Engine(100, 100, 10, 300f, 1000f);
            
        Application app = new Application(stroj);
        app.Start();
    }
}