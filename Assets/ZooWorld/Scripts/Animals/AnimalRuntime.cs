namespace ZooWorld.Animals
{
    public sealed class AnimalRuntime
    {
        public Animal Animal { get; }
        public AnimalContainer Container { get; }

        public AnimalRuntime(Animal animal, AnimalContainer container)
        {
            Animal = animal;
            Container = container;
        }
    }
}