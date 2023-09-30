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
            var game = (Game)sender;
            game.Window.AllowUserResizing = true; // TODO: not working properly with "AdaptBackBufferToScreen" but it adapts anyway?
        }
    }
}
