using UnityEngine.ResourceManagement.ResourceProviders;

namespace ZooWorld.Levels
{
    public class CurrentLevelRuntime : ICurrentLevelRuntime
    {
        private bool hasLoadedScene;
        private bool hasLevelContainer;

        private SceneInstance loadedScene;

        public LevelContainer LevelContainer { get; private set; }

        public void SetLevel(SceneInstance scene, LevelContainer levelContainer)
        {
            loadedScene = scene;
            LevelContainer = levelContainer;

            hasLoadedScene = true;
            hasLevelContainer = !ReferenceEquals(levelContainer, null);
        }

        public bool TryGetLoadedScene(out SceneInstance scene)
        {
            scene = loadedScene;
            return hasLoadedScene;
        }
        
        public void Clear()
        {
            loadedScene = default;
            LevelContainer = null;

            hasLoadedScene = false;
            hasLevelContainer = false;
        }
    }
}
