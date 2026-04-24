using UnityEngine;
using ZooWorld.Animals.Movement;

namespace ZooWorld.Animals
{
    public class Animal
    {
        private readonly IDirectionSelector directionSelector;
        private readonly ILocomotion locomotion;
        private readonly IInteractionBehaviour interactionBehaviour;

        private Vector3 currentDirection;

        public AnimalGroup Group { get; }
        public string Key { get; }
        public bool IsDead { get; private set; }
        
        public Animal(AnimalGroup group, string key, IDirectionSelector directionSelector, ILocomotion locomotion,
            IInteractionBehaviour interactionBehaviour)
        {
            this.directionSelector = directionSelector;
            this.locomotion = locomotion;
            this.interactionBehaviour = interactionBehaviour;
            
            Group = group;
            Key = key;
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