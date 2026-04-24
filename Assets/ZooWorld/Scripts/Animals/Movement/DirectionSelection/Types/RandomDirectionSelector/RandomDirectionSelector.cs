using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZooWorld.Animals.Movement
{
    public class RandomDirectionSelector : IDirectionSelector
    {
        private readonly AnimalContainer container;
        private readonly RandomDirectionConfig config;
        
        private readonly IDirectionChecker directionChecker;

        private DateTime lastDirectionSwitchTime;
        
        public RandomDirectionSelector(AnimalContainer container, RandomDirectionConfig config, IDirectionChecker directionChecker)
        {
            this.container = container;
            this.config = config;
            
            this.directionChecker = directionChecker;
        }
        
        public Vector3 UpdateDirection(Vector3 oldDirection, float deltaTime)
        {
            var timePassed = DateTime.Now - lastDirectionSwitchTime;

            if (timePassed.TotalSeconds < config.directionChangePeriod)
            {
                return directionChecker.CheckDirection(container, oldDirection);
            }
            
            lastDirectionSwitchTime = DateTime.Now;
            var newDirection = new Vector3
            {
                x = Random.Range(-1f, 1f),
                y = 0,
                z = Random.Range(-1f, 1f)
            };
            
            return directionChecker.CheckDirection(container, newDirection.normalized);
        }
    }
}