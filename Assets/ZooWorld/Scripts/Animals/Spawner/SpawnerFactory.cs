using System.Collections.Generic;
using Logging;
using Zenject;

namespace ZooWorld.Animals
{
    public class SpawnerFactory
    {
        private readonly IReadOnlyDictionary<SpawnerType, IAnimalSpawnerFactory> spawnerFactories;
        private readonly ILogsWriter logsWriter;
        
        public SpawnerFactory(DiContainer container, ILogsWriter logsWriter)
        {
            spawnerFactories = new Dictionary<SpawnerType, IAnimalSpawnerFactory>
            {
                { SpawnerType.Random, container.Instantiate<RandomSpawnerFactory>() }
            };
            
            this.logsWriter = logsWriter;
        }
        
        public IAnimalsSpawner Create(SpawnerType spawnerType, IAnimalSpawnerConfig config)
        {
            if (!spawnerFactories.TryGetValue(spawnerType, out var factory))
            {
                logsWriter.LogError($"No factory for spawnerType '{spawnerType}'");
                return null;
            }
            
            return factory.Create(config);
        }
    }
}
