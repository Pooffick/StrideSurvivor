using Stride.Engine;
using Stride.Core;
using Stride.UI.Controls;
using Stride.UI.Events;
using Stride.Games;

namespace StrideSurvivor.UI
{
    public enum State
    {
        MainMenu,
        GameStarted,
        Pause,
        LevelUp,
        GameOver
    }

    public class GameState : StartupScript
    {
        private Button _exitButton;
        private Button _startButton;

        public Entity MainMenu;

        [DataMemberIgnore]
        public State CurrentState { get; private set; } = State.MainMenu;

        public static GameState Instance { get; private set; }

        public override void Start()
        {
            Instance = this;

            var mainMenuRoot = MainMenu.Get<UIComponent>().Page.RootElement;
            _exitButton = (Button)mainMenuRoot.FindName("ExitButton");
            _startButton = (Button)mainMenuRoot.FindName("StartButton");

            _exitButton.Click += OnExitClick;
            _startButton.Click += OnStartClick;
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            ((GameBase)Game).Exit(); // Why IGame doesn't contain Exit()?
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            MainMenu.Scene.Entities.Remove(MainMenu); // Kinda strange
            CurrentState = State.GameStarted;
        }
    }
}
