namespace Common.Pool
{
    public interface IPoolable
    {
        public void OnTakenFromPool();

        public void OnReturnedBackToPool();
    }
}