using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public sealed class LinearLocomotion  : ILocomotion
    {
        private readonly AnimalContainer container;
        private readonly LinearLocomotionConfig config;


        public LinearLocomotion(AnimalContainer container, LinearLocomotionConfig config)
        {
            this.container = container;
            this.config = config;
        }
        
        public void Move(Vector3 direction, float deltaTime)
        {
            var rigidbody = container.Rigidbody;
            var speed = config.Speed;

            var nextPosition = rigidbody.position + direction * speed * deltaTime;
            rigidbody.MovePosition(nextPosition);
        }
    }
}
