using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public sealed class GroundChecker : IGroundChecker
    {
        private const float GroundedDistanceOffset = 0.05f;

        private readonly LayerMask groundLayer;

        public GroundChecker(LayerMask groundLayer)
        {
            this.groundLayer = groundLayer;
        }

        public bool Check(IGroundCheckable component)
        {
            var rayDistance = component.Height * 0.5f + GroundedDistanceOffset;
            var position = component.Rigidbody.position;
            
            return Physics.Raycast(position, Vector3.down, rayDistance, groundLayer, QueryTriggerInteraction.Ignore);
        }
    }
}