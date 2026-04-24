using Logging;
using Zenject;

namespace ZooWorld.Animals
{
    public class SpawnerFactory
    {
        private readonly DiContainer container;
        private readonly ILogsWriter logsWriter;
        
        public SpawnerFactory(DiContainer container, ILogsWriter logsWriter)
        {
            this.container = container;
            this.logsWriter = logsWriter;
        }
        
        public IAnimalsSpawner Create(IAnimalSpawnerConfig config)
        {
            switch (config)
            {
                case RandomSpawnerConfig:
                    return container.Instantiate<RandomSpawner>(new object[] { config });
                default:
                    logsWriter.LogError("Invalid spawner config");
                    return null;
            }
        }
    }
}
