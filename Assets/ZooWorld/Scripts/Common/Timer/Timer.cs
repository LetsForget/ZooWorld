namespace Common.Timer
{
    public struct Timer
    {
        public float timePast;
        public float interval;

        public Timer(float interval)
        {
            this.interval = interval;
            timePast = 0f;
        }
        
        public bool Evaluate(float deltaTime)
        {
            timePast += deltaTime;

            if (timePast >= interval)
            {
                timePast = 0;
                return true;
            }
            
            return false;
        }
    }
}