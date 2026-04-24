using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZooWorld.Animals;

namespace ZooWorld.Levels
{
    [Serializable]
    public struct LevelDTO
    {
        public AnimalDTO[] animalDTOs;
        
        public SpawnerType spawnerType;
        [SerializeReference] public IAnimalSpawnerConfig spawnerConfig;
        
        public AssetReference levelSceneRef;
    }
}