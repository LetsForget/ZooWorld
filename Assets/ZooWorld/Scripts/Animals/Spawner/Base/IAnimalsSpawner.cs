using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    public interface IAnimalsSpawner
    {
        void Initialize(Transform[] spawnPoints, AnimalDTO[] variants, List<AnimalRuntime> targetList);
        
        void UpdateSelf(float deltaTime);
    }
}
