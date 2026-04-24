using System.Collections.Generic;
using Logging;

namespace ZooWorld.Animals
{
    public sealed class AnimalInteractionBehavioursFactory
    {
        private readonly IReadOnlyDictionary<AnimalType, IAnimalInteractionBehaviour> behaviours;
        private readonly ILogsWriter logsWriter;

        public AnimalInteractionBehavioursFactory(ILogsWriter logsWriter)
        {
            this.logsWriter = logsWriter;
            behaviours = new Dictionary<AnimalType, IAnimalInteractionBehaviour>
            {
                { AnimalType.Predator, new PredatorInteractionBehaviour() },
                { AnimalType.Prey, new PreyInteractionBehaviour() }
            };
        }

        public IAnimalInteractionBehaviour Create(AnimalType type)
        {
            if (behaviours.TryGetValue(type, out var behaviour))
            {
                return behaviour;
            }

            logsWriter.LogError($"Unknown animal interaction type: {type}");
            return new PreyInteractionBehaviour();
        }
    }
}