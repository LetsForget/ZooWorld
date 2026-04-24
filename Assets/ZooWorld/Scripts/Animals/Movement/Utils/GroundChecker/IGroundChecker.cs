namespace ZooWorld.Animals.Movement
{
    public interface IGroundChecker
    {
        bool Check(IGroundCheckable component);
    }
}
