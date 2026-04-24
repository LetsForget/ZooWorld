using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalCollisionRelay : MonoBehaviour
    {
        private AnimalRuntime runtime;
        
        public void Initialize(AnimalRuntime runtime)
        {
            this.runtime = runtime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var otherRelay = collision.rigidbody
                ? collision.rigidbody.GetComponent<AnimalCollisionRelay>()
                : collision.collider.GetComponent<AnimalCollisionRelay>();

            DispatchCollision(otherRelay);
        }
        
        private void DispatchCollision(AnimalCollisionRelay otherRelay)
        {
            if (runtime == null || !otherRelay || otherRelay.runtime == null)
            {
                return;
            }

            if (ReferenceEquals(this, otherRelay))
            {
                return;
            }

            if (GetInstanceID() > otherRelay.GetInstanceID())
            {
                return;
            }
            
            runtime.Animal.HandleCollision(otherRelay.runtime.Animal);
            otherRelay.runtime.Animal.HandleCollision(runtime.Animal);
        }
    }
}