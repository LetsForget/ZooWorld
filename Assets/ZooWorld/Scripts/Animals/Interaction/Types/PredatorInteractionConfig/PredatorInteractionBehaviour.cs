namespace ZooWorld.Animals
{
    public sealed class PredatorInteractionBehaviour : IInteractionBehaviour
    {
        private readonly IPredatorInteractionConfig config;

        public PredatorInteractionBehaviour(IPredatorInteractionConfig config)
        {
            this.config = config;
        }
        
        public void HandleCollision(Animal self, Animal other)
        {
            if (other == null)
            {
                return;
            }

            if (config.Check(other))
            {
                other.MarkDead();
            }
        }
    }
}