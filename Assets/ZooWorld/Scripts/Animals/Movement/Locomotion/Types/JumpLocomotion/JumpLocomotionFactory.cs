using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class JumpLocomotionFactory : ILocomotionFactory
    {
        private readonly DiContainer container;

        public JumpLocomotionFactory(DiContainer container)
        {
            this.container = container;
        }
        
        public ILocomotion Create(AnimalContainer container, ILocomotionConfig config)
        {
            return this.container.Instantiate<JumpLocomotion>(new object[] { container, config });
        }
    }
}
