using Zenject;

namespace ZooWorld.Animals
{
    public class RandomSpawnerFactory : IAnimalSpawnerFactory
    {
        private readonly DiContainer container;

        public RandomSpawnerFactory(DiContainer container)
        {
            this.container = container;
        }
        
        public IAnimalsSpawner Create(IAnimalSpawnerConfig config)
        {
            return container.Instantiate<RandomSpawner>(new object[] { config });
        }
    }
}
