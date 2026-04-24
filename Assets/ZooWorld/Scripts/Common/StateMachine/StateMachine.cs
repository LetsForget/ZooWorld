using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.StateMachine
{
    public abstract class StateMachine<T, S> where T : Enum where S : State<T>
    {
        public event Action<T> StateChanged; 
        
        protected Dictionary<T, S> states;
        
        private readonly AsyncLock stateLock = new();
        
        private bool changingState;
        
        public S CurrentState { get; private set; }

        protected void InitializeStates()
        {
            foreach (var state in states)
            {
                state.Value.WantsToChangeState += OnStateWantsToChange;
            }
        }

        private void OnStateWantsToChange(T newStateType)
        {
            ChangeState(newStateType).Forget();
        }

        public async UniTask ChangeState(T type)
        {
            using (await stateLock.LockAsync())
            {
                changingState = true;
                
                if (CurrentState != null)
                {
                    await CurrentState.OnExit();
                }

                if (!states.TryGetValue(type, out var state))
                {
                    Debug.LogError($"State error: {type}");
                    return;
                }

                CurrentState = state;
                await CurrentState.OnEnter();
            
                StateChanged?.Invoke(type);
                
                changingState = false;
            }
        }
        
        public void UpdateSelf(float deltaTime)
        {
            if (changingState)
            {
                return;
            }
            
            CurrentState?.UpdateSelf(deltaTime);
        }

        public void FixedUpdateSelf()
        {
            CurrentState?.FixedUpdateSelf();
        }

        public void OnDrawGizmos()
        {
            CurrentState?.OnDrawGizmos();
        }
    }
}