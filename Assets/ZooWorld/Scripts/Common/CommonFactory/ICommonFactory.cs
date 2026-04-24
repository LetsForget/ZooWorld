using UnityEngine;

namespace Common
{
    public interface ICommonFactory
    {
        /// <summary>
        /// Создаёт новый экземпляр класса.
        /// </summary>
        T FromNew<T>() where T : class;

        /// <summary>
        /// Создаёт новый экземпляр класса с передачей параметров в конструктор.
        /// </summary>
        T FromNew<T>(params object[] args) where T : class;

        /// <summary>
        /// Создаёт компонент T на существующем GameObject.
        /// </summary>
        T FromNewComponentOn<T>(GameObject gameObject) where T : MonoBehaviour;

        /// <summary>
        /// Создаёт компонент T на новом GameObject с указанным именем.
        /// </summary>
        T FromNewComponentOnNewGameObject<T>(string gameObjectName = null) where T : MonoBehaviour;

        /// <summary>
        /// Создаёт экземпляр префаба и возвращает компонент типа T с корневого объекта.
        /// </summary>
        T FromPrefab<T>(GameObject prefab) where T : Component;

        /// <summary>
        /// Создаёт экземпляр префаба и возвращает сам GameObject.
        /// </summary>
        GameObject FromPrefab(GameObject prefab, Transform parent = null);

        /// <summary>
        /// Создаёт экземпляр префаба под указанным родителем и возвращает компонент типа T.
        /// </summary>
        T FromPrefabUnderParent<T>(T prefab, Transform parent) where T : Component;

        /// <summary>
        /// Создаёт экземпляр префаба в указанной позиции и с указанным поворотом, возвращает компонент T.
        /// </summary>
        T FromPrefabAt<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component;

        /// <summary>
        /// Создаёт экземпляр ScriptableObject типа T.
        /// </summary>
        T FromScriptableObject<T>() where T : ScriptableObject;
    }
}