namespace engine
{
    internal abstract class Program
    {
        static void Main(string[] args) 
        {
            Engine stroj = new Engine(10, 100, 10, 30f, 100f);
            
            Application app = new Application(stroj);
            app.Start();
        }
    }
}