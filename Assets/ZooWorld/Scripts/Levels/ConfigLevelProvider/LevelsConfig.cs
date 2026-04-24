using UnityEngine;

namespace ZooWorld.Levels
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "ZooWorld/LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [field: SerializeField] public LevelDTO[] Levels { get; private set; }
    }
}