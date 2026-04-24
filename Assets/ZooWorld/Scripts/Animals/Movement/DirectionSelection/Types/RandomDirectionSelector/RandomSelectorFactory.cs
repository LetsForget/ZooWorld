using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class RandomSelectorFactory : IDirectionSelectorFactory
    {
        private readonly DiContainer container;

        public RandomSelectorFactory(DiContainer container)
        {
            this.container = container;
        }
        
        public IDirectionSelector Create(AnimalContainer container, IDirectionSelectorConfig config)
        {
            return this.container.Instantiate<RandomDirectionSelector>(new object[] { container, config });
        }
    }
}
