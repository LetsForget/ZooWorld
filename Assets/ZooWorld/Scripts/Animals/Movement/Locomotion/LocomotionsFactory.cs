using System.Collections.Generic;
using Logging;
using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class LocomotionsFactory
    {
        private readonly DiContainer container;
        private readonly ILogsWriter logsWriter;
        
        public LocomotionsFactory(DiContainer container, ILogsWriter logsWriter)
        {
            this.container = container;
            this.logsWriter = logsWriter;
        }

        public ILocomotion Create(AnimalContainer animalContainer, ILocomotionConfig config)
        {
            switch (config)
            {
                case JumpLocomotionConfig:
                    return container.Instantiate<JumpLocomotion>(new object[] { animalContainer, config });
                case LinearLocomotionConfig:
                    return container.Instantiate<LinearLocomotion>(new object[] { animalContainer, config });
                default:
                    logsWriter.LogError("Unknown Locomotion Config");
                    return null;
            }
        }
    }
}
