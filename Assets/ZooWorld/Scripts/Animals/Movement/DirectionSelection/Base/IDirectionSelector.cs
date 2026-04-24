using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public interface IDirectionSelector
    {
        Vector3 UpdateDirection(Vector3 oldDirection, float deltaTime);
    }
}