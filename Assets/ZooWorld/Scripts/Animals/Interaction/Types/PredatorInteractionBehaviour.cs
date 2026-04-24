namespace ZooWorld.Animals
{
    public sealed class PredatorInteractionBehaviour : IAnimalInteractionBehaviour
    {
        public void HandleCollision(Animal self, Animal other)
        {
            if (other == null)
            {
                return;
            }

            switch (other.Type)
            {
                case AnimalType.Prey:
                case AnimalType.Predator:
                    other.MarkDead();
                    break;
            }
        }
    }
}