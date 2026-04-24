namespace ZooWorld.Animals.Movement
{
    public interface ILocomotionFactory
    {
        ILocomotion Create(AnimalContainer container, ILocomotionConfig config);
    }
}