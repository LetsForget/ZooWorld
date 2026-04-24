using System;
using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.Animals.Movement
{
    [Serializable]
    public struct JumpLocomotionConfig : ILocomotionConfig
    {
        [field: SerializeField] public float JumpDistance { get; private set; }
        [field: SerializeField] public float DelayAfterLanding { get; private set; }
    }
}