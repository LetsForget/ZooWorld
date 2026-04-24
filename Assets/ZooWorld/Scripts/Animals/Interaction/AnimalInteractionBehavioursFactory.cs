using Logging;

namespace ZooWorld.Animals
{
    public sealed class AnimalInteractionBehavioursFactory
    {
        private readonly ILogsWriter logsWriter;

        public AnimalInteractionBehavioursFactory(ILogsWriter logsWriter)
        {
            this.logsWriter = logsWriter;
        }

        public IInteractionBehaviour Create(AnimalDTO animalDto)
        {
            var interactionConfig = animalDto.interactionConfig;

            switch (interactionConfig)
            {
                case AnimalKeyInteractionConfig keyInteractionConfig:
                    return new PredatorInteractionBehaviour(keyInteractionConfig);
                case GroupInteractionConfig groupInteractionConfig:
                    return new PredatorInteractionBehaviour(groupInteractionConfig);
                case PreyInteractionConfig preyInteractionConfig:
                    return new PreyInteractionBehaviour();
                default:
                    logsWriter.LogError("Invalid interaction config");
                    return null;
            }
        }
    }
}