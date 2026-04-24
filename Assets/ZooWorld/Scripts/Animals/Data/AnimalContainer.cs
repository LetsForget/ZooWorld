using UnityEngine;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    public class AnimalContainer : MonoBehaviour, IGroundCheckable
    {
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public AnimalCollisionRelay Relay { get; set; }
        [field: SerializeField] public float Height { get; private set; }
    }
}