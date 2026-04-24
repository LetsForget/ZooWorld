namespace ZooWorld.Levels
{
    public interface ILevelsProvider
    {
        LevelDTO[] GetLevels();
        
        void SetCurrentLevel(int level);
        
        bool TryGetLevel(out LevelDTO level);
    }
}