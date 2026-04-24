using System.Collections.Generic;
using Logging;
using Zenject;

namespace ZooWorld.Animals.Movement
{
    public class LocomotionsFactory
    {
        private readonly IReadOnlyDictionary<LocomotionType, ILocomotionFactory> factories;
        private readonly ILogsWriter logsWriter;
        
        public LocomotionsFactory(DiContainer container, ILogsWriter logsWriter)
        {
            factories = new Dictionary<LocomotionType, ILocomotionFactory>
            {
                { LocomotionType.Jump, container.Instantiate<JumpLocomotionFactory>() },
                { LocomotionType.Linear, container.Instantiate<LinearLocomotionFactory>() }
            };

            this.logsWriter = logsWriter;
        }

        public ILocomotion Create(LocomotionType type, AnimalContainer container, ILocomotionConfig config)
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
