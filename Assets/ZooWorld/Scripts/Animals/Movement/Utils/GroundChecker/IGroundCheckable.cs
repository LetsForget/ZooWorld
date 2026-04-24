using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public interface IGroundCheckable
    {
        Rigidbody Rigidbody { get; }
        float Height { get; }
    }
}