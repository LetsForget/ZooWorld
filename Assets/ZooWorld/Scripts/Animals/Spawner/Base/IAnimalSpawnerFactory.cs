namespace ZooWorld.Animals
{
    public interface IAnimalSpawnerFactory
    {
        IAnimalsSpawner Create(IAnimalSpawnerConfig config);
    }
}