using Stride.Engine;

namespace StrideSurvivor
{
    class StrideSurvivorApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
