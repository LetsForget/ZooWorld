using System;

namespace ZooWorld.Animals
{
    [Serializable]
    public struct RandomSpawnerConfig : IAnimalSpawnerConfig
    {
        public float spawnPeriod;
    }
}