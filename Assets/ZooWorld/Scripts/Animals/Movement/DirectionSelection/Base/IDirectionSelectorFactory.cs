namespace ZooWorld.Animals.Movement
{
    public interface IDirectionSelectorFactory
    {
        IDirectionSelector Create(AnimalContainer container, IDirectionSelectorConfig config);
    }
}