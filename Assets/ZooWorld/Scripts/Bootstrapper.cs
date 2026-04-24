using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ZooWorld.StateMachine;

namespace ZooWorld
{
    public class Bootstrapper : MonoBehaviour
    {
        private GameStateMachine gameStateMachine;

        [Inject]
        private void Construct(GameStateMachine gameStateMachine)
        {
            this.gameStateMachine = gameStateMachine;
        }

        private void Awake()
        {
            gameStateMachine.ChangeState(GameStateType.Initializing).Forget();
            
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            gameStateMachine.UpdateSelf(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            gameStateMachine.FixedUpdateSelf();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            gameStateMachine.OnDrawGizmos();
        }
    }
}