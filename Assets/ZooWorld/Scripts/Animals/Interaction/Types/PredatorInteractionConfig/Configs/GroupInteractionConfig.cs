using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    [Serializable]
    public struct GroupInteractionConfig : IPredatorInteractionConfig
    {
        [SerializeField] public AnimalGroup[] groupsToEat;
        
        public bool Check(Animal animal)
        {
            foreach (var group in groupsToEat)
            {
                if (group == animal.Group)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}