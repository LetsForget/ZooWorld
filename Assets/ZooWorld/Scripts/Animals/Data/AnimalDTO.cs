using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    [Serializable]
    public struct AnimalDTO
    {
        public AnimalGroup animalGroup;
        public string animalKey;
        
        [SerializeReference] public IDirectionSelectorConfig directionSelectorConfig;
        [SerializeReference] public ILocomotionConfig locomotionConfig;
        [SerializeReference] public IInteractionConfig interactionConfig;
        
        public AssetReference animalContainerRef;
    }
}
