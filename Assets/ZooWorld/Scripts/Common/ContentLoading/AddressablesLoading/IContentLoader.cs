using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace ContentLoading
{
    public interface IContentLoader
    {
        UniTask Preload(AssetReference assetRef);
        
        UniTask Preload(IEnumerable<AssetReference> assetRefs);
        
        UniTask<T> Load<T>(AssetReference assetRef);
        
        UniTask<T> LoadComponent<T>(AssetReference assetRef);
        
        UniTask<SceneInstance> LoadScene(AssetReference assetRef, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100,
            SceneReleaseMode sceneReleaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
        
        void Release(AssetReference assetReference);
        UniTask ReleaseScene(SceneInstance scene, UnloadSceneOptions unloadOptions, bool autoReleaseHandle = true);
    }
}