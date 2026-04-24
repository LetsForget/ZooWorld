using System;

namespace ZooWorld.Animals
{
    [Serializable]
    public struct AnimalKeyInteractionConfig : IPredatorInteractionConfig
    {
        public string[] animalKeysToEat;
        
        public bool Check(Animal animal)
        {
            foreach (var animalKey in animalKeysToEat)
            {
                if (animalKey == animal.Key)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}