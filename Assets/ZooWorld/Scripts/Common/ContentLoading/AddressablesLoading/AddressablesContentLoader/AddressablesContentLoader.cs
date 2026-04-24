using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ContentLoading
{
    public class AddressablesContentLoader : IContentLoader
    {
        private readonly Dictionary<AssetReference, AsyncOperationHandle> assetCache = new();

        public UniTask Preload(AssetReference assetRef)
        {
            if (assetCache.ContainsKey(assetRef))
            {
                return UniTask.CompletedTask;
            }
            
            var handle = Addressables.LoadAssetAsync<Object>(assetRef);
            assetCache.Add(assetRef, handle);

            return handle.ToUniTask();
        }

        public async UniTask Preload(IEnumerable<AssetReference> assetRefs)
        {
            foreach (var content in assetRefs)
            {
                await Preload(content);
            }
        }
        
        public async UniTask<T> Load<T>(AssetReference assetRef)
        {
            var cached = await CheckCache<T>(assetRef, false);

            if (cached.wasInCache)
            {
                return cached.result;
            }
            
            var handle = Addressables.LoadAssetAsync<T>(assetRef);
            assetCache.Add(assetRef, handle);

            await handle.Task.AsUniTask();
            return handle.Result;
        }
        
        public async UniTask<T> LoadComponent<T>(AssetReference assetRef)
        {
            var cached = await CheckCache<T>(assetRef, true);

            if (cached.wasInCache)
            {
                return cached.result;
            }
            
            var handle = Addressables.LoadAssetAsync<GameObject>(assetRef);
            assetCache.Add(assetRef, handle);

            await handle.Task.AsUniTask();
            return handle.Result.GetComponent<T>();
        }
        
        public async UniTask<SceneInstance> LoadScene(AssetReference assetRef, LoadSceneMode mode = LoadSceneMode.Single, 
            bool activateOnLoad = true, int priority = 100, SceneReleaseMode sceneReleaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
        {
            await UniTask.SwitchToMainThread();
            
            var handle = Addressables.LoadSceneAsync(assetRef, mode, activateOnLoad, priority, sceneReleaseMode);
            await handle.Task.AsUniTask();

            return handle.Result;
        }

        private async UniTask<CacheCheckResult<T>> CheckCache<T>(AssetReference assetRef, bool isComponent)
        {
            if (assetCache.TryGetValue(assetRef, out var loadingHandle))
            {
                await loadingHandle.Task.AsUniTask();

                if (loadingHandle.Status == AsyncOperationStatus.Failed)
                {
                    assetCache.Remove(assetRef);
                    throw new LoadingFailedException(assetRef.SubObjectName);
                }

                var result = isComponent ? ((GameObject)loadingHandle.Result).GetComponent<T>() : (T)loadingHandle.Result;
                
                return new CacheCheckResult<T>(result, true);
            }

            return new CacheCheckResult<T>(default, false);
        }
        
        public void Release(AssetReference path)
        {
            if (!assetCache.TryGetValue(path, out var assetHandle))
            {
                return;
            }
            
            assetHandle.Release();
            assetCache.Remove(path);
        }

        public UniTask ReleaseScene(SceneInstance scene, UnloadSceneOptions unloadOptions, bool autoReleaseHandle = true)
        {
            return Addressables.UnloadSceneAsync(scene, unloadOptions, autoReleaseHandle).ToUniTask();
        }
        
        private readonly struct CacheCheckResult<T>
        {
            public readonly T result;
            public readonly bool wasInCache;

            public CacheCheckResult(T result, bool wasInCache)
            {
                this.result = result;
                this.wasInCache = wasInCache;
            }
        }
    }
}