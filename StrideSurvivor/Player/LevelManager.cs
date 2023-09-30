using System;

namespace StrideSurvivor.Player
{
    public class LevelManager
    {
        private static readonly object LockObject = new();
        private int _currentExperience = 0;
        private int _experienceToNextLevel = 0;

        public int CurrentLevel { get; private set; }

        public static event Action LevelChanged;

        public void AddExperience()
        {
            lock (LockObject)
            {
                _currentExperience++;
                if (_currentExperience >= _experienceToNextLevel)
                {
                    CurrentLevel++;
                    CalculateExperienceToNextLevel();
                    LevelChanged?.Invoke();
                }
            }
        }

        public LevelManager() 
        {
            CurrentLevel = 1;
            _currentExperience = 0;
            CalculateExperienceToNextLevel();
        }

        private void CalculateExperienceToNextLevel()
        {
            _experienceToNextLevel = FibonacciByOrder(CurrentLevel + 2);
        }

        private static int FibonacciByOrder(int n)
        {
            if (n < 0)
                throw new ArgumentException($"{nameof(FibonacciByOrder)}. Invalid order.");
            
            int a = 0;
            int b = 1;
            for (int i = 0; i < n; i++)
            {
                int c = a + b;
                a = b;
                b = c;
            }

            return a;
        }
    }
}
