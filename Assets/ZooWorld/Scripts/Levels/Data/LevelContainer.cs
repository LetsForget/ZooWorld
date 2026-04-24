using UnityEngine;
using Zenject;
using ZooWorld.Animals;

namespace ZooWorld.Levels
{
    public class LevelContainer : MonoBehaviour
    {
        [field: SerializeField] public Transform[] SpawnPoint { get; private set; }
        [field: SerializeField] public Camera Camera { get; private set; }
        
        public SpawnerFactory SpawnerFactory { get; private set; }
        
        [Inject]
        private void Construct(SpawnerFactory spawnerFactory)
        {
            SpawnerFactory = spawnerFactory;
        }
    }
}