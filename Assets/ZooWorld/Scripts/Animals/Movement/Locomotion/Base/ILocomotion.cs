using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public interface ILocomotion
    {
        void Move(Vector3 direction, float deltaTime);
    }
}