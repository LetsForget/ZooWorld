using UnityEngine;

namespace ZooWorld.Animals
{
    [CreateAssetMenu(menuName = "ZooWorld/Create New Animal", fileName = "New Animal")]
    public class AnimalSO : ScriptableObject
    {
        [field: SerializeField] public AnimalDTO DTO { get; private set; }
    }
}