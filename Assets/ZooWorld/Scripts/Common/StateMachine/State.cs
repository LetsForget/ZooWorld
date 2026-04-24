using System;
using Cysharp.Threading.Tasks;

namespace Common.StateMachine
{
    public abstract class State<T> where T : Enum
    {
        public event Action<T> WantsToChangeState;

        public abstract T Type { get; }
        
        public virtual UniTask  OnEnter() { return UniTask.CompletedTask; }
        
        public virtual void UpdateSelf(float deltaTime) { }
        
        public virtual void FixedUpdateSelf() { }
        
        public virtual void OnDrawGizmos() { }
        
        public virtual UniTask OnExit() { return UniTask.CompletedTask; }

        protected void RaiseChangeState(T newStateType)
        {
            WantsToChangeState?.Invoke(newStateType);
        }
    }
}