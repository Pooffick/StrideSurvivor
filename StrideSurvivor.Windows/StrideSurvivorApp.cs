using Stride.Engine;
using Stride.Games;

namespace StrideSurvivor
{
    class StrideSurvivorApp
    {
        static void Main(string[] args)
        {
            using var game = new Game();
            game.WindowCreated += OnGameWindowCreated;
            game.Run();
        }

        private static void OnGameWindowCreated(object sender, System.EventArgs e)
        {
            (sender as Game).Window.AllowUserResizing = true;
        }
    }
}
