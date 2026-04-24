using System.Collections.Generic;
using Logging;
using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class DirectionSelectorsFactory
    {
        private readonly IReadOnlyDictionary<DirectionSelectType, IDirectionSelectorFactory> factories;
        private readonly ILogsWriter logsWriter;
        
        public DirectionSelectorsFactory(DiContainer container, ILogsWriter logsWriter)
        {
            factories = new Dictionary<DirectionSelectType, IDirectionSelectorFactory>
            {
                { DirectionSelectType.Random, container.Instantiate<RandomSelectorFactory>() },
            };

            this.logsWriter = logsWriter;
        }

        public IDirectionSelector Create(DirectionSelectType type, AnimalContainer container, IDirectionSelectorConfig config)
        {
            if (!factories.TryGetValue(type, out var factory))
            {
                logsWriter.LogError($"LocomotionFactory: Unknown locomotion type: {type}");
                return null;
            }

            return factory.Create(container, config);
        }
    }
}
