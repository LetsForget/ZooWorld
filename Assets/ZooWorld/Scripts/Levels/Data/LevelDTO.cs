using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZooWorld.Animals;

namespace ZooWorld.Levels
{
    [Serializable]
    public struct LevelDTO
    {
        public AnimalSO[] animals;
        
        [SerializeReference] public IAnimalSpawnerConfig spawnerConfig;
        
        public AssetReference levelSceneRef;
    }
}