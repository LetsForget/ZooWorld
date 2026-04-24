using System;

namespace ZooWorld.Animals.Movement
{
    [Serializable]
    public struct RandomDirectionConfig : IDirectionSelectorConfig
    {
        public float directionChangePeriod;
    }
}