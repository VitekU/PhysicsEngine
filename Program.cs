namespace engine;

internal abstract class Program
{
    static void Main(string[] args) 
    {
        Engine stroj = new Engine(100, 1000000, 10, 1000f, 2000f);
            
        Application app = new Application(stroj);
        app.Start();
    }
}

/* TODO
 * boundaries - napul hotove, chybi zoom
 * narrow & broad phase
 * AABB
 */