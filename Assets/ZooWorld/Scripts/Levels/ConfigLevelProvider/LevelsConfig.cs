using UnityEngine;

namespace ZooWorld.Levels
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [field: SerializeField] public LevelDTO[] Levels { get; private set; }
    }
}