using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    [Serializable]
    public struct AnimalDTO
    {
        public AnimalType animalType;
        
        public DirectionSelectType directionSelectType;
        [SerializeReference] public IDirectionSelectorConfig directionSelectorConfig;
        
        public LocomotionType locomotionType;
        [SerializeReference] public ILocomotionConfig locomotionConfig;
        
        public AssetReference animalContainerRef;
    }
}
