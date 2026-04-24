namespace ZooWorld.Levels
{
    public class LevelProviderSO : ILevelsProvider
    {
        private readonly LevelsConfig config;

        private int currentLevel = -1;
        
        public LevelProviderSO(LevelsConfig config)
        {
            this.config = config;
        }
        
        public LevelDTO[] GetLevels()
        {
            return config.Levels;
        }

        public void SetCurrentLevel(int level)
        {
            currentLevel = level;
        }

        public bool TryGetLevel(out LevelDTO level)
        {
            if (currentLevel < 0 || currentLevel >= config.Levels.Length)
            {
                level = default;
                return false;
            }
            
            level = config.Levels[currentLevel];
            return true;
        }
    }
}