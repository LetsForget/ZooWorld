using UnityEngine.ResourceManagement.ResourceProviders;

namespace ZooWorld.Levels
{
    public interface ILevelSceneResolver
    {
        bool TryResolve(SceneInstance sceneInstance, out LevelContainer levelContainer);
    }
}
