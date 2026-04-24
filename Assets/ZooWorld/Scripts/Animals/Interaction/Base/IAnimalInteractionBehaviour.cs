namespace ZooWorld.Animals
{
    public interface IAnimalInteractionBehaviour
    {
        void HandleCollision(Animal self, Animal other);
    }
}
