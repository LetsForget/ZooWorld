using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class LinearLocomotionFactory : ILocomotionFactory
    {
        private readonly DiContainer container;

        public LinearLocomotionFactory(DiContainer container)
        {
            this.container = container;
        }
        
        public ILocomotion Create(AnimalContainer container, ILocomotionConfig config)
        {
            return this.container.Instantiate<LinearLocomotion>(new object[] { container, config });
        }
    }
}
