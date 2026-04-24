using System;
using UnityEngine;

namespace ZooWorld.Animals.Movement
{
    public sealed class JumpLocomotion : ILocomotion
    {
        private const float JumpAngleRadians = Mathf.PI / 4f;
        
        private readonly AnimalContainer container;
        private readonly JumpLocomotionConfig config;
        
        private readonly IGroundChecker groundChecker;
        
        private DateTime lastJumpTime;
        
        private JumpMovementState state;

        public JumpLocomotion(AnimalContainer container, JumpLocomotionConfig config, IGroundChecker groundChecker)
        {
            this.container = container;
            this.config = config;

            this.groundChecker = groundChecker;
        }

        public void Move(Vector3 direction, float deltaTime)
        {
            switch (state)
            {
                case JumpMovementState.Grounded:
                {
                    HandleGrounded(direction);
                    break;
                }
                case JumpMovementState.Jumping:
                {
                    HandleJumping();
                    break;
                }
                case JumpMovementState.Delay:
                {
                    HandleDelay();
                    break;
                }
            }
        }
        
        private void HandleGrounded(Vector3 direction)
        {
            var jumpDistance = config.JumpDistance;
            
            var speed = Mathf.Sqrt(jumpDistance * Mathf.Abs(Physics.gravity.y));
            var horizontalVelocity = speed * Mathf.Cos(JumpAngleRadians);
            var verticalVelocity = speed * Mathf.Sin(JumpAngleRadians);
            
            var velocity = direction.normalized * horizontalVelocity + Vector3.up * verticalVelocity;

            container.Rigidbody.AddForce(velocity, ForceMode.VelocityChange);
            state = JumpMovementState.Jumping;
        }

        private void HandleJumping()
        {
            if (!groundChecker.Check(container))
            {
                return;
            }
            
            state = JumpMovementState.Delay;
            lastJumpTime = DateTime.Now;
        }

        private void HandleDelay()
        {
            var timePassed = DateTime.Now - lastJumpTime;
            
            if (timePassed.TotalSeconds < config.DelayAfterLanding)
            {
                return;
            }
            
            state = JumpMovementState.Grounded;
        }
    }
    
    public enum JumpMovementState
    {
        Grounded,
        Jumping,
        Delay
    }
}