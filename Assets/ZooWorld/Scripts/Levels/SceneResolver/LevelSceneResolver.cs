using UnityEngine.ResourceManagement.ResourceProviders;

namespace ZooWorld.Levels
{
    public class LevelSceneResolver : ILevelSceneResolver
    {
        public bool TryResolve(SceneInstance sceneInstance, out LevelContainer levelContainer)
        {
            var scene = sceneInstance.Scene;

            if (!scene.IsValid() || !scene.isLoaded)
            {
                levelContainer = null;
                return false;
            }

            foreach (var rootObject in scene.GetRootGameObjects())
            {
                levelContainer = rootObject.GetComponentInChildren<LevelContainer>(true);

                if (levelContainer != null)
                {
                    return true;
                }
            }

            levelContainer = null;
            return false;
        }
    }
}
