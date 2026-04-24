using System.Collections.Generic;
using Logging;
using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class DirectionSelectorsFactory
    {
        private readonly DiContainer container;
        private readonly ILogsWriter logsWriter;
        
        public DirectionSelectorsFactory(DiContainer container, ILogsWriter logsWriter)
        {
            this.container = container;
            this.logsWriter = logsWriter;
        }

        public IDirectionSelector Create(AnimalContainer animalContainer, IDirectionSelectorConfig config)
        {
            switch (config)
            {
                case RandomDirectionConfig:
                    return container.Instantiate<RandomDirectionSelector>(new object[] { animalContainer, config });
                default:
                    logsWriter.LogError($"Invalid config: {config}");
                    return null;
            }
        }
    }
}
