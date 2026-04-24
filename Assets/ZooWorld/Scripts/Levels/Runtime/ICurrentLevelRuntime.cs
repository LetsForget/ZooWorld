using UnityEngine.ResourceManagement.ResourceProviders;

namespace ZooWorld.Levels
{
    public interface ICurrentLevelRuntime
    {
        LevelContainer LevelContainer { get; }
        
        void SetLevel(SceneInstance scene, LevelContainer levelContainer);

        bool TryGetLoadedScene(out SceneInstance scene);
        
        void Clear();
    }
}
