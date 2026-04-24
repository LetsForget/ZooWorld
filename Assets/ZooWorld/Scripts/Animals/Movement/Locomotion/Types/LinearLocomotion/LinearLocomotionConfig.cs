using System;
using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    [Serializable]
    public struct LinearLocomotionConfig : ILocomotionConfig
    {
        [field: SerializeField] public float Speed { get; private set; }
    }
}