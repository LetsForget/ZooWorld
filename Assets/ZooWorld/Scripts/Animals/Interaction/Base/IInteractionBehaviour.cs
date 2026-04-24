namespace ZooWorld.Animals
{
    public interface IInteractionBehaviour
    {
        void HandleCollision(Animal self, Animal other);
    }
}
