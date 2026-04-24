namespace Common.Timer
{
    public struct CoolDownTimer
    {
        public Timer timer;
        
        public bool ReadyToUse { get; private set; }

        public CoolDownTimer(float interval)
        {
            timer = new Timer(interval);
            ReadyToUse = true;
        }

        public void SetCoolDown()
        {
            ReadyToUse = false;
            timer.timePast = 0f;
        }
        
        public void Update(float deltaTime)
        {
            if (ReadyToUse)
            {
                return;
            }
            
            ReadyToUse = timer.Evaluate(deltaTime);
        }
    }
}