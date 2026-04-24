using UnityEngine;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    public class Animal
    {
        private readonly IDirectionSelector directionSelector;
        private readonly ILocomotion locomotion;
        private readonly IAnimalInteractionBehaviour interactionBehaviour;

        private Vector3 currentDirection;

        public AnimalType Type { get; }
        public bool IsDead { get; private set; }
        
        public Animal(AnimalType type, IDirectionSelector directionSelector, ILocomotion locomotion,
            IAnimalInteractionBehaviour interactionBehaviour)
        {
            this.directionSelector = directionSelector;
            this.locomotion = locomotion;
            this.interactionBehaviour = interactionBehaviour;
            
            Type = type;
        }
        
        public void UpdateSelf(float deltaTime)
        {
            currentDirection = directionSelector.UpdateDirection(currentDirection, deltaTime);
            locomotion.Move(currentDirection, deltaTime);
        }

        public void HandleCollision(Animal other)
        {
            if (IsDead || other == null)
            {
                return;
            }

            interactionBehaviour?.HandleCollision(this, other);
        }

        public void MarkDead()
        {
            IsDead = true;
        }
    }
}