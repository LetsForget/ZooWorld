using UnityEngine;
using Zenject;

namespace Common
{
    public sealed class CommonFactory : ICommonFactory
    {
        private readonly DiContainer container;

        public CommonFactory(DiContainer container) => this.container = container;
    
        public T FromNew<T>() where T : class
        {
            return container.Instantiate<T>();
        }

        public T FromNew<T>(params object[] args) where T : class
        {
            return container.Instantiate<T>(args);
        }

        public T FromNewComponentOn<T>(GameObject gameObject) where T : MonoBehaviour
        {
            return container.InstantiateComponent<T>(gameObject);
        }

        public T FromNewComponentOnNewGameObject<T>(string gameObjectName = null) where T : MonoBehaviour
        {
            return container.InstantiateComponentOnNewGameObject<T>(gameObjectName ?? typeof(T).Name);
        }

        public T FromPrefab<T>(GameObject prefab) where T : Component
        {
            return container.InstantiatePrefabForComponent<T>(prefab);
        }

        public T FromPrefabUnderParent<T>(T prefab, Transform parent) where T : Component
        {
            return container.InstantiatePrefabForComponent<T>(prefab, parent);
        }

        public T FromPrefabAt<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var instance = container.InstantiatePrefab(prefab, position, rotation, parent);
            return instance.GetComponent<T>();
        }

        public GameObject FromPrefab(GameObject prefab, Transform parent = null)
        {
            return container.InstantiatePrefab(prefab, parent);
        }

        public T FromScriptableObject<T>() where T : ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }
    }
}