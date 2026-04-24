using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public class BoundsDirectionChecker : IDirectionChecker
    {
        private readonly Collider field;

        public BoundsDirectionChecker(Collider field)
        {
            this.field = field;
        }
        
        public Vector3 CheckDirection(AnimalContainer container, Vector3 direction)
        {
            var position = container.Rigidbody.position;

            if (field.bounds.Contains(position))
            {
                return direction;
            }
            
            var directionToField = field.bounds.center - position;
            return GetDirectionClosestToField(direction, directionToField);
        }
        
        private static Vector3 GetDirectionClosestToField(Vector3 direction, Vector3 directionToField)
        {
            if (direction == Vector3.zero || directionToField == Vector3.zero)
            {
                return direction;
            }

            var currentDirectionDot = Vector3.Dot(direction.normalized, directionToField.normalized);
            var oppositeDirection = -direction;
            var oppositeDirectionDot = Vector3.Dot(oppositeDirection.normalized, directionToField.normalized);

            return oppositeDirectionDot > currentDirectionDot ? oppositeDirection : direction;
        }
    }
}