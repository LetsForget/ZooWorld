using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public interface IDirectionChecker
    {
        Vector3 CheckDirection(AnimalContainer container, Vector3 direction);
    }
}
