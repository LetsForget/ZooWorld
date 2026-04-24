namespace ZooWorld.Animals
{
    public interface IPredatorInteractionConfig : IInteractionConfig
    {
        bool Check(Animal animal);
    }
}